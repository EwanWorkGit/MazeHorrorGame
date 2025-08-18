using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class NavigationManager : MonoBehaviour
{
    [SerializeField] NavMeshSurface Surface;
    [SerializeField] Transform Navigator;
    [SerializeField] Transform Origin;
    [SerializeField] MazeGenerations MazeGenerator;
    void Start()
    {
        Surface.BuildNavMesh();
        Navigator.position = MazeGenerator.GetFarTile(Origin.position).transform.position;
        if(!Navigator.gameObject.activeInHierarchy)
            Navigator.gameObject.SetActive(true);
    }
}
