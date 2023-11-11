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

        public float eyeProportion = 0.03f;
        public float headProportion = 0.3f; 
        public float chestProportion = 0.5f; 

        public float armLengthProportion = 0.1f; 
        public float legLengthProportion = 0.2f;

        public float armWidthProportion = 0.05f;
        public float legWidthProportion = 0.08f;

        public float eyeOffsetProportion = 0.4f;
        public float memberOffset = 0.6f;
    }

    CharacterDimension characterDimension = new CharacterDimension();

    // TODO : créer une classe qui a une position, un nom, une size, une liste de prefab, un nombre d'éléments, une rotation, etc

    public void GenerateCharacter(int i, Character.Individual individual)
    {
        GameObject individualObject = new GameObject("Individual" + i);
        individualObject.transform.parent = transform;

        // TODO : changer l'offset en fonction de l'épaisseur du personnage 
        GenerateIndividualModel(individual.genome, i, individualObject.transform);
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
        float headLength = characterDimension.size * characterDimension.headProportion;
        float headWidth = characterDimension.width * characterDimension.headProportion;
        Vector3 relativePosition = new Vector3(
            (float)offsetBetweenIndividual,
            characterDimension.size - headLength / 2.0f,
            - headWidth / 2.0f);
        Vector3 positionWithoutOffset = modelObject.transform.position + relativePosition;
        float eyeGenScale = (size + 1) * characterDimension.size * characterDimension.eyeProportion; // size + 1 car size peut être égal à 0
        float eyeOffset = headWidth * characterDimension.eyeOffsetProportion;

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
        float headLength = characterDimension.size * characterDimension.headProportion;
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

        float headWidth = characterDimension.width * characterDimension.headProportion;
        Vector3 localScale = new Vector3(
            headWidth + (float)DeformSphapeByX * characterDimension.headProportion,
            headLength + (float)DeformSphapeByY * characterDimension.headProportion,
            headWidth);
        
        InstantiateObject("Head", prefab, position, localScale, new Vector3(0f, 0f, 0f), parent);
    }

    private void CreateChest(int Shape, int DeformSphapeByX, int DeformSphapeByY, int offsetBetweenIndividual, Transform parent)
    {
        float headLength = characterDimension.size * characterDimension.headProportion;
        float chestLength = characterDimension.size * characterDimension.chestProportion;
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
        float chestWidth = characterDimension.width * characterDimension.chestProportion;
        Vector3 localScale = new Vector3(
            chestWidth + (float)DeformSphapeByX * characterDimension.chestProportion,
            chestLength + (float)DeformSphapeByY * characterDimension.chestProportion,
            chestWidth);
        InstantiateObject("Chest", prefab, position, localScale, new Vector3(0f, 0f, 0f), parent);
    }

    private void CreateMembers(GameObject memberPrefab, string memberType, int numberOfMembers, float memberWidth, Vector3 position, Vector3 localScale, Vector3 localRotation, Transform parent)
    {
        float chestWidth = characterDimension.width * characterDimension.chestProportion;
        float offsetBestweenGroupMembers = chestWidth * characterDimension.memberOffset - memberWidth;
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
        float headLength = characterDimension.size * characterDimension.headProportion;
        float armLength = characterDimension.size * characterDimension.armLengthProportion;
        float armGenLength = (float)(length + 1f) * armLength;
        Vector3 relativePosition = new Vector3(
            (float)offsetBetweenIndividual,
            characterDimension.size - headLength - armGenLength / 2f,
            0f);
        Vector3 position = modelObject.transform.position + relativePosition;
        float armWidth = characterDimension.width * characterDimension.armWidthProportion;
        Vector3 localScale = new Vector3(armWidth, armGenLength, armWidth);
        Vector3 localRotation = new Vector3(0f, 15f, 5f);

        CreateMembers(armPrefab, "Arm", numberOfArms, armWidth, position, localScale, localRotation, parent);
    }

    private void CreateLegs(int length, int numberOfLegs, int offsetBetweenIndividual, Transform parent)
    {
        float chestLength = characterDimension.size * characterDimension.chestProportion;
        float headLength = characterDimension.size * characterDimension.headProportion;
        float legLength = characterDimension.size * characterDimension.legLengthProportion;
        float legGenLength = (float)(length + 1f) * legLength;
        Vector3 relativePosition = new Vector3(
            (float)offsetBetweenIndividual,
            characterDimension.size - headLength - chestLength - legGenLength / 2f,
            0f);
        Vector3 position = modelObject.transform.position + relativePosition;
        float legWidth = characterDimension.width * characterDimension.legWidthProportion;
        Vector3 localScale = new Vector3(legWidth, legGenLength, legWidth);
        Vector3 localRotation = new Vector3(0f, 0f, 0f);

        CreateMembers(legPrefab, "Leg", numberOfLegs, legWidth, position, localScale, localRotation, parent);
    }

    public void GenerateIndividualModel(Character.Genome genome, int offsetBetweenIndividual, Transform parent)
    {
        CreateEyes(
            genome.GetIndex(Population.GenomeInformations.eyeSize1) + genome.GetIndex(Population.GenomeInformations.eyeSize2),
            genome.GetIndex(Population.GenomeInformations.eyeNumber1) + genome.GetIndex(Population.GenomeInformations.eyeNumber2),
            offsetBetweenIndividual,
            parent);
        CreateHead(
            genome.GetIndex(Population.GenomeInformations.headShape1) + genome.GetIndex(Population.GenomeInformations.headShape2),
            genome.GetIndex(Population.GenomeInformations.headDeformByY),
            genome.GetIndex(Population.GenomeInformations.headDeformByZ),
            offsetBetweenIndividual,
            parent);
        CreateChest(
            genome.GetIndex(Population.GenomeInformations.chestShape1) + genome.GetIndex(Population.GenomeInformations.chestShape2),
            genome.GetIndex(Population.GenomeInformations.chestDeformByY),
            genome.GetIndex(Population.GenomeInformations.chestDeformByZ),
            offsetBetweenIndividual,
            parent);
        CreateArms(
            genome.GetIndex(Population.GenomeInformations.armSize1) + genome.GetIndex(Population.GenomeInformations.armSize2),
            genome.GetIndex(Population.GenomeInformations.armNumber1) + genome.GetIndex(Population.GenomeInformations.armNumber2),
            offsetBetweenIndividual,
            parent);
        CreateLegs(
            genome.GetIndex(Population.GenomeInformations.legSize1) + genome.GetIndex(Population.GenomeInformations.legSize2),
            genome.GetIndex(Population.GenomeInformations.legNumber1) + genome.GetIndex(Population.GenomeInformations.legNumber2),
            offsetBetweenIndividual,
            parent);
    }
}

