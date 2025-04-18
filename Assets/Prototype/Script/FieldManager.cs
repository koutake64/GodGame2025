using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public class FieldManager : MonoBehaviour
{
    // フィールドの状態
    public enum FieldState{
        NONE,
    };

    // マス目の情報
    public struct FieldInfo{
        public Vector3 pos; // 座標
        public FieldState state;  // 何があるか
    };


    [SerializeField, Header("分割数")] Vector2 frequency = new Vector2(8f, 8f);
    [SerializeField, Header("1辺の長さ")] Vector3 sideLength;
    [SerializeField, Header("オブジェクト")] GameObject field;

    MeshRenderer meshRenderer;

    FieldInfo[,] fieldInfo;

    void Start()
    {
        fieldInfo = new FieldInfo[(int)frequency.x * 2, (int)frequency.y * 2];
        meshRenderer = GetComponent<MeshRenderer>();

        meshRenderer.material.SetVector("_CheckerFrequency", new Vector4(frequency.x, frequency.y, 0.0f, 0.0f));

        this.transform.localScale = new Vector3(sideLength.x * frequency.x * 2, this.transform.localScale.y, sideLength.z * frequency.y * 2);

        for(int y = 0; y < frequency.x * 2; ++y)
        {
            for(int x = 0; x < frequency.x * 2; ++x)
            {
                Vector3 pos = new Vector3(
                    (transform.position.x - this.transform.localScale.x / 2) + (sideLength.x / 2) + 1.0f * x,
                    this.transform.position.y,
                    (transform.position.z - this.transform.localScale.z / 2) + (sideLength.y / 2) + 1.0f * y
                    );

                fieldInfo[y, x].pos = pos;
                fieldInfo[y, x].state = FieldState.NONE;
            }
        }
    }

    public FieldInfo GetFieldInfo(Vector2 num)
    {
        return fieldInfo[(int)num.y, (int)num.x];
    }
}