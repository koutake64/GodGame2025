using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float grabDistance = 3f; // íÕÇﬂÇÈãóó£
    [SerializeField] Transform holdPoint;

    private GameObject heldObject = null;
    private Rigidbody heldRb = null;
    private Collider heldCol = null;

    private void Start()
    {
        if (holdPoint == null)
        {
            GameObject holdObj = new GameObject("HoldPoint");
            holdObj.transform.SetParent(transform);
            holdObj.transform.localPosition = new Vector3(0, 1, 1.5f);
            holdPoint = holdObj.transform;
        }
    }

    private void Update()
    {
        // à⁄ìÆèàóù
        if (Input.GetKey(KeyCode.W))
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

        // Ç¬Ç©Ç›èàóù
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (heldObject == null)
            {
                TryGrabObject();
            }
            else
            {
                ReleaseObject();
            }
        }

        if (heldObject != null)
        {
            heldObject.transform.position = holdPoint.position;
        }
    }


    void TryGrabObject()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, grabDistance))
        {
            if (hit.collider.CompareTag("playerObject"))
            {
                heldObject = hit.collider.gameObject;
                heldRb = heldObject.GetComponent<Rigidbody>();
                heldCol = heldObject.GetComponent<Collider>();

                if (heldRb != null) heldRb.isKinematic = true;
                if (heldCol != null) heldCol.enabled = false;
            }
        }
    }

    void ReleaseObject()
    {
        if (heldObject != null)
        {
            if (heldRb != null) heldRb.isKinematic = false;
            if (heldCol != null) heldCol.enabled = true;

            heldObject = null;
            heldRb = null;
            heldCol = null;
        }
    }
}
