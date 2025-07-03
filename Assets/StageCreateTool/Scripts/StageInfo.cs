using UnityEngine;

public class StageInfo : MonoBehaviour
{
    private Vector2Int size;
    private Vector3 switchTime;

    private void Start()
    {
        size = new Vector2Int();
        switchTime = new Vector3();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log(
                "*** StageInfo ***\n" +
                "@Size " + "x:" + size.x + " " + "y:" + size.y + "\n" +
                "@SwitchTime " + "noon:" + switchTime.x + " " + "afternoon:" + switchTime.y + " " + "night:" + switchTime.z + "\n"
                );
        }
    }

    public void SetSize(int width, int height)
    {
        size.x = width;
        size.y = height;
    }

    public void SetSwitchTime(float noon, float afternoon, float night)
    {
        switchTime.x = noon;
        switchTime.y = afternoon;
        switchTime.z = night;
    }

    public Vector2Int GetSize()
    {
        return size;
    }

    public Vector3 GetSwitchTime()
    {
        return switchTime;
    }

}