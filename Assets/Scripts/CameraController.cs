using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;
    Vector3 startPosition;
    Quaternion startRotation;
    [SerializeField] float smoothSpeed = 0.125f;
    [SerializeField] Vector3 offset;

    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("creature"))
                {
                    target = hit.collider.transform;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            target = null;
            transform.position = startPosition;
            transform.rotation = startRotation;
        }

        if (target)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            transform.LookAt(target);
        }
    }
}
