using System.Collections.Generic;
using UnityEngine;

public class MonitoringRangeGanerate : MonoBehaviour
{
    [SerializeField, Header("生成する監視範囲オブジェクト(正面)")] private GameObject monitoringFrontObj;

    List<Vector2Int> monitoring = new List<Vector2Int>();
    List<GameObject> monitoringObject = new List<GameObject>();

    //=== メソッド ===
    private void Start()
    {
        if (monitoringFrontObj == null)
        { Debug.LogError("監視範囲オブジェクト[_shadowFrontObj]が見つかりません"); }
    }

    public void SetArrayShadow(List<Vector2Int> array)
    {
        monitoring = array;

        CreateMonitoringObject();
    }

    private void CreateMonitoringObject()
    {
        foreach (var item in monitoringObject)
        {
            Destroy(item);
        }

        // オブジェクト生成用
        GameObject obj;

        // 各々カメラの方に向かせる
        foreach (var shadowPos in monitoring)
        {
            // 生成座標
            Vector3 createPos = new Vector3(shadowPos.x, 0.0f, shadowPos.y);
            
            obj = Instantiate(monitoringFrontObj, createPos, Quaternion.identity);
            
            // 生成済みオブジェクト配列に追加
            monitoringObject.Add(obj);
        }
    }
}