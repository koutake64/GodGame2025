using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SelectFrame : MonoBehaviour
{
    [SerializeField, Header("カーソルスピード")] private float MoveSpeed = 8f;
    [SerializeField, Header("カーソルの動きの有無")] private bool isLerp = true;
    private Vector3 targetPos;
    private StageSelectManager selMng;
    private int currentButtonIndex = 0; // 現在のボタンインデックス

    // InputSystem
    [SerializeField] private InputActionAsset inputActions;
    private InputAction upAction;
    private InputAction downAction;
    private InputAction leftAction;
    private InputAction rightAction;
    private InputAction nextAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selMng = GameObject.Find("StageSelectManager").GetComponent<StageSelectManager>();
        if (!selMng)
        {
            Debug.LogError("Script:SelectFrame.cs \n" +
             "StageSelectManagerがnullです");
        }

        targetPos = selMng.GetButtonPos(currentButtonIndex);

        // Actionの取得
        var map = inputActions.FindActionMap("Menu");
        upAction = map.FindAction("UP");
        downAction = map.FindAction("Down");
        leftAction = map.FindAction("Left");
        rightAction = map.FindAction("Right");
        nextAction = map.FindAction("Next");

        // 有効化
        upAction.Enable();
        downAction.Enable();
        leftAction.Enable();
        rightAction.Enable();
        nextAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        InputMove();
        DecideButton();
        if(isLerp)
            this.transform.position = Vector3.Lerp(this.transform.position, targetPos, Time.deltaTime * MoveSpeed);
        else
            this.transform.position = targetPos; // 直接位置を設定する場合
    }

    /// <summary>
    /// カーソル移動
    /// </summary>
    void InputMove()
    {
        /*
        // 上下ボタン
        if (Input.GetKeyDown(KeyCode.S))
        {
            currentButtonIndex++;
            if (currentButtonIndex >= 5)
            {
                currentButtonIndex = 0; // 最後のボタンを超えたら最初に戻る
            }
            targetPos = selMng.GetButtonPos(currentButtonIndex);
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            currentButtonIndex--;
            if (currentButtonIndex < 0)
            {
                currentButtonIndex = 4; // 最初のボタンを下回ったら最後に戻る
            }
            targetPos = selMng.GetButtonPos(currentButtonIndex);
        }

        // 左右ボタン
        if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.A))
        {
            if( currentButtonIndex == 0 || 
                currentButtonIndex == 1 || 
                currentButtonIndex == 2 )
            {
                currentButtonIndex = 3;
                targetPos = selMng.GetButtonPos(currentButtonIndex);
            }
        }
        */
        if (downAction.WasPressedThisFrame())
        {
            currentButtonIndex = (currentButtonIndex + 1) % 5;
            targetPos = selMng.GetButtonPos(currentButtonIndex);
        }
        else if (upAction.WasPressedThisFrame())
        {
            currentButtonIndex = (currentButtonIndex + 4) % 5;
            targetPos = selMng.GetButtonPos(currentButtonIndex);
        }

        if (rightAction.WasPressedThisFrame() || leftAction.WasPressedThisFrame())
        {
            if (currentButtonIndex == 0 || currentButtonIndex == 1 || currentButtonIndex == 2)
            {
                currentButtonIndex = 3;
                targetPos = selMng.GetButtonPos(currentButtonIndex);
            }
        }
    }

    /// <summary>
    /// ボタンの決定
    /// </summary>
    void DecideButton()
    {
        /*
        if(Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(selMng.ActionButton(currentButtonIndex));
        }
        */
        if (nextAction.WasPressedThisFrame())
        {
            StartCoroutine(selMng.ActionButton(currentButtonIndex));
        }
    }
}