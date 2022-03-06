# Assignment 5 - Interaction techniques (graded)

**Date**: 06.03.2022

**Group members participating**: Oliver Benée Petersen, Thorben Christopher Schmidt

**Activity duration**: [TODO: Insert hours spend]

Friday 04.03.2022: 6 hours

saturday 05.03.2022: 12 hours

Sunday 06.03.2022: 


## Goal
We intend to implement the following three interaction techniques:

### Technique 1 - Theme plates. [Marker, Appearance]

For our first interaction technique, we intend to let the user to create general themes in their rooms using marker interaction. We want to let the user print out XR markers representative of a desired theme, and have the user tie placed artifacts to the marker. Any artifact, that is moved to this marker should change to the same material as that of the marker. Thus, the new object will fit the theme, that the user has decided for the area of the marker.  

We argue, that this approach fulfills the criteria of using tracked image marker interaction for the detection of the theme plates, and changing the appearance of an object by changing the surface material of the artifact. 

### Technique 2 - Watch your step [Relation, Animation]

For the second interaction, we want to solidify the value and fragility, that is inherent to ancient artifacts by implementing a system, that breaks the placed artifacts when a user walks into them. We intend to implement this by measuring the relation in the form of the distance between the user and an artifact, and if the two are too close, the vase will break apart. 

We argue, that this approach fulfills the criteria of using the relation between the user, and the placed artifact, as the artifact can only be destroyed, when the two are close. For the breakage, we argue that the use of rigidbodies to make the object fall apart fulfills the critera of animating a gameobject in response to user interaction.

### Technique 3 - Polishing [Gesture, Appearance]

The third technique, we intend to implement the ability for users to polish placed artifacts by scrubbing a selected object with their finger. The scrubbing of the artifact is intended to simulate rubbing the artifact with a piece of cloth or other polishing device. This way, the user will be able to gesture at a selected object to polish it.

We argue, that the use of dragging back and forth acts as a gesture simulating rubbing on the artifact. By polishing the artifact, we want to make it shiny and prettier, thus improving the appearance of the object. 

## Plan

For the implementation of the three interaction techniques, we will both be working to implement the polishing technique. Afterwards, Oliver will be focusing on the implementation of the theme plates for marker interaction, as well as writing the report, while Thorben will focus on implementing the artifact breakage of technique 2.

## Results

For each of the techniques, we will now discuss their intended functionality further, a plan of implementation, as well as the coding of this feature in unity. 

### Technique 1 - Theme plates.  
 
To implement the feeling of a marker being representative of a room, we are going to implement a marker interaction, where users can place the marker in the middle of the room, and have nearby artifacts be rendered as a color nearby. However, since rendering entire rooms is going to be performance intensive, we are going to let theme markers create a field around them, where all objects inside will change to the theme on the plate. This way, we don't have to generate a model of the room in the program, which would be performance intensive not only to save, but also to maintain. 

First, we start by selecting a set of theme plates, we can use for the design. We decided for the sake of consistency to use the same image as the one used for generating the material texture, as this makes the theme plates have an intrinsic connection beetween the output design, and their looks. To use them for the XR image recognition, we save them to a ReferenceImageLibrary, which contains all the images we can recognize. For reference, see the image below. 

<img src="https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/texturereferenceimagelibrary.PNG" />

As can be seen in the image above, we created a reference image library containing all the textures, that our vases support. We can now begin to create the image tracking feature. To do this, we add to our AR Session Origin an AR Tracked Image Manager, which will contain our reference image library. For referene see the top selected element in the inspector in the image below:

<img src="https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/attachedtoarsessionorigin.PNG" />

Now that we have a list of images we can track using our AR Tracked Image Manger, we need some way to use them. As can also be seen in the image above, we attached a script XRMaterialrecognition, which has the responsibility of taking the information from the ARTrackedImageManger, and setting the theme in the area. First, the recognizer subscribes to the image manager:

```c#
    // Start is called before the first frame update
    void Awake(){
        imageManager = FindObjectOfType<ARTrackedImageManager>();
    }

    //Subscribe to image tracking
    void OnEnable(){imageManager.trackedImagesChanged += OnImageChanged;}
```

Using these two methods call, we set the AR Tracked Image Manager to be the image manager for the material recognizer, and subscribe to its events. Now we can use the method OnImageChanged() to subscribe to found theme plates. Each update makes a call to the method OnImageChanged, which will handle the updating of the theme in the room. We start by first detecting information about the image. We find the image's location in world space, and the material associated with the image:

```c#
private Material amphora, hydria, voluteKrater, mate;

[...]

foreach (var trackedImage in args.updated){
    // Tell us which image, we are looking at, and where it is. 
    Vector3 imageLocation = trackedImage.transform.position;
    string imageName = trackedImage.referenceImage.name;
    // set correct material. 
    Debug.Log("Reference image name: " + imageName);

    if(trackedImage.referenceImage.name.Contains("Amphora_Albedo")){
        Debug.Log("MATE IS AMPHORA");
        mate = amphora;
    }
    if(trackedImage.referenceImage.name.Contains("Hydria_Albedo")){
        Debug.Log("MATE IS HYDRIA");
        mate = hydria;
    }
    if(trackedImage.referenceImage.name.Contains("VoluteKrater_Albedo")){
        Debug.Log("MATE IS VOLUTE KRATER");
        mate = voluteKrater;
    }
}
```

The summary of this code is, that we save the materials for the vases as fields amphora, hydria, and voluteKrater. Once, we track an image, we compare the name of the image, and see if the name of the image is similar to one of the materials used. Since all instances in unity get a unique ID, we use Contains() to check, if the image name (and thus object name) matches a known material. We then set the field "mate" as the material, that corresponds to the material. This mate field will then later be used for creating the theme. 

