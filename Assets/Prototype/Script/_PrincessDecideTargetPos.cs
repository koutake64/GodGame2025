using UnityEngine;
using System.Collections.Generic;

public class _PrincessDecideTargetPos : MonoBehaviour
{
    private const int searchRange = 4;

    private Vector2Int nextTargetPos;
    private Vector2Int prevTargetPos;

    private List<Vector2Int> searchRangePosList = new List<Vector2Int>();

    private _FieldDataManager fdMng;

    private Vector2Int prevEdgeTargetPos = new Vector2Int(int.MinValue, int.MinValue);

    private CharacterMoveController cmController;

    private void Start()
    {
        cmController = GetComponent<CharacterMoveController>();

        fdMng = GameObject.Find("Field").GetComponentInChildren<_FieldDataManager>();
        if (fdMng == null)
        {
            Debug.Log("owari");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            SetSearchRange();
            DecideTargetPos();
            cmController.StartAutoMove(nextTargetPos);
            Debug.Log(
                "nextTargetPos : " + nextTargetPos
                );
        }
    }

    private void SetSearchRange()
    {
        searchRangePosList.Clear();

        Vector2Int goalPos = fdMng.GetStatePos(_FieldDataManager.E_FIELDSTATE.goal)[0];
        Vector2Int princessPos = new Vector2Int((int)transform.position.x, (int)transform.position.z);

        Vector2Int direction = goalPos - princessPos;

        Vector2Int offset = new Vector2Int();
        offset.x = direction.x >= 0 ? (searchRange - 1) * -1 : 0;
        offset.y = direction.y >= 0 ? (searchRange - 1) * -1 : 0;

        Vector2Int start = princessPos + offset;

        for (int y = 0; y < searchRange; ++y)
        {
            for (int x = 0; x < searchRange; ++x)
            {
                Vector2Int p = start + new Vector2Int(x, y);
                searchRangePosList.Add(p);
            }
        }

    }

