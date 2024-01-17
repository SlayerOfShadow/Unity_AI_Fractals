using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;
    Vector3 startPosition;
    Quaternion startRotation;
    [SerializeField] float smoothSpeed = 0.05f;
    [SerializeField] Vector3 offset;
    bool onCreature = false;

    public List<Transform> cameraPositions = new List<Transform>();

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
                    onCreature = true;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            target = cameraPositions[0];
            onCreature = false;
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            target = cameraPositions[1];
            onCreature = false;
        } else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            target = cameraPositions[2];
            onCreature = false;
        } else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            target = cameraPositions[3];
            onCreature = false;
        }

        if (target)
        {
            Vector3 desiredPosition = target.position;
            if (onCreature) desiredPosition += offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            if (onCreature) transform.LookAt(target);
            else {
                Quaternion desiredRotation = target.rotation;
                Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, desiredRotation, smoothSpeed);
                transform.rotation = smoothedRotation;
            }
        }

        if (onCreature)
        {
            
        }
    }
}
