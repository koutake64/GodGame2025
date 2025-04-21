using UnityEngine;

public class PrincessAction : MonoBehaviour
{
    public enum E_ACTIONSTATE
    {
        none,
        move,

        _count
    }

    private E_ACTIONSTATE state;
    private Vector3 targetPos;

    private void FixedUpdate()
    {
        switch(state)
        {
            case E_ACTIONSTATE.none:
                DecideTargetPos();
                break;
            case E_ACTIONSTATE.move:
                //Move();
                break;
        }
    }

    private void DecideTargetPos()
    {

    }

    //private void Move()
    //{

    //}

}
