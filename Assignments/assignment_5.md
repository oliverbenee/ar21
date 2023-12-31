# Assignment 5 - Interaction techniques (graded)

**Date**: 06.03.2022

**Group members participating**: Oliver Benée Petersen, Thorben Christopher Schmidt

**Activity duration**: 30 hours

Friday 04.03.2022: 6 hours

saturday 05.03.2022: 12 hours

Sunday 06.03.2022: 11 hours + 3 more hours waiting for gitlab to work again. Convenient timing. 


## Goal
We intend to implement the following three interaction techniques:

### Technique 1 - Theme plates. [Marker, Appearance]

For our first interaction technique, we intend to let the user to create general themes in their rooms using marker interaction. We want to let the user print out XR markers representative of a desired theme, and have the user tie placed artifacts to the marker. Any artifact, that is moved to this marker should change to the same material as that of the marker. Thus, the new object will fit the theme, that the user has decided for the area of the marker.  

To solve this problem, we intend to use XR marker interaction to allow users to determine a theme for their room. When the marker is registered by the user, the artifacts surrounding them should change their appearance in response to the marker they have placed on the ground. Thus, we register the themes to predefined markers, that can be used to change the appearance of surrounding objects to the user. We argue, that the use of markers to track the theme location of the area fulfills the criterias of using tracked image marker interaction; and the use of material changes fulfills the criteria of changing the appearance of the gameobject. The materials, we will be using includes the following:

| Volute Krater | Amphora | Hydria |
| ------ | ------ | ------ |
| <img src="https://gitlab.au.dk/au598997/ar21/-/raw/main/AR22/Assets/Prefabs/Nokobot/GreekTemple%20-%20Vases%20(Free)/04_Textures/Vases/Vase_Amphora_AlbedoTransparency.png" /> | <img src="https://gitlab.au.dk/au598997/ar21/-/raw/main/AR22/Assets/Prefabs/Nokobot/GreekTemple%20-%20Vases%20(Free)/04_Textures/Vases/Vase_Hydria_AlbedoTransparency.png" /> | <img src="https://gitlab.au.dk/au598997/ar21/-/raw/main/AR22/Assets/Prefabs/Nokobot/GreekTemple%20-%20Vases%20(Free)/04_Textures/Vases/Vase_VoluteKrater_AlbedoTransparency.png" /> |

### Technique 2 - Watch your step [Proximity, Animation]

For the second interaction, we want to solidify the value and fragility, that is inherent to ancient artifacts. To simulate this feeling, we want to implement a system, that breaks the placed artifacts when a user walks into them. Thus, if the user bumps into vases, they will be knocked over and destroyed. We intend to implement this by measuring the relation in the form of the proximity between the user and an artifact. If the two are too close, the user will knock over the vase, and it will break apart. This can be accomplished by raycasting within a short range. As for the animation, we intend to simulate the destruction of the artifact using rigid body animations, as these can be generated dynamically. We argue, that this will improve the user experience, as we now can create dynamically changing destruction animations. 

### Technique 3 - Polishing [Gesture, Appearance]

The third technique, we intend to implement the ability for users to polish placed artifacts by scrubbing a selected object with their finger. The scrubbing of the artifact is intended to simulate rubbing the artifact with a piece of cloth or other polishing device. This way, the user will be able to gesture at a selected object to polish it. We argue, that the use of rubbing as a gesture is appropriate, as it helps solidify, that the "dirtiness" of the artifact must be rubbed away to improve the appearance of the artifact. 

To implement this feature, we intend to register the rubbing touch gesture, and use raycast to measure which gameobject is being polished. As the user is polishing the artifact, we intend "shine up" the artifact by brightening up its material and making it more shiny. 

## Plan

For the implementation of these interaction techniques, we expect that the most demanding technique to implement will be the first. Thus, we will split up the workload, so that Oliver will focus on the implementation of the XR plate tracking, while Thorben will first focus on implementing the polishing feature, as this will help get work done fast. Once one technique is completed, the one who completed it will move on to work on the second technique, hopefully ending up with both of us working on it. 

## Results

In the following section, we will discuss the implementation and results of the creation of the interaction techniques. We will discuss the implementation of these individually as they pertain to each technique.

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

