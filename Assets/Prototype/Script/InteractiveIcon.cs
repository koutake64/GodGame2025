using UnityEngine;

public class InteractiveIcon : MonoBehaviour
{
    Vector3 Offset;
    [SerializeField,Header("カメラ動かせるアイコン")] private GameObject obj;


    private _FieldDataManager fieldData;
    private TimeManager timeManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fieldData = GameObject.Find("Field").GetComponent<_FieldDataManager>();
        if(!fieldData)
        {
            Debug.LogError(
                "Script:InteractiveIcon.cs \n" +
                "fieldDataがnullです"
            );
        }
        timeManager = GameObject.Find("Canvas").GetComponent<TimeManager>();
        if (!timeManager)
        {
            Debug.LogError(
                "Script:InteractiveIcon.cs \n" +
                "TimeManagerがnullです"
            );
        }

        // UI非表示
        obj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // 昼、夕方以外は終了
        if (timeManager.GetCurState() == CommonSE_Proto.E_TIMEOFDAY.morning || timeManager.GetCurState() == CommonSE_Proto.E_TIMEOFDAY.night)
        {
            return;
        }
        bool showUI = false;

        // カメラ用
        var cameraPosList = fieldData.GetStatePos(_FieldDataManager.E_FIELDSTATE.surveillanceCamera);
        var cameraObjList = fieldData.GetGameObjectList(_FieldDataManager.E_FIELDSTATE.surveillanceCamera);
        var playerPos = fieldData.GetStatePos(_FieldDataManager.E_FIELDSTATE.butler);
        var player = playerPos[0];
        for (int i = 0; i < cameraPosList.Count; i++)
        {
            var cameraPos = cameraPosList[i];
            var cameraObj = cameraObjList[i];

            Vector2Int toPlayer = new Vector2Int(player.x - cameraPos.x, player.y - cameraPos.y);

            Vector2 cameraForwardVec = new Vector2(cameraObj.transform.forward.x, cameraObj.transform.forward.z).normalized;
            Vector2Int cameraFoward = new Vector2Int(
                Mathf.RoundToInt(cameraForwardVec.x),
                Mathf.RoundToInt(cameraForwardVec.y)
                );
            int dot = toPlayer.x * cameraFoward.x + toPlayer.y * cameraFoward.y;

            if(dot == 0 && toPlayer.magnitude == 1)
            {  
                showUI = true;
            }
        }

        // ライト用
        var lightPosList = fieldData.GetStatePos(_FieldDataManager.E_FIELDSTATE.light);
        var lightObjList = fieldData.GetGameObjectList(_FieldDataManager.E_FIELDSTATE.light);
        for(int i= 0;i<lightPosList.Count;i++)
        {
            var lightPos = lightPosList[i];
            var lightObj = lightObjList[i];

            Vector2Int toPlayer = new Vector2Int(player.x - lightPos.x, player.y - lightPos.y);
            Vector2 lightFowardVec = new Vector2(lightObj.transform.forward.x, lightObj.transform.forward.z).normalized;
            Vector2Int vlightFoward = new Vector2Int(
                Mathf.RoundToInt(lightFowardVec.x),
                Mathf.RoundToInt(lightFowardVec.y)
                );
            int dot = toPlayer.x * vlightFoward.x + toPlayer.y * vlightFoward.y;

            if(dot == 0 && toPlayer.magnitude == 1)
            {
                showUI = true;
            }
        }

        obj.SetActive(showUI);
    }
}
