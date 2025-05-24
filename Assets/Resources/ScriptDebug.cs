using UnityEngine;

/// <summary>
/// 作ったスクリプトを試す用
/// </summary>
public class ScriptDebug : MonoBehaviour
{
    private TextManager txtMng;
    public GameObject backGroundPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backGroundPanel.SetActive(true);
        txtMng = GameObject.Find("TextTyper").GetComponent<TextManager>();
        if (!txtMng)
            Debug.Log("TextManagerが見つかりません");
        backGroundPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            backGroundPanel.SetActive(true);
            txtMng.StartTalk(1);
        }

        if(Input.GetKeyDown(KeyCode.K))
        {
            backGroundPanel.SetActive(true);
            txtMng.StartTalk(0);
        }
    }
}
