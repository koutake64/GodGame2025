using UnityEngine;
using System.Collections.Generic;

public class PrincessTalk : MonoBehaviour
{
    TextManager textMng;
    _FieldDataManager fieldMng;
    TimeManager timeMng;

    GameObject backgroundPanel;

    int talkCnt = 0;

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
        if (timeMng.GetCurState() != CommonSE_Proto.E_TIMEOFDAY.morning)
        {
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

                backgroundPanel.SetActive(true);
                textMng.StartTalk(talkCnt);

                Debug.Log("現在のトーク番号：" + talkCnt);

                talkCnt++;
            }
        }
    }
}
