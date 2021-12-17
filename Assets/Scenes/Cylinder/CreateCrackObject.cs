using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class CreateCrackObject : MonoBehaviour
{
    [SerializeField]
    private Transform cylinderPrefab;

    private GameObject leftSphere;
    private GameObject rightSphere;
    private GameObject cylinder;

    private GameObject emptyCrack;

    string path;
    string jsonText;

    // Start is called before the first frame update
    void Start()
    {
        //jsonを読み込む
        path = Application.dataPath +"/crackdata.json";
        jsonText = File.ReadAllText(path); // ファイル内容を変数に格納

        CrackInfoObject cio = new CrackInfoObject();
        cio = JsonConvert.DeserializeObject<CrackInfoObject>(jsonText);


        //ひびの本数と同じ回数処理
        for (int i = 0; i < cio.crackInfos.Count; i++)
        {
            //ひびの座標と同じ回数処理
            for (int j = 0; j < cio.crackInfos[i].crackPoint.Count - 1; j++)
            {
                //球生成
                leftSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                rightSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                //端の球の大きさを変更
                leftSphere.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
                rightSphere.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);

                //端の球の位置
                float leftSphereX = cio.crackInfos[i].crackPoint[j].x;
                float leftSphereY = cio.crackInfos[i].crackPoint[j].y;
                float leftSphereZ = cio.crackInfos[i].crackPoint[j].z;

                float rightSphereX = cio.crackInfos[i].crackPoint[j + 1].x;
                float rightSphereY = cio.crackInfos[i].crackPoint[j + 1].y;
                float rightSphereZ = cio.crackInfos[i].crackPoint[j + 1].z;


                leftSphere.transform.position = new Vector3(leftSphereX, leftSphereY, leftSphereZ);
                rightSphere.transform.position = new Vector3(rightSphereX, rightSphereY, rightSphereZ);

                //ひびのオブジェクト生成
                InstantiateCylinder(cylinderPrefab, leftSphere.transform.position, rightSphere.transform.position);


                //emptyobjectのCracksの子要素としてひびのオブジェクトを生成
                emptyCrack = GameObject.Find("Cracks");

                leftSphere.transform.parent = emptyCrack.transform;
                rightSphere.transform.parent = emptyCrack.transform;
                cylinder.transform.parent = emptyCrack.transform;
            }
            

        }

        //emptyobjectのCracksのprefab化と出力
        GameObject CrackObjs = GameObject.Find("Cracks");
        //var prefab = PrefabUtility.CreatePrefab("Assets/crackobjs.prefab", CrackObjs);
        var prefab = PrefabUtility.SaveAsPrefabAsset(CrackObjs, "Assets/CrackPrefabFile.prefab");

    }




    private void InstantiateCylinder(Transform cylinderPrefab, Vector3 beginPoint, Vector3 endPoint)
    {
        cylinder = Instantiate<GameObject>(cylinderPrefab.gameObject, Vector3.zero, Quaternion.identity);
        UpdateCylinderPosition(cylinder, beginPoint, endPoint);
    }

    private void UpdateCylinderPosition(GameObject cylinder, Vector3 beginPoint, Vector3 endPoint)
    {
        Vector3 offset = endPoint - beginPoint;
        Vector3 position = beginPoint + (offset / 2.0f);

        cylinder.transform.position = position;
        cylinder.transform.LookAt(beginPoint);
        Vector3 localScale = cylinder.transform.localScale;

        //円柱の大きさ変更
        localScale.x = 0.02f;
        localScale.y = 0.02f;
        localScale.z = (endPoint - beginPoint).magnitude;

        cylinder.transform.localScale = localScale;
    }

    //jsonのデシリアライズ用のデータクラス
    [Serializable]
    [JsonObject("CrackPoint")]
    public class CrackPoint
    {
        public float x;
        public float y;
        public float z;
    }

    [Serializable]
    [JsonObject("CrackInfo")]

    public class CrackInfo
    {
        public double CrackWidth;
        public double CrackLength;
        public List<CrackPoint> crackPoint;
    }

    [Serializable]
    [JsonObject("CrackInfoObject")]
    public class CrackInfoObject
    {
        public List<CrackInfo> crackInfos;
    }



}
