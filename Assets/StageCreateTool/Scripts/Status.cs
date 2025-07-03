using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Status : MonoBehaviour
{
    private GameObject statusEditPanel;
    private Vector2Int pos;
    private _FieldDataManager.E_FIELDSTATE state;
    private _FieldDataManager.E_FIELDSTATE prevState;
    private int id;
    private int prevId;
    private CommonSE_Proto.E_DIRECTION dir;
    private CommonSE_Proto.E_DIRECTION prevDir;
    private List<Vector2Int> route;
    [SerializeField] private TMP_Text tmp;
    [SerializeField] private Image img;

    private void Start()
    {
        state = _FieldDataManager.E_FIELDSTATE.none;
        prevState = _FieldDataManager.E_FIELDSTATE.none;
        id = -1;
        prevId = -1;
        dir = CommonSE_Proto.E_DIRECTION.down;
        prevDir = CommonSE_Proto.E_DIRECTION.down;
        route = new List<Vector2Int>();

    }

    private void LateUpdate()
    {
        if (state != prevState || id != prevId || dir != prevDir)
        {
            ChangeTMP();
            ChangeImageColor();
            prevState = state;
            prevId = id;
            prevDir = dir;
        }
    }

    private void ChangeTMP()
    {
        string dirStr = "";

        switch (dir)
        {
            case CommonSE_Proto.E_DIRECTION.up:
                dirStr = "Å™";
                break;
            case CommonSE_Proto.E_DIRECTION.right:
                dirStr = "Å®";
                break;
            case CommonSE_Proto.E_DIRECTION.down:
                dirStr = "Å´";
                break;
            case CommonSE_Proto.E_DIRECTION.left:
                dirStr = "Å©";
                break;
        }

        switch (state)
        {
            case _FieldDataManager.E_FIELDSTATE.none:
                tmp.text = "Å[";
                break;
            case _FieldDataManager.E_FIELDSTATE.start:
                tmp.text = "S";
                break;
            case _FieldDataManager.E_FIELDSTATE.goal:
                tmp.text = "G";
                break;
            case _FieldDataManager.E_FIELDSTATE.pillar:
                tmp.text = "íå" + ":" + id.ToString();
                break;
            case _FieldDataManager.E_FIELDSTATE.wall:
                tmp.text = "ï«" + ":" + id.ToString();
                break;
            case _FieldDataManager.E_FIELDSTATE.exhibitionStand:
                tmp.text = "ë‰" + ":" + id.ToString();
                break;
            case _FieldDataManager.E_FIELDSTATE.surveillanceCamera:
                tmp.text = "äƒ" + ":" + dirStr;
                break;
            case _FieldDataManager.E_FIELDSTATE.light:
                tmp.text = "åı" + ":" + dirStr;
                break;
            case _FieldDataManager.E_FIELDSTATE.securityGuard_N:
                tmp.text = "åx" + ":" + id.ToString() + ":" + dirStr;
                break;
        }
    }

    private void ChangeImageColor()
    {
        switch (state)
        {
            case _FieldDataManager.E_FIELDSTATE.none:
                img.color = Color.white;
                break;
            case _FieldDataManager.E_FIELDSTATE.start:
                img.color = Color.blue;
                break;
            case _FieldDataManager.E_FIELDSTATE.goal:
                img.color = Color.red;
                break;
            case _FieldDataManager.E_FIELDSTATE.pillar:
                img.color = Color.gray;
                break;
            case _FieldDataManager.E_FIELDSTATE.wall:
                img.color = Color.gray;
                break;
            case _FieldDataManager.E_FIELDSTATE.exhibitionStand:
                img.color = Color.gray;
                break;
            case _FieldDataManager.E_FIELDSTATE.surveillanceCamera:
                img.color = Color.cyan;
                break;
            case _FieldDataManager.E_FIELDSTATE.light:
                img.color = Color.yellow;
                break;
            case _FieldDataManager.E_FIELDSTATE.securityGuard_N:
                img.color = Color.green;
                break;
        }
    }

    public void SetPos(Vector2Int _pos)
    {
        pos = _pos;
    }

    public Vector2Int GetPos()
    {
        return pos;
    }

    public void SetPanel(GameObject panel)
    {
        statusEditPanel = panel;
    }

    public void SetState(_FieldDataManager.E_FIELDSTATE _state, int _id = -1, CommonSE_Proto.E_DIRECTION _dir = CommonSE_Proto.E_DIRECTION.down)
    {
        state = _state;
        id = _id;
        dir = _dir;
    }

    public _FieldDataManager.E_FIELDSTATE GetState()
    {
        return state;
    }

    public int GetID()
    {
        return id;
    }

    public CommonSE_Proto.E_DIRECTION GetDir()
    {
        return dir;
    }

    public void SetRoute(List<Vector2Int> _route)
    {
        route = _route;
    }

    public void OnClick()
    {
        statusEditPanel.SetActive(true);
        StatusEdit e = statusEditPanel.GetComponent<StatusEdit>();
        e.SetPos(pos.x, pos.y);
    }

}
