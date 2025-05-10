using UnityEngine;

public class GameSystem : MonoBehaviour
{
    [Header("キャラクターの移動速度")]
    [SerializeField] private float moveSpeed;

    [Header("キャラクターの回転速度")]
    [SerializeField] private float rotateSpeed;

    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    public float GetCharacterMoveSpeed()
    {
        return moveSpeed;
    }
    public float GetCharacterRotateSpeed()
    {
        return rotateSpeed;
    }


}