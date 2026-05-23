using UnityEngine;

public class player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame

    public float speed = 5;

    void Update()
    {

        float x =Input.GetAxis("Horizontal");
        float z =Input.GetAxis("Vertical");

        transform.Translate(x*speed*Time.deltaTime,0,z*speed*Time.deltaTime);


    }
}
