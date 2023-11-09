using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterGenerator : MonoBehaviour
{

    public GameObject eyePrefab; 
    public GameObject headPrefab1; 
    public GameObject headPrefab2; 
    public GameObject headPrefab3; 
    public GameObject headPrefab4; 
    public GameObject chestPrefab1;
    public GameObject chestPrefab2;
    public GameObject chestPrefab3;
    public GameObject chestPrefab4;
    public GameObject legPrefab;
    public GameObject armPrefab;

    public GameObject modelObject;

    public float characterSize = 1.8f;
    public float scale = .5f;
    public float width = 0.2f;

    // Start is called before the first frame update
    public void GenerateCharacter(int i, Character.Individual individual)
    {
        int[] adn_eye =  individual.genome.PartialGenome(0, 4);
        int[] adn_head =  individual.genome.PartialGenome(4, 4);
        int[] adn_chest =  individual.genome.PartialGenome(8, 4);
        int[] adn_legs =  individual.genome.PartialGenome(12, 6);
        int[] adn_arms = individual.genome.PartialGenome(18, 4);
        
        Debug.Log("Génome des yeux : " + string.Join(", ", adn_eye));
        Debug.Log("Génome de la tête : " + string.Join(", ", adn_head));
        Debug.Log("Génome du torse : " + string.Join(", ", adn_chest));
        Debug.Log("Génome des jambes : " + string.Join(", ", adn_legs));
        Debug.Log("Génome des bras : " + string.Join(", ", adn_arms));

        GameObject individualObject = new GameObject("Individual" + i);
        individualObject.transform.parent = transform;

        visualize_individual(adn_eye, adn_head, adn_chest, adn_legs, adn_arms, i + 4, individualObject.transform);
    }

    void Destroy(int individualId){           
        GameObject parentObject = GameObject.Find("Individual" + individualId);
        if (parentObject != null)
        {
            foreach (Transform child in parentObject.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }

    void Start(){}

    // Update is called once per frame
    void Update()
    {
    }


    private void GenerateEyes(int[] adnEye, int offset, Transform parent)
    {
        int bitSize = adnEye[0] + adnEye[1];
        int bitNumber = adnEye[2] + adnEye[3];
        CreateEyes(bitSize, bitNumber, offset, parent);
    }

    private void GenerateHead(int[] adnHead, int offset, Transform parent)
    {
        int bitShape = adnHead[0] + adnHead[1];
        int bitDeformY = adnHead[2];
        int bitDeformZ = adnHead[3];
        CreateHead(bitShape, bitDeformY, bitDeformZ, offset, parent);
    }

    private void GenerateChest(int[] adnChest, int offset, Transform parent)
    {
        int bitForm = adnChest[0] + adnChest[1];
        int bitSizeY = adnChest[2];
        int bitSizeZ = adnChest[3];
        CreateChest(bitForm, bitSizeY, bitSizeZ, offset, parent);
    }

    private void GenerateLegs(int[] adnLegs, int offset, Transform parent)
    {
        int bitNumber = adnLegs[0] + adnLegs[1];
        int bitSize = adnLegs[2] + adnLegs[3] + adnLegs[4] + adnLegs[5];
        CreateLegs(bitSize, bitNumber, offset, parent);
    }

    private void GenerateArms(int[] adnArms, int offset, Transform parent)
    {
        int bitNumber = adnArms[0] + adnArms[1];
        int bitSize = adnArms[2] + adnArms[3];
        CreateArms(bitSize, bitNumber, offset, parent);
    }

    private void CreateEyes(int size, int number, int offset, Transform parent)
    {
        float nSize = (size + 1) * 0.1f * scale;
        GameObject eye = null;
        for (int i = 0; i <= number; i++)
        {
            if (number == 0)
            {
                eye = Instantiate(eyePrefab, modelObject.transform.position + new Vector3(0f + (float)offset, characterSize - 0.3f, -.4f), Quaternion.identity);
            }
            if (number == 1)
            {
                eye = Instantiate(eyePrefab, modelObject.transform.position + new Vector3((i % 2) * (-width) + ((i + 1) % 2) * width + (float)offset, characterSize - 0.3f, -.4f), Quaternion.identity);
            }
            if (number == 2)
            {
                if (i < 2)
                {
                    eye = Instantiate(eyePrefab, modelObject.transform.position + new Vector3((i % 2) * (-width) + ((i + 1) % 2) * width + (float)offset, characterSize - 0.3f, -.4f), Quaternion.identity);
                }
                else
                {
                    eye = Instantiate(eyePrefab, modelObject.transform.position + new Vector3(0f + (float)offset, characterSize - 0.3f + width, -.4f), Quaternion.identity);
                }
            }
            if (number == 3)
            {
                eye = Instantiate(eyePrefab, modelObject.transform.position + new Vector3((i * width - (i % 2) * width) - width + (float)offset, (i % 2f) * width + characterSize - 0.3f, -.4f), Quaternion.identity);
            }
            eye.transform.localScale = new Vector3(nSize, nSize, nSize);
            eye.transform.SetParent(parent);
        }
    }

    private void CreateHead(int Shape, int DeformY, int DeformZ, int offset, Transform parent)
    {
        GameObject head;
        Vector3 relativePosition = new Vector3(0f + (float)offset, characterSize - 0.3f, 0f);
        Vector3 position = modelObject.transform.position + relativePosition;
        switch (Shape)
        {
            case 0:
                head = Instantiate(headPrefab1, position, Quaternion.identity);
                break;
            case 1:
                head = Instantiate(headPrefab2, position, Quaternion.identity);
                break;
            case 2:
                head = Instantiate(headPrefab3, position, Quaternion.identity);
                break;
            case 3:
                head = Instantiate(headPrefab4, position, Quaternion.identity);
                break;
            default:
                // Handle invalid Shape values or provide a default behavior
                Debug.LogWarning("Invalid Shape value. Using default headPrefab1.");
                head = Instantiate(headPrefab1, position, Quaternion.identity);
                break;
        }
        head.transform.localScale = new Vector3(scale + (float)DeformY, scale + (float)DeformZ, scale);
        head.transform.SetParent(parent);
    }

    private void CreateChest(int bitForm, int bitSizeY, int bitSizeZ, int offset, Transform parent)
    {
        GameObject chest;
        Vector3 relativePosition = new Vector3(0f + (float)offset, characterSize - 0.9f, 0f);
        Vector3 position = modelObject.transform.position + relativePosition;
        switch (bitForm)
        {
            case 0:
                chest = Instantiate(chestPrefab1, position, Quaternion.identity);
                break;
            case 1:
                chest = Instantiate(chestPrefab2, position, Quaternion.identity);
                break;
            case 2:
                chest = Instantiate(chestPrefab3, position, Quaternion.identity);
                break;
            case 3:
                chest = Instantiate(chestPrefab4, position, Quaternion.identity);
                break;
            default:
                // Handle invalid Shape values or provide a default behavior
                Debug.LogWarning("Invalid Shape value. Using default headPrefab1.");
                chest = Instantiate(headPrefab1, position, Quaternion.identity);
                break;
        }
        chest.transform.localScale = new Vector3(scale + bitSizeY, scale + bitSizeZ * 0.3f, scale);
        chest.transform.SetParent(parent);
    }

    private void CreateArms(int size, int number, int offset, Transform parent)
    {
        float nSize = 0.2f * scale;
        GameObject arm; 
        for (int i = 0; i <= number; i++)
        {
            arm = Instantiate(armPrefab, modelObject.transform.position + new Vector3(width + (float)offset, characterSize - .6f - ((float)size + 1f) * 0.7f, -.1f + (float)i * 0.2f), Quaternion.identity);
            arm.transform.localScale = new Vector3(nSize, (float)size * 0.2f, nSize);
            arm.transform.localRotation = Quaternion.Euler(new Vector3(0f, 15f, 5f));
            arm.transform.SetParent(parent);

            arm = Instantiate(armPrefab, modelObject.transform.position + new Vector3(-width + (float)offset, characterSize - .6f - ((float)size + 1f) * 0.7f, -.1f + (float)i * 0.2f), Quaternion.identity);
            arm.transform.localScale = new Vector3(nSize, (float)size * 0.2f, nSize);
            arm.transform.localRotation = Quaternion.Euler(new Vector3(0f, -15f, -5f));
            arm.transform.SetParent(parent);
        }
    }

    private void CreateLegs(int size, int number, int offset, Transform parent)
    {
        float nSize = 0.2f * scale;
        GameObject leg;
        for (int i = 0; i <= number; i++)
        {
            leg = Instantiate(legPrefab, modelObject.transform.position + new Vector3(width + (float)offset, characterSize -1.2f-(float)size * 0.2f - .5f, -.1f + (float)i * 0.2f), Quaternion.identity);
            leg.transform.localScale = new Vector3(nSize, (float)size * 0.15f + 1f, nSize);
            leg.transform.SetParent(parent);

            leg = Instantiate(legPrefab, modelObject.transform.position + new Vector3(-width + (float)offset, characterSize -1.2f -(float)size * 0.2f - .5f, -.1f + (float)i * 0.2f), Quaternion.identity);
            leg.transform.localScale = new Vector3(nSize, (float)size * 0.15f + 1f, nSize);
            leg.transform.SetParent(parent);
        }
    }

    public void visualize_individual(int[] adnEye, int[] adnHead, int[] adnChest, int[] adnLegs, int[] adnArms, int offset, Transform parent)
    {
        GenerateEyes(adnEye, offset, parent);
        GenerateHead(adnHead, offset, parent);
        GenerateChest(adnChest, offset, parent);
        GenerateArms(adnArms, offset, parent);
        GenerateLegs(adnLegs, offset, parent);
    }

}

