using UnityEngine;
using UnityEngine.UI;

public class SelectFrame : MonoBehaviour
{
    [SerializeField, Header("カーソルスピード")] private float MoveSpeed = 8f;
    private Vector3 targetPos;
    private StageSelectManager selMng;
    private int currentButtonIndex = 0; // 現在のボタンインデックス


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
    }

    // Update is called once per frame
    void Update()
    {
        InputMove();
        this.transform.position = Vector3.Lerp(this.transform.position, targetPos, Time.deltaTime * MoveSpeed);
    }

    void InputMove()
    {
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
       
    }
}