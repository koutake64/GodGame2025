using UnityEngine;
using System.Collections.Generic;

public class PrincessTalk : MonoBehaviour
{
    private TextManager textMng;
    private _FieldDataManager fieldMng;
    private TimeManager timeMng;

    private GameObject backgroundPanel;

    private int talkCnt = 0;
    public bool isTalk = false;

    private void Awake()
    {
        backgroundPanel = GameObject.Find("BackgroundPanel");

        var obj = GameObject.Find("TextTyper");

        textMng = obj.GetComponent<TextManager>();
        if (!textMng)
        {
            Debug.Log(
            "=================================================================================naiyo");
        }
        fieldMng = GameObject.Find("Field").GetComponent<_FieldDataManager>();
        if (!fieldMng)
        {
            Debug.Log(
            "naiyo");
        }
        timeMng = GameObject.Find("Canvas").GetComponent<TimeManager>();
        if (!timeMng)
        {
            Debug.Log(
            "naiyo");
        }
    }

    private void Start()
    {
        backgroundPanel.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (timeMng.GetCurState() != CommonSE_Proto.E_TIMEOFDAY.morning || !isTalk)
        {
            CharacterMoveController cmc = GetComponent<CharacterMoveController>();
            cmc.ReStart();
        }

        if (timeMng.GetCurState() != CommonSE_Proto.E_TIMEOFDAY.morning || isTalk)
        {
            isTalk = textMng.talkFlg;
            return;
        }

        List<Vector2Int> list = fieldMng.GetStatePos(_FieldDataManager.E_FIELDSTATE.talk);

        if (list.Count <= 0)
        {
            return;
        }

        Vector2Int princessPos = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        for (int i = 0; i < list.Count; ++i)
        {
            if (princessPos == list[i])
            {
                CharacterMoveController cmc = GetComponent<CharacterMoveController>();
                cmc.Stop();

                fieldMng.RemoveInfo(princessPos, _FieldDataManager.E_FIELDSTATE.talk);

                backgroundPanel.SetActive(true);
                textMng.StartTalk(talkCnt,false);

                isTalk = true;
                talkCnt++;
            }
        }
    }
}
