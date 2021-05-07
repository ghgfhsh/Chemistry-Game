using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePositionSetter : MonoBehaviour
{
    PlayerInput playerInput;

    public Vector2Int coords;


    private void Start()
    {
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
    }

    protected void OnMouseOver()
    {
        playerInput.SetMousePos(coords);
    }
}
