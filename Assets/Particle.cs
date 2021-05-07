using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle
{
    public string particleName;

    [SerializeField]public float mass = 1f;

    public ParticleType particleType = ParticleType.empty;

    GridGenerator gridGenerator;

    public Sprite sprite;

    Sprite solidBlock = Resources.Load<Sprite>("Sprites/BlackSquare");
    Sprite water = Resources.Load<Sprite>("Sprites/Water Base");

    public Particle(ParticleType particleType)
    {
        ChangeParticleType(particleType);
    }

    public void ChangeParticleType(ParticleType particleType)
    {
        this.particleType = particleType;

        if (particleType == ParticleType.empty)
        {
            particleName = "Empty";
            sprite = null;
            mass = 0f;
        }
        else if (particleType == ParticleType.solidBlock)
        {
            particleName = "Solid Block";
            sprite = solidBlock;
            mass = 1f;
        }
        else if (particleType == ParticleType.water)
        {
            particleName = "Water";
            sprite = water;
        }
    }

    protected void Start()
    {
        gridGenerator = GameObject.FindGameObjectWithTag("GridGenerator").GetComponent<GridGenerator>();
    }
}
