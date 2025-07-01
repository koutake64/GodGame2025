using UnityEngine;

public class BillboardUI : MonoBehaviour
{

    // Update is called once per frame
    void LateUpdate()
    {
        if(Camera.main != null)
        {
           transform.forward = Camera.main.transform.forward;
        }
    }
}
