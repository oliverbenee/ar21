using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAround : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Initializing MoveAround script...");
    }

    // Update is called once per frame
    void Update()
    {
        // Listen for WASD movements
        transform.position += new Vector3(Input.GetAxis("Horizontal")*0.05f,0,Input.GetAxis("Vertical")*0.05f);   
        // Listen for QE movements
        transform.Rotate(0, Input.GetAxis("Rotate"),0, Space.Self);     
    }
}
