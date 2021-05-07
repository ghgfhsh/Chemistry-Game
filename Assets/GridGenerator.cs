using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public Particle particlePrefab;
    public GameObject Edge;

    public int width;
    [HideInInspector]public int height;

    public SpriteRenderer[,] gridSprites;
    public Particle[,] particleGrid;

    [SerializeField]public bool step = true;

    LiquidSimulator liquidSimulator;

    // Start is called before the first frame update
    void Start()
    {
        height = width / 16 * 9;
        gridSprites = new SpriteRenderer[width, height];
        particleGrid = new Particle[width, height];
        liquidSimulator = new LiquidSimulator(width, height, 1f);
        GenerateMap();
        CenterCamera();
        DrawParticles();
    }

    public void DrawParticles()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (particleGrid[x, y].mass < 1f)
                {
                    if (y + 1 < height && particleGrid[x, y + 1].particleType == particleGrid[x, y].particleType)
                        gridSprites[x, y].gameObject.transform.localScale = new Vector3(1, 1, 1);
                    else
                        gridSprites[x, y].gameObject.transform.localScale = new Vector3(1, Mathf.Min(particleGrid[x, y].mass, 1), 1);

                    float ypos = -((1f - Mathf.Max(Mathf.Min(1, gridSprites[x, y].gameObject.transform.localScale.y))) / 2);
                    //if (y > 0)
                    //    ypos += gridSprites[x, y - 1].gameObject.transform.localPosition.y;

                   

                    gridSprites[x, y].gameObject.transform.localPosition = new Vector3(0, ypos, -1);

                }
                else
                {
                    gridSprites[x, y].gameObject.transform.localScale = new Vector3(1, 1, 1);
                    gridSprites[x, y].gameObject.transform.localPosition = new Vector3(0, 0, -1);
                }

                gridSprites[x, y].sprite = particleGrid[x, y].sprite;
                gridSprites[x, y].gameObject.name = particleGrid[x, y].particleName + " mass: " + particleGrid[x, y].mass;

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || !step)
        {
            liquidSimulator.Simulate(ref particleGrid, ParticleType.water);
            DrawParticles();
        }
    }

    public void SpawnObjectAtCell(ParticleType particleType, Vector2Int coords)
    {
        particleGrid[coords.x, coords.y].ChangeParticleType(particleType);
        gridSprites[coords.x, coords.y].sprite = particleGrid[coords.x, coords.y].sprite;
        particleGrid[coords.x, coords.y].mass = 1f;
        DrawParticles();
    }

    private void GenerateMap()
    {
        for(int x = -1; x <= width; x++)
        {
            for (int y = -1; y <= height; y++)
            {
                //sets the edge
                if(x == -1 || x == width || y == -1 || y == height)
                {
                    Instantiate(Edge, new Vector3(x, y, 1), Quaternion.identity).transform.parent = gameObject.transform;
                    continue;
                }

                //creates background tiles
                GameObject empty = new GameObject(x + ", " + y);
                empty.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/WhiteSquare");
                empty.AddComponent<BoxCollider2D>().size = new Vector2(1, 1);
                empty.transform.localPosition = new Vector3(x, y, 1);
                empty.gameObject.AddComponent<MousePositionSetter>().coords = new Vector2Int(x, y);


                //fills all grid positions with empty particles
                gridSprites[x, y] = new GameObject("Empty").AddComponent<SpriteRenderer>();
                gridSprites[x, y].gameObject.AddComponent<BoxCollider2D>().size = new Vector2(1,1);
                gridSprites[x, y].gameObject.AddComponent<MousePositionSetter>().coords = new Vector2Int(x,y);
                gridSprites[x, y].transform.parent = empty.transform;
                gridSprites[x, y].transform.localPosition = new Vector3(0, 0, -1);
                particleGrid[x, y] = new Particle(ParticleType.empty);
            }
        }
        
    }

    private void CenterCamera()
    {
        Vector3 newPos = new Vector3(width / 2, height / 2, Camera.main.transform.position.z);
        Camera.main.transform.position = newPos;
        Camera.main.orthographicSize = (width / 50 * 20); // every 50 in size we want 20 more in orthographic to keep it in the right spot
    }


   
}

public enum ParticleType
{
    empty, solidBlock, water
}

