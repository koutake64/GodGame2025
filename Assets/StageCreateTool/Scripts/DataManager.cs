using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    [SerializeField] private StageInfo stageInfo;
    [SerializeField] private StatusManager statusMng;

    private StreamWriter editData;

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("save...");
            Vector2Int size = stageInfo.GetSize();
            Vector3 time = stageInfo.GetSwitchTime();

            editData.WriteLine(new string(size.x.ToString() + "," + size.y.ToString()));
            editData.WriteLine(new string(time.x.ToString() + "," + time.y.ToString() + "," + time.z.ToString()));

            Status[,] data = statusMng.GetAllStatus();
            
            for (int y = 0; y < size.y; ++y)
            {
                for (int x = 0; x < size.x; ++x)
                {
                    editData.WriteLine(
                        new string(
                            data[x, y].GetPos().x + "," + 
                            data[x, y].GetPos().y + "," + 
                            data[x, y].GetState() + "," + 
                            data[x, y].GetID() + "," + 
                            data[x, y].GetDir() + ","
                            ));

                }

            }

            editData.Close();
            editData.Flush();

        }
    }

    public void CreateNewData()
    {
        editData = new StreamWriter("Assets/StageCreateTool/Text/test.txt",false);

    }

    

}
