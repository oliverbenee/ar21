# Assignment 3 - Starting on your app (graded)

**Date**: 23.02.2022

**Group members participating**: Oliver Benée Petersen, Thorben Christopher Schmidt

**Activity duration**: 36 hours

15.02.2022: 2 hours Exercise 1 and adding buttons

17.02.2022: 6 hours Object selection & Fixing Excercise one

18.02.2022: 5 hours Solving excercise 2

19.02.2022: 13 hours fixing excercise 2 and Exercise 3.1

20.02.2022: 10 hours Fixing excercise 3.2

23.02.2022: 6 hours fixing up the report to hopefully acceptable standards. 

> 1.1.2: Explain how you fulfill the above requirements.

For the purpose of creating an AR application with the ability to add objects to the world, we created a new Scene, titled Assignment 3, with the following primary objects:

* Directional Light
* AR Session
* AR Session Origin
* Attached to this GameObject we add an AR Camera, which is used for the viewport of the user. Everything they see is dictated by the AR camera.
* Additionally, we attach an AR Default Plane GameObject. This feature is used for the raycast functionality, but adds ugly yellow planes. Therefore, the image view is disabled as shown in the image below. 

![ALT](111_DisabledPlane.PNG)

This solves the active plane tracking. Next, we concern ourselves with the button to add objects to the world. For this purpose, we created an 'enablePlaceButton'. This button calls the function 'enablePlace', which toggles the state of the cursor and 'confirmButton' (used to place objects). To toggle this button, we have set a listener to toggle the enablePlace() method, which enables the placeobject button and the AR cursor. It does so by enabling the cursor gameobject and changing the useCursor variable, used for telling whether the cursor is active or not. For reference, please see the code snippet below. 

```
    // Toggles visibility of the cursor and confirm button. Toggles useCursor.
    void enablePlace(){
        Debug.Log("NEW STATE: " + !useCursor);
        cursorChildObject.gameObject.SetActive(!useCursor);
        // change state of place object button;
        confirmButton.gameObject.SetActive(!useCursor);
        useCursor = !useCursor;
    }
```

This code toggles the state of the cursor and confirm placement button, solving the need for a button to press to enable adding an object. The confirm placement button can then be used for placing a button. We will discuss this later. This brings us now to the cursor. The cursor is the active raycasting object, and handles where the gameobject should be placed. This is calculated every frame by the update() call calling the updateCursor() method, assuming the useCursor field is set to true. For reference, please see below. 

```
    [SerializeField] private ARRaycastManager raycastManager;
    [...]
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
```

Whenever the cursor is updated, we use the AR Raycast Manager to determine a space in the world, we are looking at with our phone. The vector2 ScreenPosition sets this to be in the middle of the screen. This is calculated to world space using raycastManager.Raycast(), which populates the places the cursor hits on the AR Default Planes into the list of ARRaycastHits. It will do so for every plane it collides with, starting with the closest first. If we do hit a place, we want to set our cursor to be active, and have its position be set to the first hit (as that will only take the closest plane) and if not, we disable the cursor entirely. It will show a pink cursor, seen below. 

![ALT](https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/Screenshot_20220223-212014_AR22.jpg)

On the image above you can see the cursor. It is the two pink circles in the middle of the room. This cursor acts as our indication marker. 

Another thing to see is, even though we just said, that we use the AR Default Plane, there are no yellow planes and black lines. This is because we found a workaround, where they disappear if we disable the mesh renderer and remove the line renderer entirely. For reference, please see the image below.

![ALT](https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/no_renderers.png)

As for placing objects we added a "toggle place" button to enable placement of gameobjects. When placement is disabled, the place button does not light up, but is present when the cursor, and by extenssion placement, is present, as can be seen below. 

![ALT](https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/Screenshot_20220223-213413_AR22.jpg)
![ALT](https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/Screenshot_20220223-213417_AR22.jpg)

