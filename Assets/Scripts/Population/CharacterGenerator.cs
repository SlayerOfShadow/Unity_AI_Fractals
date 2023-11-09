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
    public float characterScale = .5f;
    public float characterWidth = 0.2f;


    // TODO : créer une classe qui a une position, un nom, une size, une liste de prefab, un nombre d'éléments, une rotation, etc
    // TODO : créer un enum pour avoir les informations importantes sur le personnage comme sa taille, son épaisseur, etc

    // Start is called before the first frame update
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
        GenerateIndividualModel(adn_eye, adn_head, adn_chest, adn_legs, adn_arms, i + 4, individualObject.transform);
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
        float eyesHeight = characterSize - 0.3f;
        float HeadSizeOffsetZ = -.4f;
        Vector3 relativePosition = new Vector3((float)offsetBetweenIndividual, eyesHeight, HeadSizeOffsetZ);
        Vector3 positionWithoutOffset = modelObject.transform.position + relativePosition;
        float eyeScale = (size + 1) * 0.1f * characterScale; // size + 1 car size pout être égal à 0

        for (int i = 0; i <= numberOfEyes; i++)
        {
            Vector3 position = positionWithoutOffset;
            if (numberOfEyes != 0)
            {
                position.x += 
                    (numberOfEyes == 2 && i == 2) ?
                        0 :
                        ((i + 1) % 2) * characterWidth - (i % 2) * characterWidth;
                position.y += 
                        (i < 2) ?
                            0 :
                            characterWidth;
            }
            int n = i + 1;
            InstantiateObject("Eye" + n, eyePrefab, position, new Vector3(eyeScale, eyeScale, eyeScale), new Vector3(0f, 0f, 0f), parent);
        }
    }

    private void CreateHead(int Shape, int DeformSphapeByX, int DeformSphapeByY, int offsetBetweenIndividual, Transform parent)
    {
        float headHeight = characterSize - 0.3f;
        Vector3 relativePosition = new Vector3((float)offsetBetweenIndividual, headHeight, 0f);
        Vector3 position = modelObject.transform.position + relativePosition;
        float headScale = characterScale * .3f;

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
        Vector3 localScale = new Vector3(headScale + (float)DeformSphapeByX, headScale + (float)DeformSphapeByY, headScale);
        InstantiateObject("Head", prefab, position, localScale, new Vector3(0f, 0f, 0f), parent);
    }

    // avoir l'épaisseur de la tête pour un placement optimal
    private void CreateChest(int Shape, int DeformSphapeByX, int DeformSphapeByY, int offsetBetweenIndividual, Transform parent)
    {
        float chestHeight = characterSize - 0.9f;
        float headScale = characterScale * .3f;
        Vector3 relativePosition = new Vector3((float)offsetBetweenIndividual, chestHeight - headScale / 2, 0f);
        Vector3 position = modelObject.transform.position + relativePosition;
        float chestScale = characterScale * .9f;

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
        Vector3 localScale = new Vector3(chestScale + (float)DeformSphapeByX, chestScale + (float)DeformSphapeByY * 0.3f , chestScale);
        InstantiateObject("Chest", prefab, position, localScale, new Vector3(0f, 0f, 0f), parent);
    }

    private void CreateMembers(GameObject memberPrefab, string memberType, int numberOfMembers, Vector3 position, Vector3 localScale, Vector3 localRotation, Transform parent)
    {
        for (int i = 0; i <= numberOfMembers; i++)
        {
            float offsetBetweenMembers = (float)i * 0.2f;
            int n = i + 1;
            InstantiateObject(memberType + n, memberPrefab, position + new Vector3(characterWidth, 0.0f, offsetBetweenMembers), localScale, localRotation, parent);
            int j = numberOfMembers + n + 1;
            InstantiateObject(memberType + j, memberPrefab, position + new Vector3(-characterWidth, 0.0f, offsetBetweenMembers), localScale, -localRotation, parent);
        }
    }

    // avoir la position et la taille et l'épaissseur de la poitrine pour bien placer les bras au niveau 'd'épaules'
    private void CreateArms(int length, int numberOfArms, int offsetBetweenIndividual, Transform parent)
    {
        float armHeight = characterSize - .6f;
        float armThickness = 0.1f * characterScale;
        float armLength = (float)(length + 1f) * 0.2f * characterSize;
        float chestdSizeOffsetZ = -.1f;
        Vector3 position = modelObject.transform.position + new Vector3((float)offsetBetweenIndividual, armHeight, chestdSizeOffsetZ) ;
        Vector3 localScale = new Vector3(armThickness, armLength, armThickness);
        Vector3 localRotation = new Vector3(0f, 15f, 5f);

        CreateMembers(armPrefab, "Arm", numberOfArms, position, localScale, localRotation, parent);
    }

    // TODO : avoir la position et la taille et l'épaisseur en Z (pour l'offset entre les jambes) et en X (pour la séparation entre les groupes) de la poitrine pour bien placer les jambes en dessous
    private void CreateLegs(int length, int numberOfLegs, int offsetBetweenIndividual, Transform parent)
    {
        float legHeight = characterSize - 1.4f;
        float legThickness = 0.2f * characterScale;
        float legLength = (float)(length + 1f) * 0.05f * characterSize;
        float chestdSizeOffsetZ = -.1f;
        Vector3 position = modelObject.transform.position + new Vector3((float)offsetBetweenIndividual, legHeight, chestdSizeOffsetZ) ;
        Vector3 localScale = new Vector3(legThickness, legLength, legThickness);
        Vector3 localRotation = new Vector3(0f, 0f, 0f);

        CreateMembers(legPrefab, "Leg", numberOfLegs, position, localScale, localRotation, parent);
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

