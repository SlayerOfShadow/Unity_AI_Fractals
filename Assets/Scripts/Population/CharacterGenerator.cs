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


    public Individual individual;

    // Start is called before the first frame update
    void Start()
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

        visualize_individual(adn_eye, adn_head, adn_chest, adn_legs, adn_arms);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void GenerateEyes(int[] adnEye)
    {
        int bitSize = adnEye[0] + adnEye[1];
        int bitNumber = adnEye[2] + adnEye[3];
        CreateEyes(bitSize, bitNumber);
    }

    private void GenerateHead(int[] adnHead)
    {
        int bitShape = adnHead[0] + adnHead[1];
        int bitDeformY = adnHead[2];
        int bitDeformZ = adnHead[3];
        CreateHead(bitShape, bitDeformY, bitDeformZ);
    }

    private void GenerateChest(int[] adnChest)
    {
        int bitForm = adnChest[0] + adnChest[1];
        int bitSizeY = adnChest[2];
        int bitSizeZ = adnChest[3];
        CreateChest(bitForm, bitSizeY, bitSizeZ);
    }

    private void GenerateLegs(int[] adnLegs)
    {
        int bitNumber = adnLegs[0] + adnLegs[1];
        int bitSize = adnLegs[2] + adnLegs[3] + adnLegs[4] + adnLegs[5];
        CreateLegs(bitSize, bitNumber);
    }

    private void GenerateArms(int[] adnArms)
    {
        int bitNumber = adnArms[0] + adnArms[1];
        int bitSize = adnArms[2] + adnArms[3];
        CreateArms(bitSize, bitNumber);
    }

    private void CreateEyes(int size, int number)
    {
        float nSize = (size + 1) * 0.2f;
        GameObject eye = null;
        for (int i = 0; i <= number; i++)
        {
            if (number == 0)
            {
                eye = Instantiate(eyePrefab, modelObject.transform.position + new Vector3(0f, 0f, 1f), Quaternion.identity);
            }
            if (number == 1)
            {
                eye = Instantiate(eyePrefab, modelObject.transform.position + new Vector3((i % 2) * (-0.6f) + ((i + 1) % 2) * 0.6f, 0f, 1f), Quaternion.identity);
            }
            if (number == 2)
            {
                if (i < 2)
                {
                    eye = Instantiate(eyePrefab, modelObject.transform.position + new Vector3((i % 2) * (-0.6f) + ((i + 1) % 2) * 0.6f, 0f, 1f), Quaternion.identity);
                }
                else
                {
                    eye = Instantiate(eyePrefab, modelObject.transform.position + new Vector3(0f, 0.6f,1f), Quaternion.identity);
                }
            }
            if (number == 3)
            {
                eye = Instantiate(eyePrefab, modelObject.transform.position + new Vector3((i * 0.6f - (i % 2) * 0.6f) - 0.6f, (i % 2f) * 0.6f + 0f, 1f), Quaternion.identity);
            }
            eye.transform.localScale = new Vector3(nSize, nSize, nSize);
            eye.transform.SetParent(modelObject.transform);
        }
    }

    private void CreateHead(int Shape, int DeformY, int DeformZ)
    {
        GameObject head;
        switch (Shape)
        {
            case 0:
                head = Instantiate(headPrefab1, modelObject.transform.position + new Vector3(0f, 0f, 2f), Quaternion.identity);
                break;
            case 1:
                head = Instantiate(headPrefab2, modelObject.transform.position + new Vector3(0f, 0f, 2f), Quaternion.identity);
                break;
            case 2:
                head = Instantiate(headPrefab3, modelObject.transform.position + new Vector3(0f, 0f, 2f), Quaternion.identity);
                break;
            case 3:
                head = Instantiate(headPrefab4, modelObject.transform.position + new Vector3(0f, 0f, 2f), Quaternion.identity);
                break;
            default:
                // Handle invalid Shape values or provide a default behavior
                Debug.LogWarning("Invalid Shape value. Using default headPrefab1.");
                head = Instantiate(headPrefab1, modelObject.transform.position + new Vector3(0f, 0f, 1f), Quaternion.identity);
                break;
        }
        head.transform.localScale = new Vector3(2f + (float)DeformY, 2f + (float)DeformZ, 2f);
        head.transform.SetParent(modelObject.transform);
    }

    private void CreateChest(int bitForm, int bitSizeY, int bitSizeZ)
    {
        GameObject chest;
        switch (bitForm)
        {
            case 0:
                chest = Instantiate(chestPrefab1, modelObject.transform.position + new Vector3(0f, -2f, 2f), Quaternion.identity);
                break;
            case 1:
                chest = Instantiate(chestPrefab2, modelObject.transform.position + new Vector3(0f, -2f, 2f), Quaternion.identity);
                break;
            case 2:
                chest = Instantiate(chestPrefab3, modelObject.transform.position + new Vector3(0f, -2f, 2f), Quaternion.identity);
                break;
            case 3:
                chest = Instantiate(chestPrefab4, modelObject.transform.position + new Vector3(0f, -2f, 2f), Quaternion.identity);
                break;
            default:
                // Handle invalid Shape values or provide a default behavior
                Debug.LogWarning("Invalid Shape value. Using default headPrefab1.");
                chest = Instantiate(headPrefab1, modelObject.transform.position + new Vector3(0f, 0f, 1f), Quaternion.identity);
                break;
        }
        chest.transform.localScale = new Vector3(2 + bitSizeY, 2 + bitSizeZ * 0.3f, 2);
        chest.transform.SetParent(modelObject.transform);
    }

    private void CreateLegs(int size, int number)
    {
        float nSize = 0.2f;
        GameObject leg;
        for (int i = 0; i <= number; i++)
        {
            leg = Instantiate(legPrefab, modelObject.transform.position + new Vector3(0.6f, -(float)size * 0.2f-2.5f, 1.1f + (float)i * 0.5f), Quaternion.identity);
            leg.transform.localScale = new Vector3(nSize, (float)size * 0.15f, nSize);
            leg.transform.SetParent(modelObject.transform);

            leg = Instantiate(legPrefab, modelObject.transform.position + new Vector3(-0.6f, -(float)size * 0.2f - 2.5f, 1.1f + (float)i * 0.5f), Quaternion.identity);
            leg.transform.localScale = new Vector3(nSize, (float)size * 0.15f, nSize);
            leg.transform.SetParent(modelObject.transform);
        }
    }

    private void CreateArms(int size, int number)
    {
        float nSize = 0.2f;
        GameObject arm;
        for (int i = 0; i <= number; i++)
        {
            arm = Instantiate(armPrefab, modelObject.transform.position + new Vector3(1.2f, -1f - ((float)size + 1f) * 0.7f, 1.1f + (float)i * 0.5f), Quaternion.identity);
            arm.transform.localScale = new Vector3(nSize, (float)size * 0.2f + 1f, nSize);
            arm.transform.localRotation = Quaternion.Euler(new Vector3(0f, 15f, 5f));
            arm.transform.SetParent(modelObject.transform);

            arm = Instantiate(armPrefab, modelObject.transform.position + new Vector3(-1.2f, -1f - ((float)size + 1f) * 0.7f, 1.1f + (float)i * 0.5f), Quaternion.identity);
            arm.transform.localScale = new Vector3(nSize, (float)size * 0.2f + 1f, nSize);
            arm.transform.localRotation = Quaternion.Euler(new Vector3(0f, -15f, -5f));
            arm.transform.SetParent(modelObject.transform);
        }
    }

    public void visualize_individual(int[] adnEye, int[] adnHead, int[] adnChest, int[] adnLegs, int[] adnArms)
    {
        GenerateEyes(adnEye);
        GenerateHead(adnHead);
        GenerateChest(adnChest);
        GenerateLegs(adnLegs);
        GenerateArms(adnArms);
    }

}

