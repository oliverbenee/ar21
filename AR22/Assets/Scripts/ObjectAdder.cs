using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAdder : MonoBehaviour
{
    [SerializeField]
    private Vector3 position = new Vector3(0.7f, 2f, 0f); // Location of the object
    [SerializeField]
    private GameObject prefab, floor, flyingboi, person_man; //NOTE: IN THE UNITY EDITOR THE VALUE OF THIS FIELD CAN BE SPECIFIED BY RIGHT-CLICKING THE SCRIPT

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
        GameObject thing;
        thing = Instantiate(prefab, parent);
        // Import script MoveAround
        thing.AddComponent(typeof(MoveAround));
    }

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
                worldClickPosition = hit.point;
                // Click position is set. Place prefab.
                Debug.Log("Place at: " + worldClickPosition);
                Instantiate(floor, worldClickPosition, Quaternion.identity);
            }

        /*  
         * If you press Left Alt, place the ball. 
         */
        } else if (Input.GetKeyDown("left alt")){
            Vector3 location = new Vector3(3.64f, 1.98f, -1.82f);
            Debug.Log("key ALT pressed. Place thing in the middle of the air.");
            Instantiate(flyingboi, location, Quaternion.identity);
        }
    }
}
