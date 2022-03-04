# Assignment 5 - ?? (graded)

**Date**: 06.03.2022

**Group members participating**: Oliver Benée Petersen, Thorben Christopher Schmidt

**Activity duration**: [TODO: Insert hours spend]

## Goal
We intend to implement the following three interaction techniques:

### Technique 1 - Theme plates. [Marker, Appearance]

For our first interaction technique, we intend to let the user to create general themes in their rooms using marker interaction. We want to let the user print out XR markers representative of a desired theme, and have the user tie placed artifacts to the marker. Any artifact, that is moved to this marker should change to the same material as that of the marker. Thus, the new object will fit the theme, that the user has decided for the area of the marker.  


[TODO: description]

### Technique 2 - Watch your step [Proximity, animate]

For the second interaction, we want to solidify the value and care, that is needed for ancient artifacts by implementing a system, that lets users knock the placed artifacts over when a user walks into them. We intend to implement this by measuring the relation in the form of the distance between the user and an artifact, and if the two are too close, we want to knock down the vase onto the ground. 
[TODO: description]

### Technique 3 - Polishing [Gesture, Appearance]

The third technique, we intend to implement the ability for users to polish placed artifacts by scrubbing a selected object with their finger. The scrubbing of the artifact is intended to simulate rubbing the artifact with a piece of cloth or other polishing device. This way, the user will be able to gesture at a selected object to polish it.
[TODO: description]

## Plan
[TODO: plan of this weeks exercises]

## Results
[TODO: results for this weeks exercises]



### Technique 1 - [TODO: Name]
 
[TODO: description]

### Technique 2 - [TODO: Name]
[TODO: description]

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
