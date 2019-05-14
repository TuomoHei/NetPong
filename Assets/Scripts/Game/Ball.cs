using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LiteNetLib;

public class Ball : MonoBehaviour
{
    public float startSpeed = 10f;
    public float maxSpeed = 20f;
    public float speedIncrease = 1f;

    public bool gameStart = false;

    private float currentSpeed;

    private Vector2 currentDir;

    private bool ballReset = false;

    void Start()
    {
        currentSpeed = startSpeed;
        currentDir = Random.insideUnitCircle.normalized;
    }

    void Update()
    {
        if (!gameStart)
        {
            //currentSpeed = 0;
            //currentDir = Vector2.zero;
        }

        if (ballReset)
        {
            return;
        }

        Vector2 moveDir = currentDir * currentSpeed * Time.deltaTime;
        transform.Translate(new Vector3(moveDir.x, 0f, moveDir.y));
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

        if (other.tag == "Goal")
        {
            StartCoroutine(resetBall());
            other.SendMessage("GetPoint", SendMessageOptions.DontRequireReceiver);
        }

        currentSpeed += speedIncrease;
        currentSpeed = Mathf.Clamp(currentSpeed, startSpeed, maxSpeed);
    }

    IEnumerator resetBall()
    {
        ballReset = true;
        transform.position = Vector3.zero;

        currentDir = Vector3.zero;
        currentSpeed = 0f;

        yield return new WaitForSeconds(3f);

        Start();

        ballReset = false;
    }
}