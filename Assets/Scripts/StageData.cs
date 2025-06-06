using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StageData",menuName = "StageData")]
public class StageData : ScriptableObject
{
    [System.Serializable]
    private struct S_STAGEINFO
    {
        public Vector2Int pos;
        public _FieldDataManager.E_FIELDSTATE state;
    }

    [SerializeField] private List<S_STAGEINFO> data;
}
