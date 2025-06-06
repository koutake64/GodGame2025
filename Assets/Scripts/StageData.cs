using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StageData",menuName = "StageData")]
public class StageData : ScriptableObject
{
    [System.Serializable]
    private struct S_STAGEINFO
    {
        [Header("位置"), SerializeField] public Vector2Int pos;
        [Header("向き"), SerializeField] public CommonSE_Proto.E_DIRECTION dir;
        [Header("ID"), SerializeField] public int id;
        [Header("種類"), SerializeField] public _FieldDataManager.E_FIELDSTATE state;
    }

    [Header("大きさ"), SerializeField] private Vector2Int size;
    [Header("データ"), SerializeField] private List<S_STAGEINFO> data;

}
