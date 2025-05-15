using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private CharacterMoveController moveController;
	private TimeManager timeManager;

	void Start()
    {
        moveController = GetComponent<CharacterMoveController>();
        timeManager = FindFirstObjectByType<TimeManager>();
	}


    void Update()
    {
        if (timeManager.CurrentState == CommonSE_Proto.E_TIMEOFDAY.noon)
        {
            if (Input.GetKey(KeyCode.W)) moveController.AddPosY(1);
            else if (Input.GetKey(KeyCode.S)) moveController.AddPosY(-1);
            else if (Input.GetKey(KeyCode.D)) moveController.AddPosX(1);
            else if (Input.GetKey(KeyCode.A)) moveController.AddPosX(-1);
        }

        //if(timeManager.CurrentState == CommonSE_Proto.E_TIMEOFDAY.night)
        //{
        //    this.gameObject.SetActive(false);
        //}
    }
}