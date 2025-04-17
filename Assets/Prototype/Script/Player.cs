using UnityEngine;

public class Player : MonoBehaviour
{
	[SerializeField] float speed;

	private void Update()
	{
		if(Input.GetKey(KeyCode.W))
		{
			transform.position += speed * transform.forward * Time.deltaTime;
		}

		if (Input.GetKey(KeyCode.S))
		{
			transform.position -= speed * transform.forward * Time.deltaTime;
		}

		if (Input.GetKey(KeyCode.D))
		{
			transform.position += speed * transform.right * Time.deltaTime;
		}

		if (Input.GetKey(KeyCode.A))
		{
			transform.position -= speed * transform.right * Time.deltaTime;
		}
	}
}
