using UnityEngine;
using System.Collections.Generic;

public class SecurityController : MonoBehaviour
{
    [Header("移動ターゲットリスト")]
    [SerializeField] private List<Vector2Int> targetArray = new List<Vector2Int>();

    private CharacterMoveController moveController; // CharacterMoveController
    private bool                    isEndMovement;  // 目標座標までの移動終了したか
    private int                     currentIndex;   // 配列の何番目か
    private int                     addNum;         // 加算する値

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveController = GetComponent<CharacterMoveController>();
        if (!moveController)
        {
            Debug.LogError(
               "Script:SecurityController.cs \n" +
               "moveControllerがnullです"
            );
        }

        isEndMovement = true;
        currentIndex = 0;
        addNum = 1;
    }

    // Update is called once per frame
    void Update()
    {
        // 移動が終了していたら
        if (isEndMovement && targetArray.Count > 0)
        {
            // 移動先を指定
            moveController.StartAutoMove(moveController.GetCurrentPos(), targetArray[currentIndex]);

            // 移動終了フラグを下げる
            isEndMovement = false;
        }        
    }

    public void EndMovement()
    {
        // 巡回するように配列番号を更新
        currentIndex = (currentIndex + addNum) % targetArray.Count;

        if(currentIndex < 0)
        {
            currentIndex = targetArray.Count - 1;
        }

        isEndMovement = true;
    }

    public void InverseArray()
    {
        addNum *= -1;
    }

    private void ForwardMonitoring()
    {

    }
}