By pressing the checkmark, the user is then able to instantiate a new object. This object is created via. a RayCast, where we first determine the transform position, where we should place the gameobject. We here abuse, that the object adder script is attached to its cursor, so that we can get its transform by just pulling on the field "transform". To be specific, the addition of the RealObjectAdder script to the ARCursor means, that the field "transform" inside RealObjectAdder.cs now has the same properties as the transform associated with ARCursor. This solves the adding of gameobjects.

Please see these properties here:

![ALT](https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/cursor_properties.PNG)

We can now determine where to add the gameobject as seen in the code snippet below. The gameobject is instantiated at the previously discussed transform position from the cursor. We had some conversion issues while testing, so we created a "position2" for debugging, but strictly speaking it doesnt change anything. Note, that it is also added to a list "objects", which is used for deleting all gameobjects. 

```
    // When requested by a button press, create a new object. 
    void placeObject(){
        Vector3 position = transform.position; 
        Vector3 position2 = new Vector3(position.x, position.y, position.z);
        GameObject go = Instantiate(ObjectToPlace, position2, transform.rotation);
        objects.Add(go);
    }
```

After this, we add a hidden component outline, which is later used for highlighting of selected MOs. This is done using the Asset QuickOutline, and can be implemented the following code:

```
        // Add a hidden outline to the object.
        var outline = go.AddComponent<Outline>();
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = Color.yellow;
        outline.OutlineWidth = 5f;
        outline.enabled = false;
```

