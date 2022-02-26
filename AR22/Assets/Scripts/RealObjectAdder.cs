using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class RealObjectAdder : MonoBehaviour
{
    [SerializeField] private GameObject cursorChildObject;
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private bool useCursor = false;
    [SerializeField] private Button clearAllButton;
    [SerializeField] private Button enablePlaceButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button deleteSelectedButton;
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private Button drawerButton;
    [SerializeField] private Button animationButton;

    //List of gameobjects used for delete function.
    private List<GameObject> objects = new List<GameObject>();

    //GameObject selected for editing. PreviousObject is used to disable the outline, and to deselct an object.
    private GameObject editableObject;
    // Start is called before the first frame update
    void Start()
    {
        cursorChildObject.SetActive(useCursor);
        // Place object button
        enablePlaceButton.onClick.AddListener(enablePlace);
        confirmButton.onClick.AddListener(placeObject);
        clearAllButton.onClick.AddListener(deleteAllObjects);
        deleteSelectedButton.onClick.AddListener(deleteObject);
        editableObject = new GameObject();
        editableObject.AddComponent<Outline>();
        drawerButton.onClick.AddListener(openMenu);
    }

    bool touchMutex;
    Vector2 touch1, touch2, D1;
    float oldAngle;
    // Update is called once per frame
    void Update(){
        if(useCursor){UpdateCursor();}
        // Checks if a placed gameobject is pressed. This is the activation thing. 
        if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began){
            Debug.Log("PRESSSS");
            // register when an object is pressed.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            /*
             * set editableObject as current object and highlight only that one.
             */
            if(Physics.Raycast(ray, out hit)) { 
                var worldClickPosition = hit.point;
                // So this is how you get the object:
                GameObject hitObject = hit.collider.gameObject.transform.parent.gameObject;
                Debug.Log("Selected object: " + hitObject.name);
                // Confirm if it is not a re-selection.
                Debug.Log("OLD OBJECT: " + editableObject);
                bool isSameObject = hitObject == editableObject;
                if(isSameObject){
                    Debug.Log("Selected the same object.");
                    var c = editableObject.GetComponentsInChildren<Outline>();
                    foreach(Outline ol in c){ol.enabled = false;}
                    c = hitObject.GetComponentsInChildren<Outline>();
                    foreach(Outline ol in c){ol.enabled = false;}
                    editableObject = new GameObject();
                    editableObject.AddComponent<Outline>(); // This prevents am error when not hitting an object, and no object is being edited.
                } else { // Not the same object. editableObject != hitObject.
                    // STATUS: 
                    Debug.Log("New object selected");
                    if(hitObject.name != "AR Default Plane" || hitObject.name != "ARPlane"){ // Hit a gameobject. Not the plane. 
                        Debug.Log("Hit: " + hitObject.name);
                        var c = editableObject.GetComponentsInChildren<Outline>();
                        foreach(Outline ol in c){ol.enabled = false;}
                        editableObject = hitObject;
                        c = hitObject.GetComponentsInChildren<Outline>();
                        foreach(Outline ol in c){ol.enabled = true;}
                    // STATUS: 
                    } else { // Did not hit a gameobject. Unhighlight object.
                        Debug.Log("Hit a " + hitObject.name + " instead");
                        var c = editableObject.GetComponentsInChildren<Outline>();
                        foreach(Outline ol in c){ol.enabled = false;}
                        // // TODO: Make sure to remove it from the field.
                        // editableObject = new GameObject();
                        // editableObject.AddComponent<Outline>();
                    }
                }
            } else {
                Debug.Log("Did not hit anything.");
            }
        }

        // Checks if a gameobject is dragged. This should drag the object. It will drag exactly to where you are pointing. 
        if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved){
            if(editableObject.name != "AR Default Plane" && editableObject.name != "Trackables"){
                Debug.Log("DRAGGGGGGG " + editableObject.name);
                // THIS CODE CAUSES THE GAMEOBJECT TO RAPIDLY MOVE INTO YOUR FACE SO DONT USE IT JIM. 
                //This is sourced from youtube.com/watch?v=3_CX-KtsDic
                // var speedModifier = 0.01f;
                // editableObject.transform.position = new Vector3(
                //     transform.position.x + Input.GetTouch(0).deltaPosition.x * speedModifier,
                //     transform.position.y + Input.GetTouch(0).deltaPosition.y * speedModifier);
                //editableObject.transform.Rotate(0f, touch.deltaPosition.x, 0f); // This rotates things. Oops LOL. 

                // Define a mask, that prevents us from hitting the same object when dragging.
                int mask = 1 << LayerMask.NameToLayer("artifact_layer");


                var y = editableObject.transform.position.y;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);                
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit)) { 
                    if(hit.collider.gameObject.layer != LayerMask.NameToLayer("artifact_layer")){
                        var worldClickPosition = hit.point;
                        // The if-check prevents a strange bug, where raycast seemingly teleports the gameobject into the screen. 
                        // No idea what causes it. I haven't touched the raycast code before the bug i swear. 
                        if(editableObject.transform.position != new Vector3(0.9f,-1.0f,1.9f)){
                            editableObject.transform.position = worldClickPosition;
                            Debug.Log("Target position: " + worldClickPosition.ToString());
                        }
                    }
                }
            } else {
                Debug.Log("dont drag the ar planes it breaks the system thanks");
            }
        }

        // Checks if a gameobject is rotated. This should rotate the object.
        // https://stackoverflow.com/questions/32634791/calculate-touch-rotation-angle-with-two-fingers
        if(Input.touchCount == 2){
            Debug.Log("Rotate");
            if(touchMutex == false){
                touch1 = Input.GetTouch(0).position;
                touch2 = Input.GetTouch(1).position;
                D1 = touch1-touch2;
                touchMutex = true;
            }
            Vector2 D2 = new Vector2(0,0);
            // Still dragging so recalculate.
            if(Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved){
                var targetTouch1 = Input.GetTouch(0).position;
                var targetTouch2 = Input.GetTouch(1).position;
                D2 = targetTouch1 - targetTouch2;
                var angle = Mathf.Atan2(D1.y, D1.x)-Mathf.Atan2(D2.y,D2.x);
                if(!D2.Equals(new Vector2(0,0))){
                    Debug.Log("Rotating to angle: " + angle + ". D1: " + D1 + ". D2: " + D2);
                    editableObject.transform.rotation = new Quaternion(0, angle, 0, 1);
                    oldAngle = angle;
                }
            }

            // Solve: atan2(D2.y, D2.x)-atan2(D1.y, D1.x), where D1 is the starting angle, and D2 is the finishing angle. 

            // // Try 2.
            // Vector2 _startPosition = new Vector2();
            // if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(1).phase == TouchPhase.Began){
            //     _startPosition = touch2 - touch1;
            // }
            // if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved){
            //     var currVector = touch2 - touch1;
            //     var angle = Vector2.SignedAngle(_startPosition, currVector);
            //     editableObject.transform.rotation = Quaternion.Euler(0.0f, editableObject.transform.rotation.eulerAngles.y + angle*10, 0.0f);
            //     _startPosition = currVector;
            // }
        } else {
            touchMutex = false;
        }
    } 

    // Sets a gameobject as inactive. 
    void deleteObject(){
        Debug.Log("DEDEDELETE THIS");
        if(editableObject != null && editableObject.name != "AR Default Plane"){
            Debug.Log("Deleting object: " + editableObject.name);
            editableObject.SetActive(false);
            editableObject.transform.parent.gameObject.SetActive(false);
        } else { //
            Debug.Log("Object: " + editableObject);
        }
    }

    // Open the object catalog.
    void openMenu(){
        menuCanvas.SetActive(true);
        Debug.Log("MENUUUUU");
    }

    // Toggles visibility of the cursor and confirm button. Toggles useCursor.
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
        GameObject go = Instantiate(DataHandler.Instance.artifact, position2, transform.rotation);
        objects.Add(go);

        // Add a hidden outline to the object.
        var outline = go.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = Color.yellow;
        outline.OutlineWidth = 5f; // 5f is good for scale 0.2
        outline.enabled = false;
    }

    // When requested by a button press, destroy all objects.
    void deleteAllObjects(){
        Debug.Log("call deleteobjects");
        // Iterate through list and hide those objects.
        foreach(GameObject u in objects) {
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
