using UnityEngine;

public class ButlerControal : MonoBehaviour
{
    private Vector2Int initPos;        // 初期位置
    private TimeManager timeManager;    // タイムマネージャー
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeManager = GameObject.Find("Canvas").GetComponent<TimeManager>();
        if (!timeManager)
        {
            Debug.LogError(
               "Script:CharacterMoveController.cs \n" +
               "timeManagerがnullです"
            );
        }
    }

    // Update is called once per frame
    void Update()
    {

        // 昼になったら初期位置に戻る
        if (timeManager.GetCurState() == CommonSE_Proto.E_TIMEOFDAY.afternoon && timeManager.IsChangeState() ||
            timeManager.GetCurState() == CommonSE_Proto.E_TIMEOFDAY.noon && timeManager.IsChangeState())
        {
            transform.position = new Vector3(initPos.x, 0.0f, initPos.y);
        }
    }
    public void SetInitPos(Vector2Int pos)
    {
        initPos = pos;
    }
}
