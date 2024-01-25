using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigTreeManager : MonoBehaviour
{
    [SerializeField] GameObject[] bigTrees;
    public int bridgeCompleted = 0;
    AudioSource audioData;
    [SerializeField] AudioClip clip;

    void Start()
    {
        audioData = GetComponent<AudioSource>();
    }

    public void UpdateBigTree()
    {
        bridgeCompleted += 1;
        bigTrees[bridgeCompleted - 1].SetActive(false);
        bigTrees[bridgeCompleted].SetActive(true);
        audioData.PlayOneShot(clip);
    }
}
