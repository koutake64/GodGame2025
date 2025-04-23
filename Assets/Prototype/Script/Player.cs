using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField, Header("行動クールタイム(秒)")] private float moveCooldown;
    [SerializeField, Header("プレイヤー生成位置")] private Vector2Int startPos;
    [SerializeField, Header("FieldDataManagerを設定")] private FieldDataManager fieldManager;

    private Vector2Int currentPos;
    private CharacterMoveController moveController;

    void Start()
    {
        currentPos = startPos;
        transform.position = fieldManager.GetInfo(currentPos).obj.transform.position;
        moveController = GetComponent<CharacterMoveController>();
    }

    void Update()
    {

            Vector2Int direction = Vector2Int.zero;

            if (Input.GetKey(KeyCode.W)) moveController.AddPosY(1);
            else if (Input.GetKey(KeyCode.S)) moveController.AddPosY(-1);
            else if (Input.GetKey(KeyCode.D)) moveController.AddPosX(1);
            else if (Input.GetKey(KeyCode.A)) moveController.AddPosX(-1);

    }
}