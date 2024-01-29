using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;
    Vector3 startPosition;
    Quaternion startRotation;
    [SerializeField] float smoothSpeed = 0.05f;
    [SerializeField] float rotationSpeed = 2.0f;
    [SerializeField] Vector3 offset;
    bool onCreature = false;
    MlAgent stats;
    Character character;
    [SerializeField] GameObject statsPanel;
    [SerializeField] Slider visionSlider;
    [SerializeField] Slider speedSlider;
    [SerializeField] Slider strengthSlider;
    [SerializeField] Text aliveTimer;
    [SerializeField] Text babyCount;

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

                    stats = hit.collider.GetComponent<MlAgent>();
                    character = hit.collider.GetComponent<Character>();
                    statsPanel.SetActive(true);

                    visionSlider.value = stats.vision / 6.0f;
                    speedSlider.value = stats.speed / 6.0f;
                    strengthSlider.value = stats.strenght / 6.0f;
                    aliveTimer.text = (character.GetIndividual().GetAge()).ToString();
                    babyCount.text = character.GetIndividual().GetNumberOfBabies().ToString();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            target = cameraPositions[0];
            onCreature = false;
            stats = null;
            statsPanel.SetActive(false);
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            target = cameraPositions[1];
            onCreature = false;
            stats = null;
            statsPanel.SetActive(false);
        } else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            target = cameraPositions[2];
            onCreature = false;
            stats = null;
            statsPanel.SetActive(false);
        } else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            target = cameraPositions[3];
            onCreature = false;
            stats = null;
            statsPanel.SetActive(false);
        }

        if (target)
        {
            if (onCreature)
            {
                aliveTimer.text = (character.GetIndividual().GetAge()).ToString();
                babyCount.text = character.GetIndividual().GetNumberOfBabies().ToString();
            }
            if (onCreature && Input.GetMouseButton(0))
            {
                float horizontalInput = Input.GetAxis("Mouse X");
                transform.RotateAround(target.position, Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);
                transform.parent = target;
            }
            else
            {
                Vector3 desiredPosition = target.position;
                if (onCreature) 
                {
                    desiredPosition += offset;
                    transform.LookAt(target);
                }
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
                transform.position = smoothedPosition;

                if (!onCreature)
                {
                    Quaternion desiredRotation = target.rotation;
                    Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, desiredRotation, smoothSpeed);
                    transform.rotation = smoothedRotation;
                }
                transform.parent = null;
            }
        }
    }
}
