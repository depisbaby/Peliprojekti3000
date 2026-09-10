using UnityEngine;

public class CameraRig : MonoBehaviour
{
    public GameObject cameraObject;
    public GameObject cameraHolder;

    float zoom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        float edgeMargin = 50f;

        Vector3 mousePos = Input.mousePosition;

        if (mousePos.x <= edgeMargin)
        {
            transform.position = transform.position + -transform.right * Time.deltaTime * 10;
        }
        else if (mousePos.x >= Screen.width - edgeMargin)
        {
            transform.position = transform.position + transform.right * Time.deltaTime * 10;
        }

        if (mousePos.y <= edgeMargin)
        {
            transform.position = transform.position + -transform.forward * Time.deltaTime * 10;
        }
        else if (mousePos.y >= Screen.height - edgeMargin)
        {
            transform.position = transform.position + transform.forward * Time.deltaTime * 10;
        }

        // Mouse wheel
        float scroll = Input.mouseScrollDelta.y;
        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.rotation = transform.rotation * Quaternion.Euler(0, Time.deltaTime * scroll * 1000, 0);
        }
        else
        {
            zoom = Mathf.Clamp(zoom + Time.deltaTime * -scroll * 100, 5, 20);
        }

        //move camera
        cameraObject.transform.position = Vector3.Lerp(cameraObject.transform.position, cameraHolder.transform.position, Time.deltaTime * 5);
        cameraObject.transform.rotation = Quaternion.Lerp(cameraObject.transform.rotation, cameraHolder.transform.rotation, Time.deltaTime * 5);
        cameraHolder.transform.localPosition = new Vector3(0, zoom, -zoom);

    }
}
