using System.Collections.Generic;
using UnityEngine;

public class LiquidSimulator
{
    private int width;
    private int height;

    public float maxMass = 1f;
    public float minMass = .005f;
    public float maxCompress = .02f;

    float MinFlow = 0.005f;
    float MaxFlow = 4f;
    float flowSpeed;

    public LiquidSimulator(int width, int height, float flowSpeed)
    {
        this.width = width;
        this.height = height;
        this.flowSpeed = flowSpeed;
    }

    private float CalculateVerticalCompression(float totalMass)
    {
        if (totalMass <= 1)
        {
            return maxMass;
        }
        else if (totalMass < 2 * maxMass + maxCompress)
        {
            return (maxMass * maxMass + totalMass * maxCompress) / (maxMass + maxCompress);
        }
        else
        {
            return (totalMass + maxCompress) / 2f;
        }
    }

    public void Simulate(ref Particle[,] particles, ParticleType particleToSimulate)
    {
        float flow = 0f;
        float remainingMass;
        float[,] liquidValueChange = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                liquidValueChange[x, y] = 0f;
            }
        }


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // validate
                if (particles[x, y].particleType != particleToSimulate) continue;
                if (particles[x, y].mass < minMass) continue;


                remainingMass = particles[x, y].mass; // gets the remaining mass. It's easier to work with than dealing with both of the arrays



                //if spot below can take water, caclulate water transfer amount
                if (y > 0 && (particles[x, y - 1].particleType == particleToSimulate || particles[x, y - 1].particleType == ParticleType.empty))
                {
                    //calculate flow
                    flow = CalculateVerticalCompression(particles[x, y].mass + particles[x, y - 1].mass) - particles[x, y - 1].mass;
                    if (flow > MinFlow) flow *= flowSpeed; // smooth flow by flow speed

                    //constrain the flow
                    flow = Mathf.Max(flow, 0);
                    if (flow > Mathf.Min(MaxFlow, particles[x, y].mass))
                        flow = Mathf.Min(MaxFlow, particles[x, y].mass);

                    //update values
                    if (flow != 0)
                    {
                        remainingMass -= flow;

                        liquidValueChange[x, y] -= flow;
                        liquidValueChange[x, y - 1] += flow;
                    }
                }


                if (remainingMass < minMass)
                    continue;

                if (x >= 1 && (particles[x - 1, y].particleType == ParticleType.empty || particles[x - 1, y].particleType == particleToSimulate)) //if the particle can flow left flow left
                {
                    //calculate flow
                    flow = (particles[x, y].mass - particles[x - 1, y].mass) / 4f;
                    if (flow > MinFlow) flow *= flowSpeed; // smooth flow by flow speed

                    //constrain the flow
                    flow = Mathf.Max(flow, 0);
                    if (flow > Mathf.Min(MaxFlow, particles[x, y].mass))
                        flow = Mathf.Min(MaxFlow, particles[x, y].mass);

                    //update values
                    if (flow != 0)
                    {
                        remainingMass -= flow;

                        liquidValueChange[x, y] -= flow;
                        liquidValueChange[x - 1, y] += flow;
                    }

                }

                if (remainingMass < minMass)
                    continue;


                if (x + 1 < width && (particles[x + 1, y].particleType == ParticleType.empty || particles[x + 1, y].particleType == particleToSimulate)) // if the particle can flow right, flow right
                {
                    //calculate flow
                    flow = (particles[x, y].mass - particles[x + 1, y].mass) / 4f;
                    if (flow > MinFlow) flow *= flowSpeed; // smooth flow by flow speed

                    //constrain the flow
                    flow = Mathf.Max(flow, 0);
                    if (flow > Mathf.Min(MaxFlow, particles[x, y].mass))
                        flow = Mathf.Min(MaxFlow, particles[x, y].mass);

                    //update values
                    if (flow != 0)
                    {
                        remainingMass -= flow;

                        liquidValueChange[x, y] -= flow;
                        liquidValueChange[x + 1, y] += flow;
                    }
                }
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (particles[x, y].particleType == particleToSimulate)
                    particles[x, y].mass += liquidValueChange[x, y];
                else if(particles[x,y].particleType == ParticleType.empty && liquidValueChange[x, y] > 0f)
                {
                    particles[x, y].ChangeParticleType(ParticleType.water);
                    particles[x, y].mass = liquidValueChange[x, y];
                }
                if (particles[x, y].mass < minMass) particles[x, y].ChangeParticleType(ParticleType.empty);
            }
        }
    }
}
