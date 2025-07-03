using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject statusEditPanel;

    private void Start()
    {
        menuPanel.SetActive(true);
        statusEditPanel.SetActive(true);
    }

}
