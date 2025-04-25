using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.ShaderGraph.Serialization;

public class RouteSearch : MonoBehaviour
{
    private FieldDataManager    fieldData;  // FieldDataManager
    private Vector2Int          fieldSize;  // フィールドサイズ

    // TODO プロト終わったら消す
    List<Vector2Int>            routed;     // デバッグ用通った道

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fieldData = GameObject.Find("Field").GetComponentInChildren<FieldDataManager>();
        if (!fieldData)
        {
            Debug.LogError(
               "Script:SecurityController.cs \n" +
               "fieldDataがnullです"
            );
        }

        // フィールドサイズ取得
        GameSystem system = GameObject.Find("GameSystem").GetComponent<GameSystem>();
        if (!system)
        {
            Debug.LogError(
               "Script:SecurityController.cs \n" +
               "systemがnullです"
            );
        }

        // フィールドサイズ取得
        fieldSize = system.GetFieldSize();
    }

    public List<Vector2Int> MoveRouteSearch(Vector2Int start, Vector2Int goal)
    {
        // 経路探索キュー
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        // どこから来たか記録
        Dictionary<Vector2Int, Vector2Int> routeFrom = new Dictionary<Vector2Int, Vector2Int>();
        // 探索済みマス
        HashSet<Vector2Int> done = new HashSet<Vector2Int>();

        // スタート位置
        queue.Enqueue(start);
        done.Add(start);

        // 方向
        Vector2Int[] directions = {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        // ルート探索
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            // 探索終了したか
            if(current == goal)
            {
                return ReconstructRoute(routeFrom, start, goal);
            }

            // 各方向のマスを確認
            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;

                // 通れるかチェック
                if(IsWalkable(next) && !done.Contains(next))
                {
                    queue.Enqueue(next);
                    done.Add(next);
                    routeFrom[next] = current;
                }
            }
        }

        // ルートが見つからなかった
        return new List<Vector2Int>();
    }

    private List<Vector2Int> ReconstructRoute(Dictionary<Vector2Int, Vector2Int> routeFrom, Vector2Int start, Vector2Int goal)
    {
        List<Vector2Int> route = new List<Vector2Int>();
        Vector2Int current = goal;

        while (current != start)
        {
            route.Add(current);
            current = routeFrom[current];
        }

        route.Reverse();

        // TODO プロト終わったら消す
        Color color = Color.red;
        foreach (var i in route)
        {
            fieldData.SetColor(i, color);
        }
        routed = route;

        return route;
    }
    private bool IsWalkable(Vector2Int pos)
    {
        // 範囲外チェック
        if (pos.x < 0 || pos.x >= fieldSize.x || pos.y < 0 || pos.y >= fieldSize.y) return false;
       
        // 通れるか確認
        var info = fieldData.GetInfo(pos);
        return info.state == FieldDataManager.E_FIELDSTATE.none;
    }

    // TODO プロト終わったら消す
    public void ResetTileColor()
    {
        foreach (var i in routed)
        {
            int num = i.x + i.y;

            if(num % 2 == 0)
            {
                fieldData.SetColor(i, Color.gray);
            }
            else
            {
                fieldData.SetColor(i, Color.white);
            }
        }
    }
}