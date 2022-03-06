using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    [SerializeField] private Button styleButton;
    [SerializeField] private Button switchPolishingModeButton;
    [SerializeField] private GameObject styleCanvas;


    [SerializeField] private Material mate;
    //List of gameobjects used for delete function.
    private List<GameObject> objects = new List<GameObject>();

    //GameObject selected for editing. PreviousObject is used to disable the outline, and to deselct an object.
    private GameObject editableObject;
    private GameObject shellObject;
    //Boolean to determine whether polishing mode is on or not
    private bool polishingModeOn;
    [SerializeField]
    private GameObject particleEffect;

    //broken vase prefab
    public GameObject brokenVase;
    public GameObject shatterPhysicsPlane;
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
        animationButton.onClick.AddListener(Animate);
        styleButton.onClick.AddListener(openMaterialMenu);
        switchPolishingModeButton.onClick.AddListener(switchPolishingMode);
    }

    bool touchMutex, isMenuOpen;
    Vector2 touch1, touch2, D1;
    float oldAngle;
    // Update is called once per frame
    void Update(){
        if(useCursor){UpdateCursor();}
        

        //Checking whether the camera is colliding with something
        Vector3 screenmiddle = new Vector3(539,1259,0);
        Ray breakRay = Camera.main.ScreenPointToRay(screenmiddle);
        RaycastHit breakHit;
        if(Physics.Raycast(breakRay, out breakHit)){
            if(breakHit.distance < 0.1){
                GameObject breakHitObject = breakHit.collider.gameObject.transform.parent.gameObject;
                if(breakHitObject != null && breakHitObject.name != "AR Default Plane" && breakHitObject.name != "Trackables"){
                    
                    Instantiate(brokenVase, breakHitObject.transform.position, breakHitObject.transform.rotation);
                    Instantiate(shatterPhysicsPlane, breakHitObject.transform.position + new Vector3(0,-1,0), breakHitObject.transform.rotation);
                    //Removes object
                    Destroy(breakHitObject);
                    Debug.Log("Bonked " + breakHitObject);
                }
            }
        }




        // Checks if a placed gameobject is pressed. This is the activation thing. 
        if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began && !isMenuOpen && !isPointerOverUI(Input.GetTouch(0))){
            Debug.Log("Manual log: ARE WE HITTING A MENU? " + isPointerOverUI(Input.GetTouch(0)));

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
                Debug.Log("Manual log: Selected object: " + hitObject.name);
                // Confirm if it is not a re-selection.
                Debug.Log("Manual log: OLD OBJECT: " + editableObject);
                bool isSameObject = hitObject == editableObject;
                if(isSameObject){
                    Debug.Log("Manual log: Selected the same object.");
                    var c = editableObject.GetComponentsInChildren<Outline>();
                    foreach(Outline ol in c){ol.enabled = false;}
                    c = hitObject.GetComponentsInChildren<Outline>();
                    foreach(Outline ol in c){ol.enabled = false;}
                    editableObject = new GameObject();
                    shellObject = new GameObject();
                    editableObject.AddComponent<Outline>(); // This prevents am error when not hitting an object, and no object is being edited.
                    isEditing(false);
                } else { // Not the same object. editableObject != hitObject.
                    // STATUS: 
                    Debug.Log("Manual log: New object selected");
                    if(hitObject.name != "AR Default Plane" || hitObject.name != "ARPlane"){ // Hit a gameobject. Not the plane. 
                        Debug.Log("Manual log: Hit: " + hitObject.name);
                        var c = editableObject.GetComponentsInChildren<Outline>();
                        foreach(Outline ol in c){ol.enabled = false;}
                        editableObject = hitObject;
                        shellObject = hit.collider.gameObject;
                        c = hitObject.GetComponentsInChildren<Outline>();
                        foreach(Outline ol in c){ol.enabled = true;}
                        isEditing(true);
                    // STATUS: 
                    } else { // Did not hit a gameobject. Unhighlight object.
                        Debug.Log("Manual log: Hit a " + hitObject.name + " instead");
                        var c = editableObject.GetComponentsInChildren<Outline>();
                        foreach(Outline ol in c){ol.enabled = false;}
                        // // TODO: Make sure to remove it from the field.
                        // editableObject = new GameObject();
                        // editableObject.AddComponent<Outline>();
                        isEditing(false);
                    }
                }
            } else {
                Debug.Log("Manual log: Did not hit anything.");
            }
        }

        // Checks if a gameobject is dragged. This should drag the object. It will drag exactly to where you are pointing. 
        if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved && !isPointerOverUI(Input.GetTouch(0))){
            if(editableObject.name != "AR Default Plane" && editableObject.name != "Trackables"){
                //Inserting scrubbing code here
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);       
                Debug.DrawRay(ray.origin, ray.direction, Color.yellow);         
                

                if(polishingModeOn){
                    foreach (MeshRenderer objectRenderer in shellObject.GetComponentsInChildren<MeshRenderer>()) {
                       var objectMaterial = objectRenderer.material.GetFloat("_GlossMapScale");
                        var objectSharedmaterial = objectRenderer.sharedMaterial.GetFloat("_GlossMapScale");
                        if(objectRenderer.name != "ARPlane" && objectMaterial < 1.0f && objectSharedmaterial < 1.0f){
                            objectRenderer.material.SetInt("_SmoothnessTextureChannel", 1);
                            objectRenderer.sharedMaterial.SetInt("_SmoothnessTextureChannel", 1);

                            objectRenderer.material.SetFloat("_Metallic", 0.27f);
                            objectRenderer.sharedMaterial.SetFloat("_Metallic", 0.27f);

                            objectRenderer.material.SetFloat("_GlossMapScale", objectMaterial + 0.005f);
                            objectRenderer.sharedMaterial.SetFloat("_GlossMapScale", objectSharedmaterial + 0.005f);

                        } else {
                            Debug.Log("Manual log: Error: Tried to change the smoothness of the plane.");
                        }
                    }
                } else {
                
                    Debug.Log("Manual log: DRAGGGGGGG " + editableObject.name);
                    int mask = 1 << LayerMask.NameToLayer("artifact_layer");


                    var y = editableObject.transform.position.y;         
                    RaycastHit hit;
                    if(Physics.Raycast(ray, out hit)) { 
                        if(hit.collider.gameObject.layer != LayerMask.NameToLayer("artifact_layer")){
                            var worldClickPosition = hit.point;
                            // The if-check prevents a strange bug, where raycast seemingly teleports the gameobject into the screen. 
                            // No idea what causes it. I haven't touched the raycast code before the bug i swear. 
                            if(editableObject.transform.position != new Vector3(0.9f,-1.0f,1.9f)){
                                editableObject.transform.position = worldClickPosition;
                                Debug.Log("Manual log: Target position: " + worldClickPosition.ToString());
                            } else {
                                Debug.Log("Manual log: Tried to move: " + editableObject.name + " to (0.9, -1.0, 1.9)");
                            }
                        }   
                    } else {
                        Debug.Log("Manual log: Raycast hit nothing. No movement.");
                    }
                }
            } else {
                Debug.Log("Manual log: dont drag the ar planes it breaks the system thanks");
            }
        }

        // Checks if a gameobject is rotated. This should rotate the object.
        // https://stackoverflow.com/questions/32634791/calculate-touch-rotation-angle-with-two-fingers
        if(Input.touchCount == 2 && !isPointerOverUI(Input.GetTouch(0))){
            Debug.Log("Manual log: Rotate");
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
                    Debug.Log("Manual log: Rotating to angle: " + angle + ". D1: " + D1 + ". D2: " + D2);
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
        Debug.Log("Manual log: DEDEDELETE THIS");
        if(editableObject != null && editableObject.name != "AR Default Plane" && editableObject.name != "Trackables"){
            Debug.Log("Manual log: Deleting object: " + editableObject.name);
            Destroy(FindGameObjectInChildWithTag(editableObject, "particle"));
            editableObject.SetActive(false);
            isEditing(false);
        } else { //
            Debug.Log("Manual log: Object: " + editableObject);
            Debug.Log("Manual log: Parent is: " + editableObject.gameObject.transform.parent);
        }
    }

    // Open the object catalog.
    void openMenu(){
        menuCanvas.SetActive(true);
        Debug.Log("Manual log: MENUUUUU");
    }

    // Toggles visibility of the cursor and confirm button. Toggles useCursor.
    void enablePlace(){
        Debug.Log("Manual log: NEW STATE: " + !useCursor);
        cursorChildObject.gameObject.SetActive(!useCursor);
        // change state of place object button;
        confirmButton.gameObject.SetActive(!useCursor);
        useCursor = !useCursor;
    }

    // When requested by a button press, create a new object. 
    void placeObject(){
        Debug.Log("Manual log: call placeobject");
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
        
        /*
        // // Add particle effect element. We have to add the local variable here, because otherwise it attaches the prefab field
        // // and bad things happen (TM);
        // var attachedEffect = particleEffect;
        // var particle = Instantiate(attachedEffect, editableObject.transform);
        // particle.tag = "particle";
        // particle.SetActive(false);
        // objects.Add(particle); // Lets us delete it later.
        */
    }

    // When requested by a button press, destroy all objects.
    void deleteAllObjects(){
        Debug.Log("Manual log: call deleteobjects");
        // Iterate through list and hide those objects.
        while(objects.Count != 0){
            foreach(GameObject u in objects) {
                objects.Remove(u);
                Destroy(u);
            }
            objects.Clear();
        }
        isEditing(false);
    }

    // Update location of pink cursor
    void UpdateCursor(){
        Debug.Log("Manual log: call updatecursor");
        Vector2 screenPosition = Camera.current.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);
        foreach (ARRaycastHit hit in hits){Debug.Log("Manual log: HIT: " + hit);}
        if(hits.Count > 0){
            transform.position = hits[0].pose.position;
            transform.rotation = hits[0].pose.rotation;
            cursorChildObject.SetActive(true);
         } else {
             cursorChildObject.SetActive(false);
         }
    }

    // Do the animation. 
    void Animate(){
        Debug.Log("Manual log: animate");
        GameObject emitter = FindGameObjectInChildWithTag(editableObject, "particle");
        objects.Add(emitter);
        if(emitter){
            Debug.Log("Manual log: found an emitter. destroying it.");
            Destroy(emitter);
            //if(emitter.isPaused){emitter.Play();} 
            //else {emitter.Pause();}
        } else {
            Debug.Log("Manual log: No emitter found. Creating a new one on: " + editableObject);
            var attachedEffect = particleEffect;
            var particle = Instantiate(attachedEffect, editableObject.transform);
            particle.tag = "particle";
        }
    }

    // Enables and disables buttons based on if an object is selected.
    void isEditing(bool isEditing){
        // Delete selected and animation buttons should only be active when an object is selected. so when isEditing is true. 
        deleteSelectedButton.gameObject.SetActive(isEditing);
        animationButton.gameObject.SetActive(isEditing);
        styleButton.gameObject.SetActive(isEditing);
    }

    // Roixo's answer. https://answers.unity.com/questions/893966/how-to-find-child-with-tag.html
    public static GameObject FindGameObjectInChildWithTag (GameObject parent, string tag){
        Transform t = parent.transform;
        for (int i = 0; i < t.childCount; i++) {
            Debug.Log("Manual log: Found first child. " + t.GetChild(i));
            Debug.Log("Manual log: Child has the tag: " + t.GetChild(i).tag);
            if(t.GetChild(i).gameObject.tag == tag){
                Debug.Log("Manual log: Found: " + t.GetChild(i).gameObject.name);
                return t.GetChild(i).gameObject;
            }    
        }   
        Debug.Log("Manual log: Found nothing.");
        return null;
    }

    // Open the material catalog.
    void openMaterialMenu(){
        styleCanvas.SetActive(true);
        isMenuOpen = true;
        Debug.Log("Manual log: MENUUUUU");
    }

    public void changeMaterial(){
        Debug.Log("Manual log: CHANGING MATEIRALS.");
        mate = DataHandler.Instance.material;
        GameObject toBeChanged = FindGameObjectInChildWithTag(shellObject, "physicalObject");
        if(shellObject.name != "AR Default Plane" || shellObject.name != "New Game Object"){
            Debug.Log("Manual log: CHANGING MATERIALS OF: " + shellObject.name);
            List<MeshRenderer> rendererList = new List<MeshRenderer>();
            foreach (MeshRenderer objectRenderer in shellObject.GetComponentsInChildren<MeshRenderer>()) {
                if(objectRenderer.name != "ARPlane"){
                    Debug.Log("Manual log: DADADADADADAD: " + objectRenderer.transform.parent.gameObject.name);
                    objectRenderer.material = mate;
                    objectRenderer.sharedMaterial = mate;
                    objectRenderer.materials = new Material[1] {mate};
                    objectRenderer.sharedMaterials = new Material[1] {mate};
                } else {
                    Debug.Log("Manual log: Error: Tried to change the color of the plane.");
                }
            }
        } else {
            Debug.Log("Manual log: Error: Tried to change color of the plane.");
        }
    }

    public void closeMenu(){
        isMenuOpen = false;
        Debug.Log("Manual log: Closing menu. Changing materials.");
        changeMaterial();
        
        // Re-enable outline since you pressed somewhere to add a material. 
        var outline = editableObject.GetComponent<Outline>().enabled = true;
    }

    public void switchPolishingMode(){
        polishingModeOn = !polishingModeOn;
        Debug.Log("Manual log: Switched polishing mode to: " + polishingModeOn);
        if(polishingModeOn){
            var sprite = Resources.Load<Sprite>("buttons/move");
            switchPolishingModeButton.GetComponent<Image>().sprite = sprite;
        } else {
            var sprite = Resources.Load<Sprite>("buttons/gem");
            switchPolishingModeButton.GetComponent<Image>().sprite = sprite;
        }
    }

    // SOURCE: youtube.com/watch=v=A7woL0oZCnA&t=523
    public bool isPointerOverUI(Touch touch){
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(touch.position.x, touch.position.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData,results);
        return results.Count > 0;
    }
}
