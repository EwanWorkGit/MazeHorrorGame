using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntrencePlatform : MonoBehaviour
{
    [SerializeField] MazeGenerations MazeGenerator;
    [SerializeField] Transform Player;
    void Start()
    {
        Vector3 startPosition = MazeGenerator.GetStartTile().transform.position;
        Vector3 platformPosition = startPosition - new Vector3(0, 0, 5f * transform.localScale.z);

        transform.position = platformPosition;
        Player.position = platformPosition + new Vector3(0, 1.05f, 0);
    }
}
