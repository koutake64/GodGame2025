using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    Vector3 Offset;
    [SerializeField] private GameObject obj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // UI”ñ•\Ž¦
        obj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
