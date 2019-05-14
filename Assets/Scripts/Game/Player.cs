using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;

public class Player : MonoBehaviour
{
    public float moveSpeed = 15f;
    public float moveRange = 9f;

    public bool isMovable = true;

    private Vector3 networkPos;

    void Start()
    {
        //isMovable = 
    }

    void Update()
    {
        if (!isMovable)
        {
            return;
        }

        float input = Input.GetAxis("Vertical");

        Vector3 pos = transform.position;

        pos.z += input * moveSpeed * Time.deltaTime;
        pos.z = Mathf.Clamp(pos.z, -moveRange, moveRange);
        transform.position = pos;
    }
}