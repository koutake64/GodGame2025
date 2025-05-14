using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UIを管理するクラス
/// </summary>
public class UIManager : MonoBehaviour
{
    public Animator animator;



    private bool useMemo;


    private bool currentFlag;
    private bool prevFlag;


    // Update is called once per frame
    void Update()
    {
        InputUpdate();
        UpdateAnimator();


        prevFlag = currentFlag;
    }



    void UpdateAnimator()
    {
        animator.SetBool("isUseMemo", useMemo);
    }


    void InputUpdate()
    {
        currentFlag = useMemo;


        // Tabキーでメモをポップアップする
        if (Input.GetKeyDown(KeyCode.Tab))
            useMemo = !useMemo;
    }
}
