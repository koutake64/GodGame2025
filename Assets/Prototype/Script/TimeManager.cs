using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
	[SerializeField, Header("’‹ŠJŽnŽž(•b)")] float noonTime;
	[SerializeField, Header("–éŠJŽnŽž(•b)")] float nightTime;
    [SerializeField] Text timeText;
	[SerializeField] Text levelText;
	
	private float time;

	private void Update()
	{
		time += Time.deltaTime;
		
		timeText.text = time.ToString("0" + "•b");

		if(time >= noonTime)
		{
			levelText.text = time.ToString("’‹");
		}
		
		if(time >= nightTime)
		{
			levelText.text = time.ToString("–é");
		}
	}
}
