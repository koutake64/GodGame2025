using UnityEngine;

public class InteractiveIcon : MonoBehaviour
{
    Vector3 Offset;
    [SerializeField,Header("カメラ動かせるアイコン")] private GameObject obj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // UI非表示
        obj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