    private void DecideTargetPos()
    {
        // ゴール座標をセット
        Vector2Int goalPos = fdMng.GetStatePos(_FieldDataManager.E_FIELDSTATE.goal)[0];

        // 柱座標取得
        List<Vector2Int> pillarPos = fdMng.GetStatePos(_FieldDataManager.E_FIELDSTATE.pillar);

        // 壁座標取得
        List<Vector2Int> wallPos = fdMng.GetStatePos(_FieldDataManager.E_FIELDSTATE.wall);

        // 展示台座標取得
        List<Vector2Int> exhibitionStandPos = fdMng.GetStatePos(_FieldDataManager.E_FIELDSTATE.exhibitionStand);

        // 自身の座標
        Vector2Int princessPos = new Vector2Int((int)transform.position.x, (int)transform.position.z);

        // Dictionaryで配列を確保
        Dictionary<int, List<_FieldDataManager.S_FIELDINFO>> alignmentGroups = new Dictionary<int, List<_FieldDataManager.S_FIELDINFO>>();

        // IDごとにグループ化
        void GroupByAlignment(List<Vector2Int> positions)
        {
            foreach (var pos in positions)
            {
                var infoList = fdMng.GetInfoList(pos);
                foreach (var info in infoList)
                {
                    // IDが入っていなければ次へ
                    if (info.alignmentID == -1) continue;

                    // 一度もそのIDで配列確保されていなければIDの添字で配列確保
                    if (!alignmentGroups.ContainsKey(info.alignmentID))
                    {
                        alignmentGroups[info.alignmentID] = new List<_FieldDataManager.S_FIELDINFO>();
                    }

                    // IDの添字に情報追加
                    alignmentGroups[info.alignmentID].Add(info);
                }
            }
        }

        // IDごとに配列に格納
        GroupByAlignment(wallPos);
        GroupByAlignment(exhibitionStandPos);

        foreach (var kv in alignmentGroups)
        {
            var group = kv.Value;

            // グループ内の要素数が2未満の場合は次へ
            if (group.Count < 2) continue;

            // 方向が左右のどちらかならtrue
            bool isHorizontal = group[0].dir == CommonSE_Proto.E_DIRECTION.right || group[0].dir == CommonSE_Proto.E_DIRECTION.left;

            // 方向に応じてソート
            group.Sort((a, b) => isHorizontal ? a.pos.x.CompareTo(b.pos.x) : a.pos.y.CompareTo(b.pos.y));

            // グループの端の座標を取得
            List<Vector2Int> first = new List<Vector2Int>();
            if (fdMng.GetIsThrough(group[0].pos + Vector2Int.up))
                first.Add(group[0].pos + Vector2Int.up);
            if (fdMng.GetIsThrough(group[0].pos + Vector2Int.right))
                first.Add(group[0].pos + Vector2Int.right);
            if (fdMng.GetIsThrough(group[0].pos + Vector2Int.down))
                first.Add(group[0].pos + Vector2Int.down);
            if (fdMng.GetIsThrough(group[0].pos + Vector2Int.left))
                first.Add(group[0].pos + Vector2Int.left);

           List<Vector2Int> last = new List<Vector2Int>();
            if (fdMng.GetIsThrough(group[^1].pos + Vector2Int.up))
                first.Add(group[^1].pos + Vector2Int.up);
            if (fdMng.GetIsThrough(group[^1].pos + Vector2Int.right))
                first.Add(group[^1].pos + Vector2Int.right);
            if (fdMng.GetIsThrough(group[^1].pos + Vector2Int.down))
                first.Add(group[^1].pos + Vector2Int.down);
            if (fdMng.GetIsThrough(group[^1].pos + Vector2Int.left))
                first.Add(group[^1].pos + Vector2Int.left);

            for(int j = 0; j < first.Count; ++j)
            {
                for (int i = 0; i < last.Count; ++i)
                {
                    if (princessPos == first[j] && prevEdgeTargetPos != last[i])
                    {
                        if (j < 2)
                        {
                            prevTargetPos = nextTargetPos;
                            nextTargetPos = last[j];
                            prevEdgeTargetPos = first[j];
                        }
                        else
                        {
                            prevTargetPos = nextTargetPos;
                            nextTargetPos = last[j];
                            prevEdgeTargetPos = first[j];
                        }
                    }
                }
            }


            //if (princessPos == first && last != prevEdgeTargetPos)
            //{
            //    return;
            //}

            // 対象がグループの端にいて、過去座標ともう一方の端座標が違う場合、もう一方の端をターゲットに設定
            //if (princessPos == first && last != prevEdgeTargetPos)
            //{
            //    return;
            //}
            //else if (princessPos == last && first != prevEdgeTargetPos)
            //{
            //    prevTargetPos = nextTargetPos;
            //    nextTargetPos = first;
            //    prevEdgeTargetPos = last;
            //    return;
            //}
        }

        // ターゲット候補の座標リストにゴール座標を追加
        List<Vector2Int> candidatePosList = new List<Vector2Int>();
        candidatePosList.Add(goalPos);

        // 柱の周囲4箇所を移動候補リストに追加
        foreach (var pillar in pillarPos)
        {
            if (fdMng.GetIsThrough(pillar + Vector2Int.up))
            {
                candidatePosList.Add(pillar + Vector2Int.up);
            }
            if (fdMng.GetIsThrough(pillar + Vector2Int.right))
            {
                candidatePosList.Add(pillar + Vector2Int.right);
            }
            if (fdMng.GetIsThrough(pillar + Vector2Int.down))
            {
                candidatePosList.Add(pillar + Vector2Int.down);
            }
            if (fdMng.GetIsThrough(pillar + Vector2Int.left))
            {
                candidatePosList.Add(pillar + Vector2Int.left);
            }
        }

        void AddAroundWithAlignmentCheck(List<Vector2Int> baseList)
        {
            foreach (var pos in baseList)
            {
                var infoList = fdMng.GetInfoList(pos);
                foreach (var info in infoList)
                {
                    // IDがない場合は次へ
                    if (info.alignmentID == -1) continue;

                    // 指定座標の周囲4箇所を移動候補リストに追加
                    if (fdMng.GetIsThrough(info.pos + Vector2Int.up))
                    {
                        candidatePosList.Add(info.pos + Vector2Int.up);
                    }
                    if (fdMng.GetIsThrough(info.pos + Vector2Int.right))
                    {
                        candidatePosList.Add(info.pos + Vector2Int.right);
                    }
                    if (fdMng.GetIsThrough(info.pos + Vector2Int.down))
                    {
                        candidatePosList.Add(info.pos + Vector2Int.down);
                    }
                    if (fdMng.GetIsThrough(info.pos + Vector2Int.left))
                    {
                        candidatePosList.Add(info.pos + Vector2Int.left);
                    }
                }
            }
        }

        // 壁と展示台の周囲の座標を移動候補リストに追加
        AddAroundWithAlignmentCheck(wallPos);
        AddAroundWithAlignmentCheck(exhibitionStandPos);

        // 最適なターゲット座標を算出するための距離
        int minDistToPrincess = int.MaxValue;
        int minDistToGoal = int.MaxValue;
        Vector2Int bestTarget = princessPos;

        int cnt = 0;
        foreach (var pos in candidatePosList)
        {
            cnt++;
            Debug.Log(
                "座標 : " + pos + cnt + "個目"
                );
        }

        // 移動候補リストの中でプリンセスとゴールとの距離を計算しターゲットを決定
        foreach (var pos in candidatePosList)
        {
            if (pos == princessPos)
            {
                continue;
            }

            // プリンセスとの距離を計算
            int distToPrincess = Mathf.Abs(pos.x - princessPos.x) + Mathf.Abs(pos.y - princessPos.y);
            
            // ゴールとの距離を計算
            int distToGoal = Mathf.Abs(pos.x - goalPos.x) + Mathf.Abs(pos.y - goalPos.y);

            // プリンセスとの距離が一番近い
            if (distToPrincess < minDistToPrincess ||
            // 設定済みのプリンセスとの距離と同じかつゴールにさらに近い場合は更新
                (distToPrincess == minDistToPrincess && distToGoal < minDistToGoal))
            {
                minDistToPrincess = distToPrincess;
                minDistToGoal = distToGoal;
                bestTarget = pos;
            }
        }

        // 過去座標・ターゲット座標更新
        prevTargetPos = nextTargetPos;
        nextTargetPos = bestTarget;

        Debug.Log(
            "bestTarget : " + bestTarget
            );
    }
}
