using System.Collections.Generic;
using UnityEngine;

public class MonitoringRangeGanerate : MonoBehaviour
{
    [SerializeField, Header("生成する監視範囲オブジェクト(正面)")] private GameObject monitoringFrontObj;
    [SerializeField, Header("生成する監視範囲オブジェクト(斜め)")] private GameObject monitoringDiagonalObj;

    List<Vector2Int> monitoring = new List<Vector2Int>();
    List<GameObject> monitoringObject = new List<GameObject>();

    //=== メソッド ===
    private void Start()
    {
        if (monitoringFrontObj == null)
        { Debug.LogError("監視範囲オブジェクト(正面)[_shadowFrontObj]が見つかりません"); }
        if (monitoringDiagonalObj == null)
        { Debug.LogError("監視範囲オブジェクト(斜め)[_shadowDiagonalObj]が見つかりません"); }
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

        // 基準となるオブジェクト座標
        Vector2Int Standard = monitoring[0];
        Vector3 standardPos = new Vector3(Standard.x, 0.0f, Standard.y);

        // ループを回す際にいないので削除
        monitoring.Remove(Standard);

        // オブジェクト生成用
        GameObject obj;

        // 各々カメラの方に向かせる
        foreach (var shadowPos in monitoring)
        {
            // 生成座標
            Vector3 createPos = new Vector3(shadowPos.x, 0.0f, shadowPos.y);
            
            // 方向を算出
            Vector2Int different = Standard - shadowPos;

            // オブジェクト生成
            if (different.x == 0 || different.y == 0)
            {
                obj = Instantiate(monitoringFrontObj, createPos, Quaternion.identity);
            }
            else
            {
                obj = Instantiate(monitoringDiagonalObj, createPos, Quaternion.identity);
            }

            // 方向を変更
            obj.transform.LookAt(standardPos);

            // 生成済みオブジェクト配列に追加
            monitoringObject.Add(obj);
        }
    }
}