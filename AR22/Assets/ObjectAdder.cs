using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAdder : MonoBehaviour
{
    [SerializeField]
    private Vector3 position = new Vector3(0.7f, 2f, 0f); // Location of the object
    [SerializeField]
    private GameObject prefab, raycast, flyingboi; //NOTE: IN THE UNITY EDITOR THE VALUE OF THIS FIELD CAN BE SPECIFIED BY RIGHT-CLICKING THE SCRIPT

    // Start is called before the first frame update
    void Start()
    {
        //ClientScene.RegisterPrefab();
        Debug.Log("Hello, World"); // Print hello world
        // Place the associated prefab (see this file in the unity editor)
        // At the position, we defined as "position". 
        //Instantiate(prefab, position, Quaternion.identity);

        // Place object directly on top of another by making the plane a parent object to the placed object.
        Transform parent = transform.Find("3D Plane");
        Instantiate(prefab, parent);
    }
    public bool ballPlaced; // This boolean is used to minimize lag. 

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Tick no. -------------->");

        /*
         * Place cylinder object by pressing Mouse1 or CTRL.
         */
        if(Input.GetButtonDown("Fire1")){
            // Source: https://www.youtube.com/watch?v=RGjojuhuk_s
            Vector3 worldClickPosition = Vector3.one; // placeholder for world click position
            // Calculate where we clicked.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)) { 
                Debug.Log("New position: " + hit.point);
                worldClickPosition = hit.point;
            }

            // Click position is set. Place prefab.
            Debug.Log("Place at: " + worldClickPosition);
            Instantiate(raycast, worldClickPosition, Quaternion.identity);
            worldClickPosition = hit.point;

        /*  
         * If you press Left Alt, place the ball. 
         */
        } else if (Input.GetKey("left alt") && !ballPlaced){
            Vector3 location = new Vector3(3.64f, 0.98f, -1.82f);
            Debug.Log("key ALT pressed. Place thing in the middle of the air.");
            Instantiate(flyingboi, location, Quaternion.identity);
            ballPlaced = true;
        }
    }
}
