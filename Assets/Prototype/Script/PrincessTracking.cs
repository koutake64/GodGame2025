using UnityEngine;

public class PrincessTracking : MonoBehaviour
{
    private CharacterMoveController playerController;
    private CharacterMoveController princessController;

    private Vector2Int lastPrincessPrevPos;

    void Start()
    {
        playerController = GetComponent<CharacterMoveController>();
        if (playerController == null)
        {
            Debug.LogError("PrincessTracking: プレイヤーに CharacterMoveController がありません");
            enabled = false;
            return;
        }

        GameObject princessObj = GameObject.FindWithTag("Princess");
        if (princessObj == null)
        {
            Debug.LogError("PrincessTracking: タグ 'Princess' のオブジェクトが見つかりません");
            enabled = false;
            return;
        }

        princessController = princessObj.GetComponent<CharacterMoveController>();
        if (princessController == null)
        {
            Debug.LogError("PrincessTracking: プリンセスに CharacterMoveController が見つかりません");
            enabled = false;
            return;
        }

        lastPrincessPrevPos = princessController.GetPrevPos();
    }

    void Update()
    {
        if (princessController == null || playerController == null) return;

        Vector2Int currentPrevPos = princessController.GetPrevPos();

        // プリンセスが動いたら、その一つ前の位置に追従
        if (currentPrevPos != lastPrincessPrevPos)
        {
            lastPrincessPrevPos = currentPrevPos;
            playerController.StartAutoMove(currentPrevPos); // プレイヤーがプリンセスを追う
        }
    }
}
