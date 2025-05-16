using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
	private CharacterMoveController moveController;
	private TimeManager timeManager;

	private Vector2 moveInput; // “ü—Í’l
	private float inputCooldown = 0.2f; // “ü—ÍŠÔŠu
	private float inputTimer = 0f;

	public void OnMove(InputAction.CallbackContext context)
	{
		moveInput = context.ReadValue<Vector2>();
	}

	void Start()
	{
		moveController = GetComponent<CharacterMoveController>();
		timeManager = FindFirstObjectByType<TimeManager>();
	}

	void Update()
	{
		if (timeManager.CurrentState != CommonSE_Proto.E_TIMEOFDAY.noon
			&& timeManager.CurrentState != CommonSE_Proto.E_TIMEOFDAY.afternoon)
			return;

		inputTimer -= Time.deltaTime;
		if (inputTimer > 0) return;

		// —Dæ‡ˆÊFã‰º¨¶‰E
		if (moveInput.y > 0.5f)
		{
			moveController.AddPosY(1);
			inputTimer = inputCooldown;
		}
		else if (moveInput.y < -0.5f)
		{
			moveController.AddPosY(-1);
			inputTimer = inputCooldown;
		}
		else if (moveInput.x > 0.5f)
		{
			moveController.AddPosX(1);
			inputTimer = inputCooldown;
		}
		else if (moveInput.x < -0.5f)
		{
			moveController.AddPosX(-1);
			inputTimer = inputCooldown;
		}

	}

}