To see the results of this implementation, please see the video [HERE.](https://www.tutorialsandyou.com/markdown/ 'Please see the video I spent a lot of time on it')

[![IMAGE ALT TEXT](http://img.youtube.com/vi/_6v-6kp6raQ/0.jpg)](http://www.youtube.com/watch?v=_6v-6kp6raQ "Demo video")

### Technique 2 - Watch your step

To implement the ability to knock over placed artifacts and have them be destroyed, we need to create a shattered vase, which can then fall. To achieve this, we first needed a new model that can be broken apart, since breaking apart an object in unity is best done by replacing the object with another that looks the same but is split up to fall apart. This is because it is less performance intensive to instantiate a fixed prefab compared to generating a new one with the changes to be made. We did this by taking our Amphora model and its textures into Blender, and first using the 'solidify' function, so the vase model is hollow, and then running 'cell fracture' to create the shards. The resulting model can be seen below. 

<img src="https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/Broken_Amphora_Model.png" height="360" />

After creating this model, we can now import it into unity, and set it up as a prefab, where each shard of the broken vase is a child object of the parent gameobject, which is "the entire vase". We can now start coding the shattering physics. First, we create variables for our broken vase prefab and a new invisible plane that the vase can fall apart onto.

```c#
//broken vase prefab
    public GameObject brokenVase;
    public GameObject shatterPhysicsPlane;
```
Now that we have the two objects, that need to interact, we now need a trigger for the interaction. To simulate the feeling of the user walking into the object to knock them over, we use raycast to cast a ray from the middle of the screen, and a check on the distance of the objects hit by the ray. We then check if that object is not null, not an AR default plane, and not a trackable either. This ensures that this code is only run when we actually 'bump' the phone into the vase. Once the phone is close enough to the vase, we instantiate the broken vase prefab and its invisible plane, and then destroy the original vase GameObject. This effectively replaces the old vase with the broken one. 

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

Now that the vase has been instantiated, we can simulate the physics of the vase. Since it will not be used for raycasting, we can add a rigid body to the vase to simulate the vase falling to the ground and being destroyed. For reference, see the image below:

<img src="https://gitlab.au.dk/au598997/ar21/-/raw/a3048382722c9d25aa4810d8c55f3592935c32e6/Images/collider_and_body.PNG"/>

In addition to the rigid body, we add a mesh collider to ensure, that the vase will be stopped by the plane and not fall through it. Below the object, we spawn a fixed temporary plane with a mesh collider, which allows the vase to not fall through the ground. Using this approach, we get the result seen in the GIF below. This then leads to the result below.

<img src="https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/technique_2.gif" height="420" />


### Technique 3 - Polishing

To implement the polishing feature, we want to simulate a rubbing gesture on the vase itself. Since polishing a vase would have similar gestures to moving a vase, we want to switch between the two modes, to avoid conflicts when the user is trying to do a specific action. To achieve this, we first need to create a button that allows for the switch between polishing mode and movement mode. Then we need a toggle statement to switch between polishing and movement. First, we started by creating a 'switchPolishingModeButton', which will be used for the toggling:

<img src="https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/moveOrPolishButton.PNG">

When the polishing button is pressed, we make a call to the method switchPolishingMode(), which changes the state between polishing mode and movement mode. The change of modes does two things: First, it changes the sprite of the moveOrPolishButton to be representative of the mode they can switch back to. If the user switched to polishing mode, move mode will show up, and vice versa. The method call is handled by the following code: 

```c#
[SerializeField] private Button switchPolishingModeButton;
switchPolishingModeButton.onClick.AddListener(switchPolishingMode);

[...]

//Boolean to determine whether polishing mode is on or not
private bool polishingModeOn;

[...]

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

With this code, we have now toggled the polishing mode, and changed the mode sprite to be representative of the mode that the user can switch back to. Now back in the update function, we can change the dragging implementation: 

```c#
if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved && !isPointerOverUI(Input.GetTouch(0))){
    if(editableObject.name != "AR Default Plane" && editableObject.name != "Trackables"){
        [...]
        if(polishingModeOn){
            [...] // <--- we are in polishing mode
        } else {
            [...]
        }
```

Now as for the implementation of the scrubbing itself, we take the vase itself, and iterate over all meshrenders in it and all its child-objects. 

```c#
shellObject = hit.collider.gameObject;
[...]
foreach (MeshRenderer objectRenderer in shellObject.GetComponentsInChildren<MeshRenderer>()) {
    //[We will get to this part next]
}
```

Next, we increase the Glossiness of the artifact's own material by 0.005 points, as long as this is still under 1. If we go above one, the object would become 'too glossy' and eventually become completely white. Next, we also give the object some metallicness in its textures, which allows it to shine. Finally, we also set the smoothness to 1, which removes the sandiness of the gameobject. 

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

With this code implemented, as long as the user rubs over the artifact, it will increase in shininess, thus giving the feeling of being polished. To see this in action, please see the GIF below:

<img src="https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/technique_3.gif" height="420" />

_A short gif presenting the function. We place two vases, then press the button to switch to polishing mode, and polish the left vase._

## Additional notes.
- Fixed a bug, where the editing options would toggle when pressing the AR Cursor. Object editing will no longer be enabled when pressing the cursor. 
- GitLab has been causing a lot of issues for us with rejecting commits due to connection errors. It has been very hard to work and collaborate because of this. 

## Conclusion
We have now successfully expanded our application by adding three interaction techniques: users can now generate themes for a given room using theme plates to change the material and thereby theme of surrounding artifacts. This leverages markers and changing the appearance of the object. Users are now also able to destroy artifacts by walking into them. This leverages proximity and animations. Lastly, users can also polish their artifacts using rubbing gestures, to change the appearance of the artifacts.