Next, we need to find the objects in vicinity to create the theme around. To avoid the performance limitations of tracking a complete room and walls, we use OverlapSpheres to define an area in which we are going to set the theme. This method returns an array with all colliders touching or inside the sphere, which is useful for image tracking, as it allows us to define a general area, where we can search for objects in which to create our theme. Using the OverlapSphere, we can iterate through found objects like a list, and filter out the AR Plane and AR Camera. Filtering out these elements are important, as changing the material of the AR Camera creates exceptions (since the camera cannot have a material), and changing the material of the AR Plane makes it visible with the color of the theme plate. 

```c#
var closetsobjects = Physics.OverlapSphere(imageLocation, 20);
foreach (var item in closetsobjects){
    bool isPlaneOrCam = !item.transform.gameObject.name.Contains("ARPlane") && !item.transform.gameObject.name.Contains("Camera");
    if(!isPlaneOrCam){
        Debug.Log("Found gameobject: " + item.transform.gameObject.name);
        Debug.Log("Setting material to: " + mate.name);

        // These lines of code are used to double-check the size of the gameobject. 
        var renderer = item.GetComponentInChildren<MeshRenderer>();
        Debug.Log("LALALALALA RENDERER: " + renderer);
        var size =  item.GetComponentsInChildren<MeshRenderer>()[0].bounds.size;
        // Expect objects to be of size 1*1.5*1
        Debug.Log("Gameobject's size is: (x: " +size.x + ", y: " + size.y + ", z: " + size.z);
        [...] //<---- We'll get to this part next.
    } else {
        Debug.Log("Error: Name is: " + item.transform.gameObject.name);
    }
}
```

After having found only the renderers of the vases, we can now start to edit their design. To do this, we use the same approach as we used for manually changing the material of the gameobject in the menu; we define the variables material and sharedmaterial, which sets the new material of the gameobject.

```c#
List<MeshRenderer> rendererList = new List<MeshRenderer>();
foreach (MeshRenderer objectRenderer in item.transform.gameObject.GetComponentsInChildren<MeshRenderer>()) {
    if(objectRenderer.name != "ARPlane"){
        Debug.Log("Manual log: DADADADADADAD: " + objectRenderer.transform.parent.gameObject.name);
        objectRenderer.material = mate;
        objectRenderer.sharedMaterial = mate;
    } else {
        Debug.Log("Manual log: Error: Tried to change the color of the plane.");
    }
}
```

To see the results of this implementation, please see the video

[![IMAGE ALT TEXT](http://img.youtube.com/vi/_6v-6kp6raQ/0.jpg)](http://www.youtube.com/watch?v=_6v-6kp6raQ "Demo video")

 [HERE.](https://www.tutorialsandyou.com/markdown/ 'Please see the video I spent a lot of time on it')

### Technique 2 - [TODO: Name]
To achieve this, we first needed a new model, since breaking apart an object in unity is best done by replacing the object with another that looks the same but is split up to fall apart. We did this by taking our Amphora model and it's textures into Blender, and first using the 'solidify' function, so the vase model is hollow, and then running 'cell fracture' to create the shards. The result of that is seen below.

<img src="https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/Broken_Amphora_Model.png" height="360" />

We then importet this into unity and set it up as a prefab, where we could adress and edit each shard as their own prefab.

We create variables for our broken vase prefab and a new invisible plane that the vase can fall apart onto.

```c#
//broken vase prefab
    public GameObject brokenVase;
    public GameObject shatterPhysicsPlane;
```
Then, using raycast to cast a ray from the middle of the screen, and a check on the distance of the objects hit by the ray. We then check if that object is not null, not an AR default plane, and not a trackable either. This ensures that this code is only run when we actually 'bump' the phone into the vase. Once the phone is close enough to the vase, we spawn the broken vase prefab, and the invisible plane under it, and then destroy the original vase GameObject.

```c#
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
```

The broken vase and plane both have rigid bodies and mesh colliders. The plane is fixed in place. This then leads to the result below.

<img src="https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/technique_2.gif" height="420" />




### Technique 3 - Polishing [Gesture, Appearance]
Since Polishing a vase would have similar movement to moving a vase, we want to switch between the two, to avoid conflicts when the user is trying to do a specific action. To achieve this, we first created a button, called 'switchPolishingModeButton', a boolean polishingModeOn, and a method that the button would call, which flips the boolean and depending on the current state, changes the icon of the button. 

<img src="https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/gem.png" height="60" />
<img src="https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/move.png" height="60" />

_The polishing and move icons for the button. Each shows which mode you will switch to_

```c#
[SerializeField] private Button switchPolishingModeButton;
switchPolishingModeButton.onClick.AddListener(switchPolishingMode);

//Boolean to determine whether polishing mode is on or not
private bool polishingModeOn;

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
```

_The button setup and method to switch the polishingModeOn boolean and the button sprite_

```c#
if(polishingModeOn){
    //polishing code
} else {
    //move object code
}
```

_If statement to control whether the object is moved or polished_

Shown below: Every time we scrub across the selected object, we iterate over all Meshrenderers for the object and set the Glossiness up by 0.005 points, as long as this is still under 1. If we go above one, the object would become 'too glossy' and eventually become completely white.

```c#
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
```

<img src="https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/technique_3.gif" height="420" />

_A short gif presenting the function. We place two vases, then press the button to switch to polishing mode, and polish the left vase.

## Additional notes.
- Fixed a bug, where the editing options would toggle when pressing the AR Cursor. Object editing will no longer be enabled when pressing the cursor. 

## Conclusion
[TODO: conclusions of this weeks exercises]

## References
[TODO: used references]
