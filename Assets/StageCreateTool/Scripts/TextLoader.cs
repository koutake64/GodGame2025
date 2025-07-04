using System.Collections.Generic;
using UnityEngine;

public class TextLoader : MonoBehaviour
{
    [SerializeField] private TextAsset stageTextFile;

    public int stageWidth;
    public int stageHeight;
    public int timeMorning;
    public int timeEvening;
    public int timeNight;

    public struct CellData
    {
        public int x, y;
        public _FieldDataManager.E_FIELDSTATE state;
        public int id;
        public CommonSE_Proto.E_DIRECTION dir;
        public Vector2Int route;
    }

    public List<CellData> cellList = new List<CellData>();

    void Awake()
    {
        if (stageTextFile != null)
        {
            LoadStage(stageTextFile.text);
        }
        else
        {
            Debug.Log("TextAsset が設定されていません");
        }
    }

    public void LoadStage(string text)
    {
        string[] lines = text.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        // ステージサイズ
        string[] size = lines[0].Split(',');
        stageWidth = int.Parse(size[0]);
        stageHeight = int.Parse(size[1]);

        // 昼～夜の時間
        string[] time = lines[1].Split(',');
        timeMorning = int.Parse(time[0]);
        timeEvening = int.Parse(time[1]);
        timeNight = int.Parse(time[2]);

        // マス情報
        cellList.Clear();
        for (int i = 2; i < lines.Length; i++)
        {
            string[] tokens = lines[i].Split(',');

            int x = int.Parse(tokens[0]);
            int y = int.Parse(tokens[1]);
            string stateStr = tokens[2];
            int id = int.Parse(tokens[3]);
            string dirStr = tokens[4];

            _FieldDataManager.E_FIELDSTATE state = ParseState(stateStr);
            CommonSE_Proto.E_DIRECTION direction = ParseDirection(dirStr);

            int routeX = 0;
            int routeY = 0;

            if (state == _FieldDataManager.E_FIELDSTATE.securityGuard_N)
            {
                routeX = int.Parse(tokens[5]);
                routeY = int.Parse(tokens[6]);
            }

            cellList.Add(new CellData
            {
                x = x,
                y = y,
                state = state,
                id = id,
                dir = direction,
                route = new Vector2Int(routeX, routeY)
            });
        }

        Debug.Log($"読み込み完了: サイズ({stageWidth},{stageHeight}) マス数:{cellList.Count}");
    }

    _FieldDataManager.E_FIELDSTATE ParseState(string str)
    {
        if (System.Enum.TryParse(str, out _FieldDataManager.E_FIELDSTATE result))
        {
            return result;
        }
        else
        {
            return _FieldDataManager.E_FIELDSTATE.none;
        }
    }

    CommonSE_Proto.E_DIRECTION ParseDirection(string str)
    {
        return System.Enum.TryParse(str, out CommonSE_Proto.E_DIRECTION result) ? result : CommonSE_Proto.E_DIRECTION.down;
    }
}
