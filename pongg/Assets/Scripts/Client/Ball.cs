using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Ball : MonoBehaviour
{

    public bool isMoving = false;
    private float currentSpeed;
    public Vector2 currentDir;

    void Start()
    {
        StartCoroutine(resetBall());
    }

    void FixedUpdate()
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
        isMoving = false;
        transform.position = Vector3.zero;
        currentSpeed = 0;
        currentDir = Vector2.zero;

        yield return new WaitForSeconds(1f);

        isMoving = true;
        currentSpeed = 5f;
        currentDir = (Vector3.right + Vector3.up).normalized;
        RandomDirection();
    }

    private void RandomDirection()
    {
        int rand = UnityEngine.Random.Range(1, 4);
        currentDir = Quaternion.Euler(Vector3.forward * 90 * rand) * currentDir;
    }

}