using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public ParticleType test1;
    public ParticleType test2;

    Vector2Int mosPos;

    [SerializeField]GridGenerator gridGenerator;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            //Debug.Log(mosPos);
            gridGenerator.SpawnObjectAtCell(test1, mosPos);
        }
        if (Input.GetMouseButton(1))
        {
            //Debug.Log(mosPos);
            gridGenerator.SpawnObjectAtCell(test2, mosPos);
        }
    }

    public void SetMousePos(Vector2Int coords)
    {
        mosPos = coords;
    }
}
