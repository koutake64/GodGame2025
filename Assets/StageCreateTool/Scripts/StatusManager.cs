using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    [SerializeField] private GameObject button;
    [SerializeField] private StageInfo stageInfo;
    
    private Status[,] statuses;

    private void Update()
    {
        Vector3 move = new Vector3();

        if (Input.GetKey(KeyCode.W))
        {
            move.y += 100;
        }
        if (Input.GetKey(KeyCode.A))
        {
            move.x -= 100;
        }
        if (Input.GetKey(KeyCode.S))
        {
            move.y -= 100;
        }
        if (Input.GetKey(KeyCode.D))
        {
            move.x += 100;
        }

        transform.position += move * Time.deltaTime;

    }

    public void GenerateButton()
    {
        Vector2Int size = stageInfo.GetSize();
        statuses = new Status[size.x, size.y];
        for (int y = 0; y < size.y; ++y)
        {
            for (int x = 0; x < size.x; ++x)
            {
                GameObject obj = Instantiate(
                    button,
                    new Vector3(x * 100.0f, y * 100.0f, 0.0f),
                    Quaternion.identity
                    );

                obj.transform.SetParent(GameObject.Find("StatusManager").transform);
                Status s = obj.GetComponent<Status>();
                if (s)
                {
                    s.SetPos(new Vector2Int(x, y));
                    s.SetPanel(GameObject.Find("StatusEditPanel"));
                }
                statuses[x, y] = s;

            }
        }

        GameObject statusEditPanel = GameObject.Find("StatusEditPanel");
        statusEditPanel.SetActive(false);

    }
    
    public Status GetStatus(int x, int y)
    {
        return statuses[x, y];
    }

    public Status[,] GetAllStatus()
    {
        return statuses;
    }

}
