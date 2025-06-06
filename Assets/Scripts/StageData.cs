using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StageData",menuName = "StageData")]
public class StageData : ScriptableObject
{
    [System.Serializable]
    private struct S_STAGEINFO
    {
        [Header("à íu"), SerializeField] public Vector2Int pos;
        [Header("å¸Ç´"), SerializeField] public CommonSE_Proto.E_DIRECTION dir;
        [Header("ID"), SerializeField] public int id;
        [Header("âΩÇ≈Ç∑Ç©ÅH"), SerializeField] public _FieldDataManager.E_FIELDSTATE state;
    }

    [SerializeField] private List<S_STAGEINFO> data;
}
