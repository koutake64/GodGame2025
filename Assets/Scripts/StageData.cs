using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StageData",menuName = "StageData")]
public class StageData : ScriptableObject
{
    [System.Serializable]
    public struct S_STAGEINFO
    {
        [Header("位置"), SerializeField] public Vector2Int pos;
        [Header("向き"), SerializeField] public CommonSE_Proto.E_DIRECTION dir;
        [Header("ID"), SerializeField] public int id;
        [Header("種類"), SerializeField] public _FieldDataManager.E_FIELDSTATE state;
    }

    [System.Serializable]
    public struct S_ROUTEINFO
    {
        [Header("ID<動かす警備員と同じID>"), SerializeField] public int id;
        [Header("ルート"), SerializeField] public List<Vector2Int> route;
    }

    [Header("ステージナンバー"), SerializeField] private int stageNum;
    [Header("大きさ"), SerializeField] private Vector2Int size;
    [Header("データ"), SerializeField] private List<S_STAGEINFO> data;
    [Header("警備員ルート"), SerializeField] private List<S_ROUTEINFO> route;

    public int GetStageNum()
    {
        return stageNum;
    }

    public List<S_STAGEINFO> GetData()
    {
        return data;
    }
    
    public Vector2Int GetSize()
    {
        return size;
    }

    public List<S_ROUTEINFO> GetRoute()
    {
        return route;
    }

}
