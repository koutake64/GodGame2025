using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class StatusEdit : MonoBehaviour
{
    [SerializeField] private StatusManager statusMng;
    [SerializeField] private TMP_Text posText;
    [SerializeField] private TMP_Dropdown stateDropdown;
    [SerializeField] private TMP_Dropdown dirDropdown;
    [SerializeField] private TMP_Text idText;
    [SerializeField] private TMP_Text routeXText;
    [SerializeField] private TMP_Text routeYText;

    private Vector2Int pos;
    private _FieldDataManager.E_FIELDSTATE state;
    private int id;
    private CommonSE_Proto.E_DIRECTION dir;

    private void Start()
    {
        state = _FieldDataManager.E_FIELDSTATE.none;
        id = -1;
        dir = CommonSE_Proto.E_DIRECTION.down;

        stateDropdown.onValueChanged.AddListener(OnDropdownStateChanged);
        dirDropdown.onValueChanged.AddListener(OnDropdownDirChanged);

    }

    public void OnDropdownStateChanged(int index)
    {
        switch (index)
        {
            case 0:
                state = _FieldDataManager.E_FIELDSTATE.none;
                break;
            case 1:
                state = _FieldDataManager.E_FIELDSTATE.start;
                break;
            case 2:
                state = _FieldDataManager.E_FIELDSTATE.goal;
                break;
            case 3:
                state = _FieldDataManager.E_FIELDSTATE.pillar;
                break;
            case 4:
                state = _FieldDataManager.E_FIELDSTATE.wall;
                break;
            case 5:
                state = _FieldDataManager.E_FIELDSTATE.exhibitionStand;
                break;
            case 6:
                state = _FieldDataManager.E_FIELDSTATE.surveillanceCamera;
                break;
            case 7:
                state = _FieldDataManager.E_FIELDSTATE.light;
                break;
            case 8:
                state = _FieldDataManager.E_FIELDSTATE.securityGuard_N;
                break;
            case 9:
                state = _FieldDataManager.E_FIELDSTATE.talk;
                break;
        }

    }

    public void OnDropdownDirChanged(int index)
    {
        switch (index)
        {
            case 0:
                dir = CommonSE_Proto.E_DIRECTION.up;
                break;
            case 1:
                dir = CommonSE_Proto.E_DIRECTION.right;
                break;
            case 2:
                dir = CommonSE_Proto.E_DIRECTION.down;
                break;
            case 3:
                dir = CommonSE_Proto.E_DIRECTION.left;
                break;
        }

    }

    public void OnClickComplete()
    {
        Status s = statusMng.GetStatus(pos.x, pos.y);
        s.SetState(state, TMPToID(idText), dir);

        if (s.GetState() == _FieldDataManager.E_FIELDSTATE.securityGuard_N)
        {
            List<Vector2Int> route = new List<Vector2Int>();
            Vector2Int r = new Vector2Int(TMPToINT(routeXText), TMPToINT(routeYText));
            route.Add(r);
            s.SetRoute(route);
        }

        this.gameObject.SetActive(false);
    }

    public void SetPos(int x, int y)
    {
        pos.x = x; 
        pos.y = y;

        posText.text = "ˆÊ’u  x : " + x + ", y : " + y;

    }

    private int TMPToID(TMP_Text tmp)
    {
        int value = 0;
        string str = "";
        char[] c = tmp.text.ToCharArray();
        for (int i = 0; i < c.Length - 1; ++i)
        {
            str += c[i];
        }

        if (str.Length > 0)
        {
            value = int.Parse(str);
        }
        else
        {
            value = -1;
        }

        return value;

    }

    private int TMPToINT(TMP_Text tmp)
    {
        int value = 0;
        string str = "";
        char[] c = tmp.text.ToCharArray();
        for (int i = 0; i < c.Length - 1; ++i)
        {
            str += c[i];
        }

        value = int.Parse(str);

        return value;

    }

}
