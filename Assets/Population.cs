using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{

    public int populationSize = 6;
    private CharacterGenerator CharacterGeneratorScript; 

    // Start is called before the first frame update
    void Start()
    {
        CharacterGeneratorScript = GetComponentInChildren<CharacterGenerator>();
        if (CharacterGeneratorScript != null)
        {
            for (int i = 0; i < populationSize; i++){
                CharacterGeneratorScript.GenerateCharacter(i);
            }
        }
    }

    private void Update()
    {
    }
    // Update is called once per frame

}
