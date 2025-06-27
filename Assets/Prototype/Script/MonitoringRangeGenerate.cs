using System.Collections.Generic;
using UnityEngine;

public class MonitoringRangeGanerate : MonoBehaviour
{
    [SerializeField, Header("生成する監視範囲オブジェクト(正面)")] private GameObject monitoringFrontObj;
    [SerializeField, Header("生成する監視範囲オブジェクト(斜め)")] private GameObject monitoringDiagonalObj;

    List<List<Vector2Int>> monitoring = new List<List<Vector2Int>>();
    List<GameObject> monitoringObject = new List<GameObject>();

    //=== メソッド ===
    private void Start()
    {
        if (monitoringFrontObj == null)
        { Debug.LogError("監視範囲オブジェクト(正面)[_shadowFrontObj]が見つかりません"); }
        if (monitoringDiagonalObj == null)
        { Debug.LogError("監視範囲オブジェクト(斜め)[_shadowDiagonalObj]が見つかりません"); }
    }

    public void SetArrayShadow(List<List<Vector2Int>> array)
    {
        monitoring = array;

        CreateMonitoringObject();
        Debug.Log("SetArrayShadow");
    }

    private void CreateMonitoringObject()
    {
        foreach (var item in monitoringObject)
        {
            Destroy(item);
        }

        foreach (var item in monitoring)
        {
            // 基準となるオブジェクト座標
            Vector2Int Standard = item[0];
            Vector3 standardPos = new Vector3(Standard.x, 0.0f, Standard.y);

            // ループを回す際にいないので削除
            item.Remove(Standard);

            // 各々カメラの方に向かせる
            foreach (var shadowPos in item)
            {
                // 生成座標
                Vector3 createPos = new Vector3(shadowPos.x, 0.0f, shadowPos.y);
                
                // 方向を算出
                Vector2Int different = Standard - shadowPos;

                // オブジェクト生成
                GameObject obj;
                if (different.x == 0 || different.y == 0)
                {
                    Debug.Log("Front");
                    obj = Instantiate(monitoringFrontObj, createPos, Quaternion.identity);
                }
                else
                {
                    Debug.Log("Diagonal");
                    obj = Instantiate(monitoringDiagonalObj, createPos, Quaternion.identity);
                }

                // 方向を変更
                obj.transform.LookAt(standardPos);

                // 生成済みオブジェクト配列に追加
                monitoringObject.Add(obj);
            }
        }
    }
    /// <summary>
    /// カメラ位置を含む索敵マスリストを元に監視範囲オブジェクトを生成
    /// </summary>
    public void SetArrayMonitoring(List<Vector2Int> searchedTiles)
    {
        if (searchedTiles == null || searchedTiles.Count == 0) return;

        // 既存のオブジェクトを削除
        foreach (var obj in monitoringObject)
        {
            Destroy(obj);
        }
        monitoringObject.Clear();

        Vector2Int standard = searchedTiles[0];
        Vector3 standardPos = new Vector3(standard.x, 0f, standard.y);

        for (int i = 1; i < searchedTiles.Count; i++)
        {
            Vector2Int shadowPos = searchedTiles[i];
            Vector3 createPos = new Vector3(shadowPos.x, 0f, shadowPos.y);

            Vector2Int different = standard - shadowPos;

            GameObject obj;
            if (different.x == 0 || different.y == 0)
            {
                obj = Instantiate(monitoringFrontObj, createPos, Quaternion.identity);
            }
            else
            {
                obj = Instantiate(monitoringDiagonalObj, createPos, Quaternion.identity);
            }

            obj.transform.LookAt(standardPos);
            monitoringObject.Add(obj);
        }
    }
}