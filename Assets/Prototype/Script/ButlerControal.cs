using UnityEngine;

public class ButlerControal : MonoBehaviour
{
    private Vector2Int initPos;        // 初期位置
    private TimeManager timeManager;    // タイムマネージャー


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

    void Update()
    {

        // 昼になったら初期位置に戻る
        if (timeManager.GetCurState() == CommonSE_Proto.E_TIMEOFDAY.afternoon && timeManager.IsChangeState())
        {
            transform.position = new Vector3(initPos.x, 0.0f, initPos.y);
            transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
        }
    }

    public void SetInitPos(Vector2Int pos)
    {
        initPos = pos;
    }
}
