using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MazeGenerations : MonoBehaviour
{
    [SerializeField] GameObject GroundTile, TileWall;
    [SerializeField] Transform Parent;

    [SerializeField] List<MazeTile> PathedTiles = new();

    [SerializeField] float TileOffset = 5f;
    [SerializeField] int GridWidth = 5, GridHeight = 5;

    [SerializeField] int MaxPathingAttempts = 10, MinPathingLength = 1, MaxPathingLength = 12;

    MazeTile[,] Tiles;
    

    private void Awake()
    {
        Tiles = new MazeTile[GridHeight, GridWidth];

        GenerateGroundTiles(); //floor
        GenerateWalls(); //walls
        GeneratePath(); //carve out walls
        //DestroyStrayWalls(); //destroy "lonely" walls
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(GridWidth * TileOffset, 0f, GridWidth * TileOffset));

        if(PathedTiles != null && PathedTiles.Count > 0)
        {
            MazeTile finalTile = PathedTiles[PathedTiles.Count - 1];
            Gizmos.DrawCube(finalTile.transform.position + new Vector3(0, 10, 0), new Vector3(5, 2, 5));

            MazeTile startTile = PathedTiles[0]; //does not use GetStartTile since it has to be available outside of play
            Gizmos.DrawCube(startTile.transform.position + new Vector3(0, 10, 0), new Vector3(5, 2, 5));
        }
    }

    void GenerateGroundTiles()
    {
        for (int z = 0; z < GridHeight; z++)
        {
            for (int x = 0; x < GridWidth; x++)
            {
                float xOffset = (GridWidth * TileOffset) / 2f - (TileOffset / 2f);
                float zOffset = (GridHeight * TileOffset) / 2f - (TileOffset / 2f);

                Vector3 tilePosition = new Vector3(x * TileOffset, 0f, z * TileOffset);
                GameObject newTile = Instantiate(GroundTile, tilePosition - new Vector3(xOffset, 0, zOffset), Quaternion.identity);
                newTile.transform.parent = Parent;
                Tiles[z, x] = newTile.AddComponent<MazeTile>();
            }
        }
    }

    void GenerateWalls()
    {
        //add offset on x and z depending on wall, y is constant
        Vector3 yOffset = new Vector3(0, TileWall.transform.localScale.y / 2f, 0);
        Vector3 zOffset = new Vector3(0f, 0f, TileOffset / 2f);
        Vector3 xOffset = new Vector3(TileOffset / 2f, 0f, 0);

        for (int height = 0; height < GridHeight; height++)
        {
            for(int width = 0; width < GridWidth; width++)
            {
                MazeTile tile = Tiles[height, width];
                GameObject newWall = null;

                //north
                newWall = Instantiate(TileWall, tile.transform.position + yOffset + zOffset, Quaternion.identity);
                tile.Walls.Add(newWall);
                

                //east
                newWall = Instantiate(TileWall, tile.transform.position + yOffset + xOffset, Quaternion.Euler(new Vector3(0, -90, 0)));
                tile.Walls.Add(newWall);

                //EDGES ONLY

                //west
                if (width == 0)
                {
                    newWall = Instantiate(TileWall, tile.transform.position + yOffset - xOffset, Quaternion.Euler(new Vector3(0, 90, 0)));
                    tile.Walls.Add(newWall);
                }

                //south
                if(height == 0)
                {
                    newWall = Instantiate(TileWall, tile.transform.position + yOffset - zOffset, Quaternion.identity);
                    tile.Walls.Add(newWall);
                }

                foreach(GameObject wall in tile.Walls)
                {
                    wall.transform.parent = Parent;
                }
            }
        }
    }

    void GeneratePath()
    {
        int startingX = Random.Range(0, GridWidth);
        int currentZ = 0, currentX = startingX; //starting at the beginning of the maze with the randomized x

        MazeTile lastTile = null;
        MazeTile currentTile = Tiles[0, startingX];

        //removes wall from entrence
        Vector3 direction = -currentTile.transform.forward;
        if (Physics.Raycast(currentTile.transform.position, direction, out RaycastHit hit, TileOffset))
        {
            if (hit.collider != null)
            {
                Destroy(hit.collider.gameObject);
            }
        }

        PathedTiles.Add(Tiles[0, startingX]); //starting tile

       for(int attemps = 0; attemps < MaxPathingAttempts; attemps++)
       {
            int dir = Random.Range(0, 2); //will be put in height if z and width if x

            if (dir == 0)
            {
                //x
                bool posMovement = Random.Range(0, 2) == 0 ? false : true;

                int repeats = Random.Range(MinPathingLength, MaxPathingLength);
                for (int i = 0; i < repeats; i++)
                {
                    if(posMovement)
                    {
                        //+x
                        if(currentX + 1 < GridWidth)
                        {
                            currentX++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        //-x
                        if(currentX - 1 >= 0)
                        {
                            currentX--;
                        }
                        else
                        {
                            break;
                        }
                    }

                    lastTile = currentTile;
                    currentTile = Tiles[currentZ, currentX];

                    PathedTiles.Add(currentTile);
                    DestroyWall(lastTile, currentTile);
                }
            }
            else
            {
                //z
                bool posMovement = Random.Range(0, 2) == 0 ? false : true;

                int repeats = Random.Range(3, 8);
                for (int i = 0; i < repeats; i++)
                {
                    if (posMovement)
                    {
                        //+z
                        if (currentZ + 1 < GridHeight)
                        {
                            currentZ++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        //-z
                        if (currentZ - 1 >= 0)
                        {
                            currentZ--;
                        }
                        else
                        {
                            break;
                        }
                    }

                    lastTile = currentTile;
                    currentTile = Tiles[currentZ, currentX];

                    PathedTiles.Add(currentTile);
                    DestroyWall(lastTile, currentTile);
                }
            }
       }
    }

    void DestroyWall(MazeTile lastTile, MazeTile currentTile)
    {
        Vector3 direction = currentTile.transform.position - lastTile.transform.position;
        if(Physics.Raycast(lastTile.transform.position, direction, out RaycastHit hit, TileOffset))
        {
            if(hit.collider != null)
            {
                Debug.DrawRay(lastTile.transform.position, direction * 2f, Color.green, 10f);
                GameObject wall = hit.collider.gameObject;

                if(lastTile.Walls.Contains(wall))
                    lastTile.Walls.Remove(wall);
                if (currentTile.Walls.Contains(wall))
                    currentTile.Walls.Remove(wall);

                Destroy(wall);
            }
        }
    }

    void DestroyStrayWalls()
    {
        for(int height = 0; height < GridHeight; height++)
        {
            for(int width = 0; width < GridWidth; width++)
            {
                MazeTile tile = Tiles[height, width];

                if (tile != null)
                {
                    if (tile.Walls.Count <= 1)
                    {
                        if(height != 0 || width != 0) //to prevent edges from being removed
                        {
                            List<GameObject> wallsToRemove = new();
                            foreach (GameObject wall in tile.Walls)
                            {
                                wallsToRemove.Add(wall);
                                Destroy(wall);
                            }

                            foreach (GameObject wall in wallsToRemove)
                            {
                                tile.Walls.Remove(wall);
                            }
                        }
                        
                    }
                }
            }
        }
    }

    public MazeTile GetStartTile()
    {
        if(PathedTiles.Count > 0)
            return PathedTiles[0];
        else
            return null;
    }

    public MazeTile GetFarTile(Vector3 origin)
    {
        MazeTile furthestTile = PathedTiles.OrderBy(Tile => Vector3.Distance(Tile.transform.position, origin)).Last();
        return furthestTile;
    }

    public MazeTile GetRandomTileWithinRange(Vector3 origin, float minDistance, float maxDistance)
    {
        int randomX = Random.Range(0, GridWidth);
        int randomZ = Random.Range(0, GridHeight);

        MazeTile randomTile = Tiles[randomZ, randomX];
        if(PathedTiles.Contains(randomTile) && Vector3.Distance(origin, randomTile.transform.position) < maxDistance && Vector3.Distance(origin, randomTile.transform.position) > minDistance)
            return randomTile;

        else
            return null;
    }
}
    
