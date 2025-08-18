using System.Collections.Generic;
using UnityEngine;

public class MazeTile : MonoBehaviour
{
    public List<GameObject> Walls = new List<GameObject>();

    public void NullClearing()
    {
        Walls.RemoveAll(wall => wall == null || !wall);
    }
}
