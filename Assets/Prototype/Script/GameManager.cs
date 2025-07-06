using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = false;             // カーソル
        Application.targetFrameRate = 60;   // FPS
        QualitySettings.vSyncCount = 0;     // Sync(モニター依存にならないようにする処理
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            Cursor.visible = true;
        }
        if (Input.GetMouseButton(0))
        {
            Cursor.visible = false;
        }
    }
}
