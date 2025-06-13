using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
        {
            Cursor.visible = true;
        }
    }
}
