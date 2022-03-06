# Assignment 5 - ?? (graded)

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


[TODO: description]

### Technique 2 - Watch your step [Proximity, animate]

For the second interaction, we want to solidify the value and fragility, that is inherent to ancient artifacts by implementing a system, that breaks the placed artifacts when a user walks into them. We intend to implement this by measuring the relation in the form of the distance between the user and an artifact, and if the two are too close, the vase will break apart. 

### Technique 3 - Polishing [Gesture, Appearance]

The third technique, we intend to implement the ability for users to polish placed artifacts by scrubbing a selected object with their finger. The scrubbing of the artifact is intended to simulate rubbing the artifact with a piece of cloth or other polishing device. This way, the user will be able to gesture at a selected object to polish it.

## Plan
[TODO: plan of this weeks exercises]

## Results
[TODO: results for this weeks exercises]



### Technique 1 - [TODO: Name]
 
[TODO: description]

### Technique 2 - [TODO: Name]
To achieve this, we first needed a new model, since breaking apart an object in unity is best done by replacing the object with another that looks the same but is split up to fall apart. We did this by taking our Amphora model and it's textures into Blender, and first using the 'solidify' function, so the vase model is hollow, and then running 'cell fracture' to create the shards. The result of that is seen below.

<img src="https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/Broken_Amphora_Model.png" height="360" />

We then importet this into unity and set it up as a prefab, where we could adress and edit each shard as their own prefab.

We create variables 

<img src="https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/technique_2.gif" height="420" />


```c#
//broken vase prefab
    public GameObject brokenVase;
    public GameObject shatterPhysicsPlane;
```

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

_A short gif presenting the function. We place two vases, then press the button to switch to polishing mode, and polish the left vase._

## Conclusion
[TODO: conclusions of this weeks exercises]

## References
[TODO: used references]
