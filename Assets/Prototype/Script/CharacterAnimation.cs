using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private Animator animator;
    private CharacterMoveController moveController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        animator = GetComponent<Animator>();
        moveController = GetComponent<CharacterMoveController>();
    }

    private void Update()
    {
        if (moveController.GetIsMove())
        {
            Moving();
        }
        else
        {
            StopMoving();
        }
    }

    /// <summary>
    /// 移動アニメーションを開始
    /// </summary>
    public void Moving()
    {
        animator.SetBool("Move", true);
    }

    /// <summary>
    /// 移動アニメーションを停止
    /// アイドル状態に戻る
    /// </summary>
    public void StopMoving()
    {
        animator.SetBool("Move", false);
    }

    /// <summary>
    /// お嬢様が警備員につかまったときのアニメーション
    /// </summary>
    public void GameOver()
    {
        animator.SetTrigger("GameOver");
    }
}
