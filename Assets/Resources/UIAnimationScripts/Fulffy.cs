using UnityEngine;

public class Fluffy : MonoBehaviour
{
    [SerializeField,Header("—h‚ê‚Ì‹­‚³")]
    private float strength; // —h‚ê‚Ì‹­‚³
    [SerializeField,Header("—h‚ê‚Ì‘¬‚³")]
    private float frequency; // —h‚ê‚Ì‘¬‚³

    private Vector3 startPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = this.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        float offset = Mathf.Sin(Time.time * frequency) * strength;
        this.transform.localPosition = startPos + new Vector3(0, offset, 0);
    }
}
