using UnityEngine;
using System.Collections.Generic;

public class RouteSearch : MonoBehaviour
{
    private _FieldDataManager   fieldData;  // FieldDataManager
    private Vector2Int          fieldSize;  // フィールドサイズ
    private SecurityController  security;   // SecurityController

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fieldData = GameObject.Find("Field").GetComponent<_FieldDataManager>();
        if (!fieldData)
        {
            Debug.LogError(
               "Script:RouteSearch.cs \n" +
               "fieldDataがnullです"
            );
        }

        // 警備員スクリプト取得
        security = GetComponent<SecurityController>();

        // フィールドサイズ取得
        fieldSize = fieldData.GetFieldSize();
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

        // 目標座標が移動不可な場合に可能な限り近づける
        Vector2Int closest = start;
        float minDist = Vector2Int.Distance(start, goal);

        // ルート探索
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            float dist = Vector2Int.Distance(current, goal);
            if(dist < minDist)
            {
                minDist = dist;
                closest = current;
            }

            // 探索終了したか
            if(current == goal)
            {
                return ReconstructRoute(routeFrom, start, goal, false);
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

        // 目標まで到達できなかった
        if(security)
        {
            security.InverseArray();
        }

        // 近づけるところまでのルート
        return ReconstructRoute(routeFrom, start, closest, true);
    }

    private List<Vector2Int> ReconstructRoute(Dictionary<Vector2Int, Vector2Int> routeFrom, Vector2Int start, Vector2Int goal, bool isInverse)
    {
        List<Vector2Int> route = new List<Vector2Int>();
        Vector2Int current = goal;

        while (current != start)
        {
            route.Add(current);
            current = routeFrom[current];
        }

        // リストの要素を反対にする
        route.Reverse();

        if (isInverse)
        {
            List<Vector2Int> routeKeep = new List<Vector2Int>(route);
            routeKeep.Reverse();
            route.AddRange(routeKeep);
        }

        return route;
    }
    private bool IsWalkable(Vector2Int pos)
    {
        // 範囲外チェック
        if (pos.x < 0 || pos.x >= fieldSize.x || pos.y < 0 || pos.y >= fieldSize.y)
        {
            return false;
        }

        // 通れるか確認
        if (!fieldData.GetIsThrough(pos))
        {
            return false;
        }

        return true;
    }
}