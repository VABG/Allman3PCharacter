using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    PlayerController p;
    [SerializeField] Transform parentTransform;

    float rotation = 0;

    float defaultDistance;
    Vector3 cameraPositionFarthest;
    Vector3 cameraPositionNearest;

    // Start is called before the first frame update
    void Start()
    {
        p = FindObjectOfType<PlayerController>();

        Ray r = new Ray(transform.position, transform.forward);
        RaycastHit rayHit;
        Physics.Raycast(r, out rayHit);


        defaultDistance = rayHit.distance - .5f;
        cameraPositionFarthest = transform.localPosition;
        cameraPositionNearest = transform.localPosition + transform.forward * defaultDistance;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        rotation = -Input.GetAxis("Mouse Y");
        Ray r = new Ray(transform.position, transform.forward);
        RaycastHit rayHit;
        if (Physics.Raycast(r,  out rayHit, defaultDistance))
        {
            float distance = rayHit.distance;
            distance /= defaultDistance;
            transform.localPosition = Vector3.Lerp(cameraPositionNearest, cameraPositionFarthest, distance);
        }
    }

    private void LateUpdate()
    {
        parentTransform.position = Vector3.Lerp(parentTransform.position, p.transform.position, 6 * Time.deltaTime);
        Vector3 pRot = parentTransform.rotation.eulerAngles;
        Vector3 playerRot = p.transform.rotation.eulerAngles;
        parentTransform.rotation = Quaternion.Euler(pRot.x + rotation, playerRot.y, 0);
    }
}
