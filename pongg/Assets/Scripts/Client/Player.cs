using UnityEngine;

public class Player : MonoBehaviour
{

    private readonly float moveSpeed = 5f;

    void Update()
    {
        float vertical = Input.GetAxis("Vertical");
        transform.Translate(Vector3.up * vertical * Time.deltaTime);
    }
}