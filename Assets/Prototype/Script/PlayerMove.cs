using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private CharacterMoveController moveController;

    void Start()
    {
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