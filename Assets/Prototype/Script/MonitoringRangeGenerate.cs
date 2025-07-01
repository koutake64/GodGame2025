// Assets/Scripts/MonitoringRangeGanerate.cs
using UnityEngine;
using System.Collections.Generic;

public class MonitoringRangeGanerate : MonoBehaviour
{
    [SerializeField, Header("生成する監視範囲オブジェクト(正面)")]
    private GameObject monitoringFrontObj;

    [SerializeField, Header("生成する監視範囲オブジェクト(斜め)")]
    private GameObject monitoringDiagonalObj;

    private List<Vector2Int> monitoring = new List<Vector2Int>();
    private readonly List<GameObject> monitoringObject = new List<GameObject>();

    private void Start()
    {
        if (monitoringFrontObj == null)
            Debug.LogError("監視範囲オブジェクト(正面)[monitoringFrontObj]が設定されていません");

        if (monitoringDiagonalObj == null)
            Debug.LogError("監視範囲オブジェクト(斜め)[monitoringDiagonalObj]が設定されていません");
    }

    /// <summary>
    /// 単一カメラ用：カメラ位置＋視界内タイルから監視オブジェクトを生成
    /// </summary>
    public void SetArrayMonitoring(List<Vector2Int> searchedTiles)
    {
        if (searchedTiles == null || searchedTiles.Count < 2) return;

        monitoring = searchedTiles;
        CreateMonitoringObjects();
    }

    /// <summary>
    /// 監視範囲オブジェクトを生成・再利用する
    /// </summary>
    private void CreateMonitoringObjects()
    {
        int usedCount = 0;

        Vector2Int standard = monitoring[0];
        Vector3 standardPos = new Vector3(standard.x, 0f, standard.y);

        for (int i = 1; i < monitoring.Count; i++)
        {
            Vector2Int shadowPos = monitoring[i];
            Vector3 createPos = new Vector3(shadowPos.x, 0f, shadowPos.y);
            Vector2Int diff = standard - shadowPos;

            GameObject obj;

            if (usedCount < monitoringObject.Count)
            {
                obj = monitoringObject[usedCount];
                obj.transform.position = createPos;
            }
            else
            {
                var prefab = (diff.x == 0 || diff.y == 0) ? monitoringFrontObj : monitoringDiagonalObj;
                obj = Instantiate(prefab, createPos, Quaternion.identity);
                monitoringObject.Add(obj);
            }

             usedCount++;
        }

        // 余分なオブジェクトを退避
        for (int i = usedCount; i < monitoringObject.Count; i++)
        {
            monitoringObject[i].transform.position = new Vector3(9999f, -100f, 9999f);
        }
    }
}