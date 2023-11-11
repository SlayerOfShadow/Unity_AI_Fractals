using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

    class CharacterInformations
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

    CharacterInformations characterInformations = new CharacterInformations();

    // TODO : endroit où ranger parent offset et Position genre une IndividualBody et là tu peux mettre geenrateIndiv dedans

    class BodyPart
    {
        private string _name;
        private GameObject _prefab;
        private Vector3 _position;
        private Vector3 _scale;
        private Vector3 _rotation;
        private Transform _parent;

        public BodyPart(string name, GameObject prefab, Vector3 position, Vector3 scale, Vector3 rotation, Transform parent)
        {
            _name = name;
            _prefab = prefab;
            _position = position;
            _scale = scale;
            _rotation = rotation;
            _parent = parent;
        }

        public BodyPart(BodyPart bodyPart)
        {
            _name = bodyPart.GetName();
            _prefab = bodyPart.GetPrefab();
            _position = bodyPart.GetPosition();
            _scale = bodyPart.GetScale();
            _rotation = bodyPart.GetRotation();
            _parent = bodyPart.GetParent();           
        }

        public string GetName()
        {
            return _name;
        }

        public GameObject GetPrefab()
        {
            return _prefab;
        }

        public Vector3 GetPosition()
        {
            return _position;
        }

        public Vector3 GetScale()
        {
            return _scale;
        }

        public Vector3 GetRotation()
        {
            return _rotation;
        }

        public Transform GetParent()
        {
            return _parent;
        }

        public void SetName(string name)
        {
            _name = name;
        }

        public void SetPosition(Vector3 position)
        {
            _position = position;
        }

        public void OffsetPosition(Vector3 offset)
        {
            _position += offset;
        }

        public void SetScale(Vector3 scale)
        {
            _scale = scale;
        }

        public void MultiplyScale(Vector3 scalar)
        {
            _scale.x *= scalar.x;
            _scale.y *= scalar.y;
            _scale.z *= scalar.z;
        }
        
        public void SetRotation(Vector3 rotation)
        {
            _rotation = rotation;
        }

        public void InstantiateObject()
        {
            GameObject gameObject;
            gameObject = Instantiate(_prefab, _position, Quaternion.identity);
            gameObject.name = _name;
            gameObject.transform.localScale = _scale;
            gameObject.transform.localRotation = Quaternion.Euler(_rotation);
            gameObject.transform.SetParent(_parent);
        }
    }

    public class IndividualBody
    {
        private GameObject _gameObject;
        private Vector3 _position;

        public IndividualBody(string name, Transform transform, Vector3 modelPosition, float offset)
        {
            _gameObject = new GameObject(name);
            _gameObject.transform.parent = transform;

            _position = modelPosition + new Vector3(offset, 0f, 0f);
        }

        public Vector3 GetPosition()
        {
            return _position;
        }

        public Transform GetTransform()
        {
            return _gameObject.transform;
        }
    }

    public void GenerateCharacter(int i, Character.Individual individual)
    {
        IndividualBody individualBody = new IndividualBody(
            "Individual" + i,
            transform,
            modelObject.transform.position,
            (float)i * characterInformations.width);

        InstantiateIndividualModel(individual.genome, individualBody);
    }

    public void InstantiateIndividualModel(Character.Genome genome, IndividualBody individualBody)
    {
        float headLength = characterInformations.size * characterInformations.headProportion;
        float headWidth = characterInformations.width * characterInformations.headProportion;
        float chestLength = characterInformations.size * characterInformations.chestProportion;
        float chestWidth = characterInformations.width * characterInformations.chestProportion;

        // Eyes
        float eyeGenScale = GenSize(
            genome.GetIndex(Population.GenomeInformations.eyeSize1) + genome.GetIndex(Population.GenomeInformations.eyeSize2),
            characterInformations.size,
            characterInformations.eyeProportion); 
        BodyPart standardEye = new BodyPart( "Eye",
            eyePrefab,
            individualBody.GetPosition() + RelativePosition(- headLength / 2.0f, - headWidth / 2.0f), // set an eye standard position then instantiation will take care of offsets
            new Vector3(eyeGenScale, eyeGenScale, eyeGenScale),
            new Vector3(0f, 0f, 0f),
            individualBody.GetTransform()
            );

        float eyeOffset = headWidth * characterInformations.eyeOffsetProportion;
        InstantiateEyes(
            standardEye,
            genome.GetIndex(Population.GenomeInformations.eyeNumber1) + genome.GetIndex(Population.GenomeInformations.eyeNumber2),
            eyeOffset
        );

        // Head
        BodyPart head = new BodyPart(
            "Head",
            GenPrefab(
                genome.GetIndex(Population.GenomeInformations.headShape1) + genome.GetIndex(Population.GenomeInformations.headShape2),
                headPrefab1,
                headPrefab2,
                headPrefab3,
                headPrefab4),
            individualBody.GetPosition() + RelativePosition(- headLength / 2f),
            GenDeformScale(
                headLength,
                headWidth,
                genome.GetIndex(Population.GenomeInformations.headDeformByY),
                genome.GetIndex(Population.GenomeInformations.headDeformByZ),
                characterInformations.headProportion
            ),
            new Vector3(0f, 0f, 0f),
            individualBody.GetTransform()
        );
        head.InstantiateObject();

        // Chest
        BodyPart chest = new BodyPart(
            "Chest",
            GenPrefab(
                genome.GetIndex(Population.GenomeInformations.chestShape1) + genome.GetIndex(Population.GenomeInformations.chestShape2),
                chestPrefab1,
                chestPrefab2,
                chestPrefab3,
                chestPrefab4),
            individualBody.GetPosition() + RelativePosition(- headLength - chestLength / 2f),
            GenDeformScale(
                chestLength,
                chestWidth,
                genome.GetIndex(Population.GenomeInformations.chestDeformByY),
                genome.GetIndex(Population.GenomeInformations.chestDeformByZ),
                characterInformations.chestProportion
            ),
            new Vector3(0f, 0f, 0f),
            individualBody.GetTransform()
        );
        chest.InstantiateObject();

        // Arms
        float armGenLength = GenSize(
            genome.GetIndex(Population.GenomeInformations.armSize1) + genome.GetIndex(Population.GenomeInformations.armSize2),
            characterInformations.size,
            characterInformations.armLengthProportion);
        float armWidth = characterInformations.width * characterInformations.armWidthProportion;

        BodyPart rightArm = new BodyPart(
            "Arm",
            armPrefab,
            individualBody.GetPosition() + RelativePosition(- headLength - armGenLength / 2f),
            MemberScale(armGenLength, armWidth),
            new Vector3(0f, 15f, 5f),
            individualBody.GetTransform()
        );
        InstantiateMembers(
            rightArm,
            genome.GetIndex(Population.GenomeInformations.armNumber1) + genome.GetIndex(Population.GenomeInformations.armNumber2),
            armWidth,
            chestWidth);

        // Legs
        float legGenLength = GenSize(
            genome.GetIndex(Population.GenomeInformations.legSize1) + genome.GetIndex(Population.GenomeInformations.legSize2),
            characterInformations.size,
            characterInformations.legLengthProportion);
        float legWidth = characterInformations.width * characterInformations.legWidthProportion;

        BodyPart rightLeg = new BodyPart(
            "Leg",
            legPrefab,
            individualBody.GetPosition() + RelativePosition(- headLength - chestLength - legGenLength / 2f),
            MemberScale(legGenLength, legWidth),
            new Vector3(0f, 0f, 0f),
            individualBody.GetTransform()
        );
        InstantiateMembers(rightLeg,
            genome.GetIndex(Population.GenomeInformations.legNumber1) + genome.GetIndex(Population.GenomeInformations.legNumber2),
            legWidth,
            chestWidth);
    }

    private GameObject GenPrefab(int Shape, GameObject prefab0, GameObject prefab1, GameObject prefab2, GameObject prefab3)
    {
        GameObject prefab;
        switch (Shape)
        {
            case 0:
                prefab = prefab0;
                break;
            case 1:
                prefab = prefab1;
                break;
            case 2:
                prefab = prefab2;
                break;
            case 3:
                prefab = prefab3;
                break;
            default:
                // Handle invalid Shape values or provide a default behavior
                Debug.LogWarning("Invalid Shape value. Using default prefab0.");
                prefab = prefab0;
                break;
        }
        return prefab;
    }

    private Vector3 GenDeformScale(float length, float width, int deformSphapeByX, int deformSphapeByY, float proportion)
    {
        return new Vector3(
            width + (float)deformSphapeByX * proportion,
            length + (float)deformSphapeByY * proportion,
            width);
    }

    private float GenSize(int size, float characterSize, float proportion)
    {
        return (float)(size + 1f) * characterSize * proportion; // size + 1 car size peut être égal à 0
    }

    private Vector3 MemberScale(float length, float width)
    {
        return new Vector3(width, length, width);
    }

    private Vector3 RelativePosition(float yOffset, float zOffset = 0f)
    {
        return new Vector3(
            0f,
            characterInformations.size + yOffset,
            zOffset);
    }

    private void InstantiateEyes(BodyPart eye, int numberOfEyes, float eyeOffset)
    {
        for (int i = 0; i <= numberOfEyes; i++)
        {
            if (numberOfEyes != 0)
            {
                Vector3 offsetPosition = new Vector3();
                offsetPosition.x = 
                    (numberOfEyes == 2 && i == 2) ?
                        0 :
                        ((i + 1) % 2) * eyeOffset - (i % 2) * eyeOffset;
                offsetPosition.y = 
                        (i < 2) ?
                            0 :
                            eyeOffset;
                eye.OffsetPosition(offsetPosition);
                int n = i + 1;
                eye.SetName("Eye" + n);
            }
            eye.InstantiateObject();
        }
    } 

    // The left body part is made with the right one
    private void InstantiateMembers(BodyPart rightModel, int numberOfMembers, float memberWidth, float chestWidth)
    {
        float offsetBestweenGroupMembers = chestWidth * characterInformations.memberOffset - memberWidth;
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
                Debug.LogWarning("Invalid number of members, set miniOffset to 0f");
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
            
            BodyPart leftMember = new BodyPart(rightModel);
            BodyPart rightMember = new BodyPart(rightModel);

            // Instantiate right member
            int n = i + 1;
            rightMember.SetName(rightModel.GetName() + n);
            rightMember.OffsetPosition(new Vector3(offsetBestweenGroupMembers, 0.0f, positionMemberInZ));
            rightMember.InstantiateObject();

            // Instantiate left member
            int j = numberOfMembers + n + 1;
            leftMember.SetName(rightModel.GetName() + j);
            leftMember.OffsetPosition(new Vector3(- offsetBestweenGroupMembers, 0.0f, positionMemberInZ));
            leftMember.SetRotation(- rightModel.GetRotation());
            leftMember.InstantiateObject();
        }
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
}