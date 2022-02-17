using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class RealObjectAdder : MonoBehaviour
{
    [SerializeField]
    private GameObject cursorChildObject;
    [SerializeField]
    private GameObject ObjectToPlace;
    [SerializeField]
    private ARRaycastManager raycastManager;
    [SerializeField]
    private bool useCursor = false;
    [SerializeField]
    private Button clearAllButton;
    [SerializeField]
    private Button enablePlaceButton;
    [SerializeField]
    private Button confirmButton;

    //List of gameobjects used for delete function.
    private List<GameObject> objects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        cursorChildObject.SetActive(useCursor);
        // Place object button
        enablePlaceButton.onClick.AddListener(enablePlace);
        confirmButton.onClick.AddListener(placeObject);
        clearAllButton.onClick.AddListener(deleteObjects);
    }

    // Update is called once per frame
    void Update()
    {
        if(useCursor){UpdateCursor();}
        // if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began){
        //     placeObject();    
        // } 
    }

    // Enable placing objects
    void enablePlace(){
        Debug.Log("NEW STATE: " + !useCursor);
        cursorChildObject.gameObject.SetActive(!useCursor);
        // change state of place object button;
        confirmButton.gameObject.SetActive(!useCursor);
        useCursor = !useCursor;
    }

    // When requested by a button press, create a new object. 
    void placeObject(){
        Debug.Log("call placeobject");
        Vector3 position = transform.position; 
        Vector3 position2 = new Vector3(position.x, position.y, position.z);
        GameObject go = Instantiate(ObjectToPlace, position2, transform.rotation);
        objects.Add(go);
    }

    // When requested by a button press, destroy all objects.
    void deleteObjects(){
        Debug.Log("call deleteobjects");
        // Iterate through list and hide those objects.
        foreach(GameObject u in objects) {
            //For some reason, we CANNOT destory the object. Because that would make sense right?
            u.SetActive(false);
        }
        objects.Clear();
    }

    // Update location of pink cursor
    void UpdateCursor(){
        Debug.Log("call updatecursor");
        Vector2 screenPosition = Camera.current.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);
        foreach (ARRaycastHit hit in hits){Debug.Log("HIT: " + hit);}
        if(hits.Count > 0){
            transform.position = hits[0].pose.position;
            transform.rotation = hits[0].pose.rotation;
            cursorChildObject.SetActive(true);
         } else {
             cursorChildObject.SetActive(false);
         }
    }
}
