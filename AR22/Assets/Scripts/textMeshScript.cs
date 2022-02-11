using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class textMeshScript : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Debug.Log("Camera Script running");
        Vector3 cameraPosition = Camera.main.transform.position;
        transform.LookAt(cameraPosition);
        transform.Rotate(0, 180, 0);
    }
}
