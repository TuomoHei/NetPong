using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Network network;

    public int id;

    private readonly float moveSpeed = 2.5f;

    void Update()
    {
        id = network.id;

        if (id == 0)
        {
            gameObject.transform.position = new Vector3(-5, gameObject.transform.position.y, gameObject.transform.position.z);
        }

        else if (id == 1)
        {
            gameObject.transform.position = new Vector3(5, gameObject.transform.position.y, gameObject.transform.position.z);
        }

        float vertical = Input.GetAxis("Vertical");
        transform.Translate(Vector3.up * vertical * Time.deltaTime * moveSpeed);

        var pos = transform.position;
        pos.y = Mathf.Clamp(transform.position.y, -3.5f, 3.5f);
        transform.position = pos;

    }
}