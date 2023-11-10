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



    class CharacterDimension
    {
        public float size = 1.8f;
        public float width = 1f;
    }


    CharacterDimension characterDimension = new CharacterDimension();

    float eyeProportion = 0.03f;
    float headProportion = 0.3f; 
    float chestProportion = 0.5f; 

    float armLengthProportion = 0.1f; 
    float legLengthProportion = 0.2f;

    float armWidthProportion = 0.05f;
    float legWidthProportion = 0.08f;

    float eyeOffsetProportion = 0.4f;
    float memberOffset = 0.6f;

    // TODO : créer une classe qui a une position, un nom, une size, une liste de prefab, un nombre d'éléments, une rotation, etc

    public void GenerateCharacter(int i, Character.Individual individual)
    {
        int[] adn_eye = individual.genome.PartialGenome(0, 4);
        int[] adn_head = individual.genome.PartialGenome(4, 4);
        int[] adn_chest = individual.genome.PartialGenome(8, 4);
        int[] adn_legs = individual.genome.PartialGenome(12, 6);
        int[] adn_arms = individual.genome.PartialGenome(18, 4);
        
        Debug.Log("Génome des yeux : " + string.Join(", ", adn_eye));
        Debug.Log("Génome de la tête : " + string.Join(", ", adn_head));
        Debug.Log("Génome du torse : " + string.Join(", ", adn_chest));
        Debug.Log("Génome des jambes : " + string.Join(", ", adn_legs));
        Debug.Log("Génome des bras : " + string.Join(", ", adn_arms));

        GameObject individualObject = new GameObject("Individual" + i);
        individualObject.transform.parent = transform;

        // TODO : changer l'offset en fonction de l'épaisseur du personnage 
        GenerateIndividualModel(adn_eye, adn_head, adn_chest, adn_legs, adn_arms, i, individualObject.transform);
    }

    public void DestroyCharacter(int individualId){           
        GameObject parentObject = GameObject.Find("Individual" + individualId);
        if (parentObject != null)
        {
            foreach (Transform child in parentObject.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }

    private void GenerateEyes(int[] adnEye, int offsetBetweenIndividual, Transform parent)
    {
        int bitSize = adnEye[0] + adnEye[1];
        int bitNumber = adnEye[2] + adnEye[3];
        CreateEyes(bitSize, bitNumber, offsetBetweenIndividual, parent);
    }

    private void GenerateHead(int[] adnHead, int offsetBetweenIndividual, Transform parent)
    {
        int bitShape = adnHead[0] + adnHead[1];
        int bitDeformY = adnHead[2];
        int bitDeformZ = adnHead[3];
        CreateHead(bitShape, bitDeformY, bitDeformZ, offsetBetweenIndividual, parent);
    }

    private void GenerateChest(int[] adnChest, int offsetBetweenIndividual, Transform parent)
    {
        int bitForm = adnChest[0] + adnChest[1];
        int bitSizeY = adnChest[2];
        int bitSizeZ = adnChest[3];
        CreateChest(bitForm, bitSizeY, bitSizeZ, offsetBetweenIndividual, parent);
    }

    private void GenerateLegs(int[] adnLegs, int offsetBetweenIndividual, Transform parent)
    {
        int bitNumber = adnLegs[0] + adnLegs[1];
        int bitSize = adnLegs[2] + adnLegs[3] + adnLegs[4] + adnLegs[5];
        CreateLegs(bitSize, bitNumber, offsetBetweenIndividual, parent);
    }

    private void GenerateArms(int[] adnArms, int offsetBetweenIndividual, Transform parent)
    {
        int bitNumber = adnArms[0] + adnArms[1];
        int bitSize = adnArms[2] + adnArms[3];
        CreateArms(bitSize, bitNumber, offsetBetweenIndividual, parent);
    }

    private void InstantiateObject(string name, GameObject prefab, Vector3 position, Vector3 localScale, Vector3 localRotation, Transform parent)
    {
        GameObject gameObject;
        gameObject = Instantiate(prefab, position, Quaternion.identity);
        gameObject.name = name;
        gameObject.transform.localScale = localScale;
        gameObject.transform.localRotation = Quaternion.Euler(localRotation);
        gameObject.transform.SetParent(parent);
    }

    private void CreateEyes(int size, int numberOfEyes, int offsetBetweenIndividual, Transform parent)
    {
        float headLength = characterDimension.size * headProportion;
        float headWidth = characterDimension.width * headProportion;
        Vector3 relativePosition = new Vector3(
            (float)offsetBetweenIndividual,
            characterDimension.size - headLength / 2.0f,
            - headWidth / 2.0f);
        Vector3 positionWithoutOffset = modelObject.transform.position + relativePosition;
        float eyeGenScale = (size + 1) * characterDimension.size * eyeProportion; // size + 1 car size peut être égal à 0
        float eyeOffset = headWidth * eyeOffsetProportion;

        for (int i = 0; i <= numberOfEyes; i++)
        {
            Vector3 position = positionWithoutOffset;
            if (numberOfEyes != 0)
            {
                position.x += 
                    (numberOfEyes == 2 && i == 2) ?
                        0 :
                        ((i + 1) % 2) * eyeOffset - (i % 2) * eyeOffset;
                position.y += 
                        (i < 2) ?
                            0 :
                            eyeOffset;
            }
            int n = i + 1;
            InstantiateObject(
                "Eye" + n,
                eyePrefab,
                position,
                new Vector3(eyeGenScale, eyeGenScale, eyeGenScale),
                new Vector3(0f, 0f, 0f),
                parent);
        }
    }

    private void CreateHead(int Shape, int DeformSphapeByX, int DeformSphapeByY, int offsetBetweenIndividual, Transform parent)
    {
        float headLength = characterDimension.size * headProportion;
        Vector3 relativePosition = new Vector3(
            (float)offsetBetweenIndividual,
            characterDimension.size - headLength / 2.0f,
            0f);
        Vector3 position = modelObject.transform.position + relativePosition;

        GameObject prefab;
        switch (Shape)
        {
            case 0:
                prefab = headPrefab1;
                break;
            case 1:
                prefab = headPrefab2;
                break;
            case 2:
                prefab = headPrefab3;
                break;
            case 3:
                prefab = headPrefab4;
                break;
            default:
                // Handle invalid Shape values or provide a default behavior
                Debug.LogWarning("Invalid Shape value. Using default headPrefab1.");
                prefab = headPrefab1;
                break;
        }

        float headWidth = characterDimension.width * headProportion;
        Vector3 localScale = new Vector3(
            headWidth + (float)DeformSphapeByX * headProportion,
            headLength + (float)DeformSphapeByY * headProportion,
            headWidth);
        
        InstantiateObject("Head", prefab, position, localScale, new Vector3(0f, 0f, 0f), parent);
    }

    private void CreateChest(int Shape, int DeformSphapeByX, int DeformSphapeByY, int offsetBetweenIndividual, Transform parent)
    {
        float headLength = characterDimension.size * headProportion;
        float chestLength = characterDimension.size * chestProportion;
        Vector3 relativePosition = new Vector3(
            (float)offsetBetweenIndividual,
            characterDimension.size - headLength - chestLength / 2f,
            0f);
        Vector3 position = modelObject.transform.position + relativePosition;

        GameObject prefab;
        switch (Shape)
        {
            case 0:
                prefab = chestPrefab1;
                break;
            case 1:
                prefab = chestPrefab2;
                break;
            case 2:
                prefab = chestPrefab3;
                break;
            case 3:
                prefab = chestPrefab4;
                break;
            default:
                // Handle invalid Shape values or provide a default behavior
                Debug.LogWarning("Invalid Shape value. Using default chestPrefab1.");
                prefab = chestPrefab1;
                break;
        }
        float chestWidth = characterDimension.width * chestProportion;
        Vector3 localScale = new Vector3(
            chestWidth + (float)DeformSphapeByX * chestProportion,
            chestLength + (float)DeformSphapeByY * chestProportion,
            chestWidth);
        InstantiateObject("Chest", prefab, position, localScale, new Vector3(0f, 0f, 0f), parent);
    }

    private void CreateMembers(GameObject memberPrefab, string memberType, int numberOfMembers, float memberWidth, Vector3 position, Vector3 localScale, Vector3 localRotation, Transform parent)
    {
        float chestWidth = characterDimension.width * chestProportion;
        float offsetBestweenGroupMembers = chestWidth * memberOffset - memberWidth;
        float miniOffset;
        switch(numberOfMembers)
        {
            case 0 :
                miniOffset = 0f;
                break;
            case 1 :
                miniOffset = 0.5f; 
                break;
            case 2 :
                miniOffset = 0.33f;
                break;
            default :
                Debug.Log("Invalid number of members, set miniOffset to 0f");
                miniOffset = 0f;
                break;
        }
        float offsetBetweenMembers = (chestWidth - memberWidth) * miniOffset;
        for (int i = 0; i <= numberOfMembers; i++)
        {
            float positionMemberInZ = 
                (numberOfMembers == 1 && i == 0) ?
                - offsetBetweenMembers : 
                offsetBetweenMembers * ((float)i - (float)numberOfMembers + 1f);
            int n = i + 1;
            InstantiateObject(
                memberType + n,
                memberPrefab,
                position + new Vector3(offsetBestweenGroupMembers, 0.0f, positionMemberInZ),
                localScale,
                localRotation,
                parent);
            int j = numberOfMembers + n + 1;
            InstantiateObject(
                memberType + j,
                memberPrefab,
                position + new Vector3(- offsetBestweenGroupMembers, 0.0f, positionMemberInZ),
                localScale,
                - localRotation,
                parent);
        }
    }

    private void CreateArms(int length, int numberOfArms, int offsetBetweenIndividual, Transform parent)
    {
        float headLength = characterDimension.size * headProportion;
        float armLength = characterDimension.size * armLengthProportion;
        float armGenLength = (float)(length + 1f) * armLength;
        Vector3 relativePosition = new Vector3(
            (float)offsetBetweenIndividual,
            characterDimension.size - headLength - armGenLength / 2f,
            0f);
        Vector3 position = modelObject.transform.position + relativePosition;
        float armWidth = characterDimension.width * armWidthProportion;
        Vector3 localScale = new Vector3(armWidth, armGenLength, armWidth);
        Vector3 localRotation = new Vector3(0f, 15f, 5f);

        CreateMembers(armPrefab, "Arm", numberOfArms, armWidth, position, localScale, localRotation, parent);
    }

    private void CreateLegs(int length, int numberOfLegs, int offsetBetweenIndividual, Transform parent)
    {
        float chestLength = characterDimension.size * chestProportion;
        float headLength = characterDimension.size * headProportion;
        float legLength = characterDimension.size * legLengthProportion;
        float legGenLength = (float)(length + 1f) * legLength;
        Vector3 relativePosition = new Vector3(
            (float)offsetBetweenIndividual,
            characterDimension.size - headLength - chestLength - legGenLength / 2f,
            0f);
        Vector3 position = modelObject.transform.position + relativePosition;
        float legWidth = characterDimension.width * legWidthProportion;
        Vector3 localScale = new Vector3(legWidth, legGenLength, legWidth);
        Vector3 localRotation = new Vector3(0f, 0f, 0f);

        CreateMembers(legPrefab, "Leg", numberOfLegs, legWidth, position, localScale, localRotation, parent);
    }

    public void GenerateIndividualModel(int[] adnEye, int[] adnHead, int[] adnChest, int[] adnLegs, int[] adnArms, int offsetBetweenIndividual, Transform parent)
    {
        GenerateEyes(adnEye, offsetBetweenIndividual, parent);
        GenerateHead(adnHead, offsetBetweenIndividual, parent);
        GenerateChest(adnChest, offsetBetweenIndividual, parent);
        GenerateArms(adnArms, offsetBetweenIndividual, parent);
        GenerateLegs(adnLegs, offsetBetweenIndividual, parent);
    }

}