The outline implementation is based on [quickoutline.](https://assetstore.unity.com/packages/tools/particles-effects/quick-outline-115488) Although we are not the authors of this asset, the benefit of using this approach is, that it allows us to add our indication as a component to the gameobject, which leads to cleaner code. Additionally, we believe this looks significantly better than the alternative we know, which would be to change the shader of the gameobject, that is selected. An alternative would be to use the suggested bounds, but the use of quick outline leads to cleaner code. An illustration of our use of QuickOutline can be seen below:

![ALT](https://gitlab.au.dk/au598997/ar21/-/raw/assignment_3/Images/Screenshot_20220218-003837_AR22.jpg)

Sidenote: We temporarily re-enabled the AR debug planes for testing. 

When the outline component is attached, we can now create a yellow outline round our gameobject by calling the following code:

```
hitObject.GetComponent<Outline>().enabled = true;
```

This code allows for the outline to be enabled at runtime. In our case, we used raycast for object selection. When we cast a ray, we first get the gameobject and isolate parent objects:

```
var worldClickPosition = hit.point;
// So this is how you get the object:
GameObject hitObject = hit.collider.gameObject.transform.parent.gameObject;
```

And then we enable the outline only on the new object by writing:

```
editableObject.GetComponent<Outline>().enabled = false;
editableObject = hitObject;
hitObject.GetComponent<Outline>().enabled = true;
```

The disabling of the outline is a chicken-and-egg problem to explain. We essentially disable the outpline on the previous gameobject highlighted (named editableObject). We then take the new object, we hit (titled hitObject), and set that as the new editable object. We then activate the outline on that new object. This ensures, that only the latest gameobject will be highlighted. 

This brings us to the editableObject. This is a field of the gameobject used for editing:

```
//GameObject selected for editing. PreviousObject is used to disable the outline, and to deselct an object.
private GameObject editableObject;
```

This gameobject is the one the user is able to move and delete. On default, to prevent NullPointer exceptions, this is instantiated as an empty gameobject with an empty outline: 

```
editableObject = new GameObject();
editableObject.AddComponent<Outline>();
```

For the button to clear all objects, we created a list to track all available GameObjects: 

```
private List<GameObject> objects = new List<GameObject>();
```

As previously discussed when discussing placing an object, all created gameobjects are placed in this list. This allows us to delete all gameobjects by iterating through the list, and setting their ActiveState to false, which makes them invisible. We then empty the list using List.Clear(), which can be seen below. 

```
        Debug.Log("call deleteobjects");
        // Iterate through list and hide those objects.
        foreach(GameObject u in objects) {
            u.SetActive(false);
        }
        objects.Clear();
```

This technically doesn't delete the gameobjects, but prevents nullpointer exceptions we would otherwise get using the Destroy() function, since the gameobject we place is made up of multiple objects. 

> 2.1.3: Explain your solution

To select a gameobject to use, the gist is, that the user selects an object to be the editableObject. This is handled during the raycast when an object is not being placed.

We first check if the user presses an object. 
```
if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began){
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;
    if(Physics.Raycast(ray, out hit)) { 
        var worldClickPosition = hit.point;
        // So this is how you get the object:
        GameObject hitObject = hit.collider.gameObject.transform.parent.gameObject;
```

Given the gameobject we use, we find the parent object of the bike, since we otherwise only would select a component. This way, we can move hte whole bike. 

Next, we check that we get a new object. If it is the same object, we just want to disable the outline and make the user unable to further edit the object. We do this using a bool-check. 

```
Debug.Log("Selected object: " + hitObject.name);
// Confirm if it is not a re-selection.
Debug.Log("OLD OBJECT: " + editableObject);
bool isSameObject = hitObject == editableObject;
if(isSameObject){
    Debug.Log("Selected the same object.");
    editableObject.GetComponent<Outline>().enabled = false;
    hitObject.GetComponent<Outline>().enabled = false;
    editableObject = new GameObject();
    editableObject.AddComponent<Outline>(); // This prevents am error when not hitting a bike, and no bike is being edited.
```

If the user selects a new object, we need to identify if the user isn't hitting the AR default plane. If the user edits this plane, bad things happen. So we check if the user is hitting a bike first. If they are hitting the bike, we disable the outline of the old object, enable the outline of the new one, and make the new object editable by setting it to the field editableObject. 

```
} else { // Not the same object. editableObject != hitObject.
// STATUS: 
Debug.Log("New object selected");
if(hitObject.name == "BMXBikeE(Clone)"){ // Hit a bike.
    Debug.Log("Hit a bike!");
    editableObject.GetComponent<Outline>().enabled = false;
    editableObject = hitObject;
    hitObject.GetComponent<Outline>().enabled = true;
```

If the user doesn't hit a bike, we only disable the outline. We tried to set the editableObject variable to null when the user deselects the object, but we ran into issues with nullpointer-exceptions, when doing this. 

```
} else { // Did not hit a bike.
    Debug.Log("Hit a " + hitObject.name + " instead");
    editableObject.GetComponent<Outline>().enabled = false;
}
```

> 2.2.2: Explain your solution.

To delete a selected object, we can once again use our field editableObject: 

```
private GameObject editableObject;
```

Given that this object houses the object the user can modify, we can set the editableObject's active state to false to "delete" it. For reference see the following code snippet:

```
if(editableObject != null && editableObject.name != "AR Default Plane"){
    Debug.Log("Deleting object: " + editableObject.name);
    editableObject.SetActive(false);
    editableObject.transform.parent.gameObject.SetActive(false);
} else { //
    Debug.Log("Object: " + editableObject);
}
```

Before deleting anything, we need to prevent critical user errors, including NullPointers and deleting the AR plane. First, we check that there is an editableObject to delete. If not, we tell the user, that there is an error in what we try to delete. If the editable object is the AR Default plane, we also alert the user, that they are trying to delete the default plane, which would completely break the raycast function. 

If we are not trying to edit nothing, or the AR default plane, we set the editable object's active state to false, and set the state of the outline to false. This way, the user will not be able to see the gameobject. We decided to also disable the outline for futureproofing against if we wanted to make it possible for the user to re-enable the gameobject.

> 3.1.2: Explain your solution.

To move a GameObject, we assumed, that the user only uses one finger to drag a gameobject. Our implementation took inspiration from [HERE](youtube.com/watch?v=3_CX-KtsDic). First, we check that the user is only using one finger to  guard against errors when implementing rotation:

```
if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
```

If this is the case, then we assume the user is dragging a gameobject. We then need to check, that the user is dragging the right object, and not something like the AR default plane: 

```
if(editableObject.name == "BMXBikeE(Clone)")
```

After doing this, we can drag the object. We added a bit of redundancy in out implementation, but the essential part of the implementation are these lines of code:
            
```
var y = editableObject.transform.position.y;
Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
RaycastHit hit;
if(Physics.Raycast(ray, out hit)) { 
    var worldClickPosition = hit.point;
    editableObject.transform.position = worldClickPosition;
}
```

This code uses raycast to find a position on the screen, and translates it into a world position every frame. It then defines the new position of that gameobject (using transform.position) as the new position. Essentially, we teleport the gameobject to the correct place every single frame. An alternative would have been to add a constant to the vector of the gameobject, but this would have the limitation of not feeling "accurate" in editing, as the movement is no longer mapped one-to-one. 

> 3.2.2: Explain your solution. You must also - in detail - explain the math behind your solution (including some kind of sketch).

The gist of our implementation of rotation uses the function Atan2. Our solution is based on the proposal found ![HERE](https://stackoverflow.com/questions/32634791/calculate-touch-rotation-angle-with-two-fingers)

We start by assuming, that the first touch(X,Y) is the "straightline angle". We use this to calculate a "start vector" D1=A1-B1, where A1 and B1 are the two touch points. For reference, see the code below:

```
if(touchMutex == false){
    touch1 = Input.GetTouch(0).position;
    touch2 = Input.GetTouch(1).position;
    D1 = touch1-touch2;
    touchMutex = true;
}
```

After the calculation, we lock a "touchmutex" to prevent a new Update() cycle (or a new frame) from trying to recalculate the start vector for the rotation angle. Next, we wait for the user to move their fingers to the end position of the rotation. Each Update() cycle we calculate a new D2, which represents the "ending vector" angle of the rotation, assuming the user has moved their fingers. Once the user releases their touch, we solve D2=A2-B2, where A2 and B2 are the ending touch points. 

```
if(Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved){
    var targetTouch1 = Input.GetTouch(0).position;
    var targetTouch2 = Input.GetTouch(1).position;
    D2 = targetTouch1 - targetTouch2;
```

We now have two vectors D1 and D2 representing the angles of the start and end of the rotation. We can now use the function Atan2 to calculate the angle between the two vectors. Usually Atan2 is measured between the positive x-axis and another vector. For reference, see the image below. As a consequence, we have to calculate each angle seperately using Atan2, and then subtract them from one another to find the angle between the two vectors instead of one vector and the x-axis. 

![ALT](https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/1200px-Atan2definition.svg.png)

To circumvent the issue with the x-axis, we solve in our code: 

```
var angle = Mathf.Atan2(D1.y, D1.x)-Mathf.Atan2(D2.y,D2.x);
```

We then guard against an error, where D2 becomes (0,0). We are not sure why this error occurs, but checking that D2 isn't (0,0) makes the error disappear: 

```
if(!D2.Equals(new Vector2(0,0))){
    Debug.Log("Rotating to angle: " + angle + ". D1: " + D1 + ". D2: " + D2);
    editableObject.transform.rotation = new Quaternion(0, angle, 0, 1);
    oldAngle = angle;
}
```

After guarding against the error, as can be seen, we set the rotation to a fixed amount, based on how much the user turns their finger. This way, the GameObject doesn't rapidly spin when the user starts to turn the object. 

## Goal & Plan
Build a functioning appplication, that allows for placement, deletion, movement, and rotation of MOs.

## Results
Our application now allows for the placement of meaningful objects. These objects can be moved, deleted, and rotated. 

## Conclusion
We have successfully created an application that allows placement and basic manipulation of our MO.
