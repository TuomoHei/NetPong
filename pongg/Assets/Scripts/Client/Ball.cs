using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public bool isMoving;

    private float currentSpeed;

    private Vector2 currentDir;

    void Start()
    {
        currentSpeed = 5f;
        currentDir = Random.insideUnitCircle.normalized;
    }

    void Update()
    {
        if (isMoving)
        {
            Vector2 moveDir = currentDir * currentSpeed * Time.deltaTime;
            transform.Translate(new Vector3(moveDir.x, moveDir.y, 0));
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Boundary")
        {
            currentDir.y *= -1;
        }

        if (other.tag == "Player")
        {
            currentDir.x *= -1;
        }

        if (other.tag == "BoundaryVer")
        {
            currentDir.x *= 1;
        }

        if (other.tag == "Goal")
        {
            StartCoroutine(resetBall());
        }
    }

    IEnumerator resetBall()
    {
        transform.position = Vector3.zero;
        currentSpeed = 0;
        currentDir = Vector2.zero;

        yield return new WaitForSeconds(1f);

        Start();
    }
}