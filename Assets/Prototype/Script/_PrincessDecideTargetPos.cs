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
    private TimeManager timeMng;

    private int currentKey = -1;

    List<int> keyList = new List<int>();
    List<int> shadowKeyList = new List<int>();

    private void Start()
    {
        cmController = GetComponent<CharacterMoveController>();

        fdMng = GameObject.Find("Field").GetComponentInChildren<_FieldDataManager>();
        if (fdMng == null)
        {
            Debug.Log("owari");
        }
        timeMng = GameObject.Find("Canvas").GetComponentInChildren<TimeManager>();
        if (timeMng == null)
        {
            Debug.Log("owari");
        }

        if (StageNummber.Get() == 0)
        {
            keyList.Add(5);
        }

    }

    private void Update()
    {
        switch(timeMng.GetCurState())
        {
            case CommonSE_Proto.E_TIMEOFDAY.morning:
                MorningUpdate();
                break;
            case CommonSE_Proto.E_TIMEOFDAY.noon:

                break;
            case CommonSE_Proto.E_TIMEOFDAY.afternoon:

                break;
            case CommonSE_Proto.E_TIMEOFDAY.night:
                NightUpdate();
                break;
        }
    }

    private void MorningUpdate()
    {
        // 仮
        SetSearchRange();
        if (!cmController.GetAutoMove())
        {
            DecideTargetPos();
            cmController.StartAutoMove(nextTargetPos);
            //Debug.Log(
            //    "nextTargetPos : " + nextTargetPos
            //    );
        }
        fdMng.ChangeColor();
    }

    private void NightUpdate()
    {
        SetSearchRange();
        if (!cmController.GetAutoMove())
        {
            DecideTargetPos();
            cmController.StartAutoMove(nextTargetPos);
            //Debug.Log(
            //    "nextTargetPos : " + nextTargetPos
            //    );
        }
    }

    private void SetSearchRange()
    {
        searchRangePosList.Clear();

        List<Vector2Int> goalList = fdMng.GetStatePos(_FieldDataManager.E_FIELDSTATE.goal);
        if (goalList.Count <= 0) return;

        Vector2Int goalPos = goalList[0];
        Vector2Int princessPos = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        Vector2Int direction = goalPos - princessPos;

        // x, y方向の向きを決定（+1 or -1）
        int dirX = direction.x >= 0 ? 1 : -1;
        int dirY = direction.y >= 0 ? 1 : -1;

        for (int y = 0; y < searchRange; ++y)
        {
            for (int x = 0; x < searchRange; ++x)
            {
                Vector2Int offset = new Vector2Int(x * dirX, y * dirY);
                Vector2Int p = princessPos + offset;

                // 範囲チェック（任意：マップ外アクセス防止）
                if (p.x >= 0 && p.y >= 0)
                {
                    searchRangePosList.Add(p);
                }
            }
        }
    }


    private void DecideTargetPos()
    {

        // ゴール座標をセット
        Vector2Int goalPos = new Vector2Int();
        if (fdMng.GetStatePos(_FieldDataManager.E_FIELDSTATE.goal).Count > 0)
        {
            goalPos = fdMng.GetStatePos(_FieldDataManager.E_FIELDSTATE.goal)[0];
        }

        // 柱座標取得
        List<Vector2Int> pillarPos = fdMng.GetStatePos(_FieldDataManager.E_FIELDSTATE.pillar);

        // 壁座標取得
        List<Vector2Int> wallPos = fdMng.GetStatePos(_FieldDataManager.E_FIELDSTATE.wall);

        // 影の座標取得
        List<Vector2Int> shadowPos = fdMng.GetStatePos(_FieldDataManager.E_FIELDSTATE.shadow);
        Dictionary<int, Vector2Int> shadowCandidatePos = new Dictionary<int, Vector2Int>();

        // 展示台座標取得
        List<Vector2Int> exhibitionStandPos = fdMng.GetStatePos(_FieldDataManager.E_FIELDSTATE.exhibitionStand);

        // 自身の座標
        Vector2Int princessPos = new Vector2Int((int)transform.position.x, (int)transform.position.z);

        // Dictionaryで配列を確保
        Dictionary<int, List<_FieldDataManager.S_FIELDINFO>> alignmentGroups = new Dictionary<int, List<_FieldDataManager.S_FIELDINFO>>();

        // ターゲット候補の座標リストにゴール座標を追加
        //List<Vector2Int> candidatePosDic = new List<Vector2Int>();
        Dictionary<int, List<Vector2Int>> candidatePosDic = new Dictionary<int, List<Vector2Int>>();
        List<Vector2Int> goalPosList = new List<Vector2Int>();
        goalPosList.Add(goalPos);
        candidatePosDic.Add(0, goalPosList);
        int cnt = 1;

        // 影とキーを登録
        for (int i = 0; i < shadowPos.Count; ++i)
        {
            shadowCandidatePos.Add(cnt, shadowPos[i]);
            cnt++;
        }

        cnt = 1;

        // 柱の周囲4箇所を移動候補リストに追加
        foreach (var pillar in pillarPos)
        {
            List<Vector2Int> targetList = new List<Vector2Int>();

            if (fdMng.GetIsThrough(pillar + Vector2Int.up))
            {
                targetList.Add(pillar + Vector2Int.up);
            }
            if (fdMng.GetIsThrough(pillar + Vector2Int.right))
            {
                targetList.Add(pillar + Vector2Int.right);
            }
            if (fdMng.GetIsThrough(pillar + Vector2Int.down))
            {
                targetList.Add(pillar + Vector2Int.down);
            }
            if (fdMng.GetIsThrough(pillar + Vector2Int.left))
            {
                targetList.Add(pillar + Vector2Int.left);
            }

            if(targetList.Count == 0)
            {
                continue;
            }

            candidatePosDic.Add(cnt, targetList);

            cnt++;
        }

        // 最適なターゲット座標を算出するための距離
        int minDistToPrincess = int.MaxValue;
        int minDistToGoal = int.MaxValue;
        Vector2Int bestTarget = princessPos;
        int key = 0;

        // 範囲内に影があれば優先して移動する
        foreach(var list in shadowCandidatePos)
        {
            if (shadowKeyList.Contains(list.Key))
            {
                continue;
            }

            for (int i = 0; i < searchRangePosList.Count; ++i)
            {
                if (list.Value == searchRangePosList[i])
                {
                    prevTargetPos = nextTargetPos;
                    nextTargetPos = list.Value;
                    shadowKeyList.Add(list.Key);
                    return;
                }
            }

        }

        // 移動候補リストの中でプリンセスとゴールとの距離を計算しターゲットを決定
        foreach (var list in candidatePosDic)
        {
            if(keyList.Contains(list.Key))
            {
                continue;
            }

            foreach (var pos in list.Value)
            {
                
                if (pos == princessPos || list.Key == currentKey)
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
                    key = list.Key;
                }
            }
        }

        currentKey = key;
        keyList.Add(currentKey);

        // 過去座標・ターゲット座標更新
        prevTargetPos = nextTargetPos;
        nextTargetPos = bestTarget;

        Debug.Log(
            "key : " + currentKey
            );
    }

    public List<Vector2Int> GetRange()
    {
        return searchRangePosList;
    }

    public Vector2Int GetNextPos()
    {
        return nextTargetPos;
    }

}
