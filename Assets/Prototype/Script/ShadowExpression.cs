using UnityEngine;
using System.Collections.Generic;


public class ShadowExpression : MonoBehaviour
{
    //=== シリアライズフィールド ===
    [SerializeField, Header("生成する影オブジェクト(正面)")] private GameObject _shadowFrontObj;
    [SerializeField, Header("生成する影オブジェクト(斜め)")] private GameObject _shadowDiagonalObj;

    List<List<Vector2Int>> shadow = new List<List<Vector2Int>>();
    List<GameObject> shadowObject = new List<GameObject>();

    //=== メソッド ===
    private void Start()
    {
        if (_shadowFrontObj == null)
        { Debug.LogError("影オブジェクト(正面)[_shadowFrontObj]が見つかりません"); }
        if (_shadowDiagonalObj == null)
        { Debug.LogError("影オブジェクト(斜め)[_shadowDiagonalObj]が見つかりません"); }
    }

    public void SetArrayShadow(List<List<Vector2Int>> array)
    {
        shadow = array;

        CreateShadowObject();
    }

    private void CreateShadowObject()
    {
        foreach (var item in shadowObject)
        {
            Destroy(item);
        }

        foreach (var item in shadow)
        {
            // 座標が2つ以上ないなら次のループへ
            if(item.Count <= 1)
            {
                continue;
            }

            Vector2Int Standard = item[0];
            item.Remove(Standard);
            Vector3 standardPos = new Vector3(Standard.x, 0.0f, Standard.y);

            foreach (var shadowPos in item)
            {
                Vector3 createPos = new Vector3(shadowPos.x, 0.0f, shadowPos.y);
                Vector2Int different = Standard - shadowPos;

                GameObject obj;
                // Instantiateで生成
                if (different.x == 0 || different.y == 0)
                {
                    obj = Instantiate(_shadowFrontObj,createPos,Quaternion.identity);
                }
                else
                {
                    obj =Instantiate(_shadowDiagonalObj,createPos,Quaternion.identity);
                }

                obj.transform.LookAt(standardPos);

                shadowObject.Add(obj);
            }
        }
    }
}