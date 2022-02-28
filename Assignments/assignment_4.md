# Assignment 4 - Interaction techniques (graded)

**Date**: 28-02-2022

**Group members participating**: 
Oliver M. Benee Petersen: Implementing Exercise 1, and 2. Writing main report content. 
Thorben C. Schmidt: Fixing handin report images, solved issues with materials not changing. 

**Activity duration**: 
- About 18 hours on Exercise 1. 
- About 50 hours on Exercise 2. 
- About 6 more hours on fixing everything as various parts kept breaking over and over, and writing the report. 

These are estimates, i forgot to keep track of the time. 

## Goal
Formulate the new project idea. Implement object catalogue and assignment 4.

## Ancientize

We write this part as a note to ourselves on what we want to accomplish. We have decided to move away from the concept of BikeStorm due to the difficulty of implementation and it being outside of the scope of the course. 

The concept, titled "Ancientize" will let the user create a museum in their own home by adding ancient statues, sculptures, pillars, and pottery to create their own home museum with artifacts from a forgotten past. 

## Plan
Implement new gameobjects suitable for Ancientize. Implement an object catalog suitable for these gameobjects. For each exercise, plan an intended solution and discuss actual solution. 

## Results. 

### Exercise 1.

In this exercise, we intend to leverage the Unity UI capabilities in order to design an object catalog, such that the user can insert at least 3 different GameObjects. We are asked to do this in the form of a menu, or similar, that allows the creation of a menu, that lets us select multiple different Gameobjects, and be agnostic to screen orientation. We translate this to mean, that we are intended to create a dynamic menu, which allows for the addition of n number of objects, which demands some sort of scrolling in order to view multiple gameobjects, rather than creating a static menu for the options.

To solve this problem, we first start by considering the potential menu options. Regarding implementation, we went with the approach of using a linear menu along the bottom center of the screen. We place it in this location to allow the user to still see the area, they are going to be placing an object at on the screen. The use of a linear menu scrolling sideways occupies the least possible space, while still showing the potential menu options clearly on screen. Given our application area is XR, an argument could be made in favor of a radial menu, as it would allow for a closer relationship to gestures, and allows users to map a direction to a given object to place. However, these menus only excel, when the number of options is limited, and the task presented shows clear intent to use many gameobjects. Additionally, we also consider how a radial menu would block the entire screen, and make the user unable to see anything. Thus, to increase scalability by the highest possible factor, we intend to implement a linear menu with scrolling. When the user presses the menu button, we want to open a menu, that allows for the selection of new GameObjects. For reference of the image states see the images below: 

![MENUOFF](https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/menuoff.png)
![MENUON](https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/menuon.png)

Later on, we improved the menu, by changing the visuals to be appropriate to a landscape mode, as well as fitting the scrollbar neatly on top of the catalog.

![IMPROVEDMENU](https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/Screenshot_20220228-003614_AR22.jpg)
![IMPROVEDMENUOFF](https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/Screenshot_20220228-003608_AR22.jpg)

The menu also works in landscape mode. 

![ROTATEDIMPROVEDMENU](https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/Screenshot_20220228-023534_AR22.jpg)

We now further discuss the practical implementation of this. The menu is activated through the three-lines button seen on the image with the menu off. It runs a method openMenu() which enables the menu. 

```
void openMenu(){
    menuCanvas.SetActive(true);
    Debug.Log("MENUUUUU");
}
```

The menu itself consists of a canvas to contain all button options. For reference, see the image below, where an explaination of the gameobjects will follow. 

![GAMEOBJECTS](https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/menu_go_list.PNG)

We start with the menu canvas, which is a GameObject used to house the "image" that contains the menu. It could be said, that this canvas acts as a container for the menu and its options. Going one level down, we added a GameObject UIBackgroundScrollableList and a Scrollbar. The scrollbar is a standard Unity UI GameObject, but the UIBackgroundScrollableList is a gameobject, that lists the options, and contains a Scroll Rect gameobject, that allows for horizontal scrolling. For reference, see the image below: 

![SCROLLRECT](https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/scrollrect.PNG)

Additionally, we also add a HorizontalLayoutGroup to define the layout, and a ContentSizeFitter+LayoutElement to make every option fit. This solves the menu background, but now we need to populate the menu with content. We do so by creating a gameobject, we called GridContent. It has the components ContentSizeFitter, LayoutElement, and HorizontalLayoutGroup. These three components together create a horizontal layout of all menu options, that can be scrolled sideways, thanks to the UIBackgroundScrollableList. We can now start populating the menu with options. We do this by creating Button-gameobjects as children. These children have the component Layout Element to fix their size in the list layout, a button to be pressed, a Text gameobject with the menu option name on it (so the user can see what it means), and a MenuButtonManager to handle the options pressed. For reference, see the image below: 

![BUTTONOBJECTS](https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/buttonexamplewithmanager.PNG)

When a menu option is pressed, we trigger a MenuButtonManager, which selects the associated gameobject to the button and its manager, and closes the catalog menu:

```
public GameObject artifact;
[...]
void selectObject(){
    DataHandler.Instance.artifact = artifact;
    Debug.Log("ARTIFACT: " + artifact.name);
    // This line disables the menu. 
    this.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject.SetActive(false);
    isObjectNull = false;
}
```

Each MenuButtonManager contains a GameObject field, containing the associated artifact, it can instance. It also contains an IsObjectNull, which is used as a failsafe to guard against having the manager parse an empty gameobject. This is checked at runtime, but we serialized the field nonetheless for testing. Thanks to the fact, that each button has its own MenuButtonManager with an associated artifact, our solution can handle "a lot" of objects by just scaling up the scrollable menu, by adding new options for each new artifact. Each option is another Button in the GridContent with a MenuButtonManager. The MenuButtonManager can then be left responsible for converting the button's associated prefab into a gameobject.

Although the MenuButtonManager contains the gameobject, it is the responsibility of the RealObjectAdder to instantiate the artifact. Thus, to parse the data between each MenuButtonManager, we parse a DataHandler, containing the GameObject to be spawned. Its code is as follows:

```
  public GameObject artifact;
  private static DataHandler instance; 
  // Use this method anywhere to call properties from the data handler. 
  public static DataHandler Instance {
    get {
      // if this object doesn't exist, create one. 
      if(instance == null){
        instance = FindObjectOfType<DataHandler>();
      }
      // if it does exist, return it. 
      return instance;
    }
  }
```

To summarize, for every button press, the MenuButtonManager sets all new DataHandler instances to contain the artifact to be instantiated, and closes itself. Then, when the RealObjectAdder wants to place an object, it just instantiates the artifact held by the DataHandler. 

```
GameObject go = Instantiate(DataHandler.Instance.artifact, position2, transform.rotation);
```

Back in the MenuButtonManager, it then determines itself to close the menu after a button press. This is to mitigate the tight coupling between the menu and the RealObjectAdder. 

```
    // This line disables the menu. 
    this.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject.SetActive(false);
    isObjectNull = false;
```

After the menu has been closed, the user can place and edit gameobjects the same way they are used to previously, through the enablePlaceButton, and the placeObjectButton. The advantage of this solution is, that this allows loose coupling between the RealObjectAdder, the DataHandler, and the MenuButtonManager, but comes at the cost of high code complexity, but since we cannot access the RealObjectAdder directly from the MenuButtonManager, and thereby broadcast when an object has been selected, we have to run SetPlace() from the RealObjectAdder once it opens the menu. A disadvantage of our approach is, however, that the user can press the placeObjectButton before opening the menu and selecting a gameobject. This causes a NullPointerException normally, which causes AR tracking issues. Thus, to mitigate this, we set a deafult vase, being the Vase_Amphora. This completes the interaction loop with the object catalog. 

We argue, that this fulfills the requirements of allowing multiple objects to be placed by having a menu pop up from the bottom with a list of all available gameobjects to place. Given that the list can be infitetely expanded and scrolled in, it allows handling of "a lot" of objects. 

To see a video demonstrating the result, click [HERE.](https://youtu.be/Lztn7zc0sDE)

Seperate link: https://youtu.be/Lztn7zc0sDE

### Exercise 2.1

In this exercise, we want to extend the functionality of a gameobject through an animation added to a selected object. The intent is, that whenever a gameobject is selected, we want to add an option to create an effect on the gameobject, that is directly attached to it, and independent of the rest of the objects. In our case, we want to use animations to enhance the atmosphere of our program through particle effects. 

To solve this, we intend to create a button, which will appear once an object is seleted. This button, when pressed, should toggle a particle effect, which will add small "magical" particles to the artifact. We believe, that this enhance the experience by adding a the "mystery" and "ancient magic" theme to our project. 

To begin, we first created the GameObject "particle system", which can be attached to other gameobjects. This gameobject contains a component with unity's Particle System attached to it. For reference, see the image below:

![PARTICLES](https://gitlab.au.dk/au598997/ar21/-/raw/68a74cdd72642477ab20fa53613b33d2dcd1bc2d/Images/particles.PNG)

Originally, our idea was to add these particle effects as a component to the main artifact, which could be enabled and disabled at runtime using the built-in Play() and Pause() methods, that emitters have. By adding the component and removing it at runtime, we open up for changing the type of emitter to be used at runtime much easier (as a potential later feature), since we only have to instantiate the particleEffect, we wish to apply at runtime, then deleting it, rather than having to pre-determine a certain amount and types of particle effects we wish to add, and then modifying those existing components to what we need. 

Regarding the implementation, we started by creating an Animate() method for handling the animation. It starts by searching for any potential emitters on the gameObject already: 

```
// Do the animation. 
void Animate(){
  Debug.Log("ANIMATIOOOOOOONS");
  GameObject emitter = FindGameObjectInChildWithTag(editableObject, "particle");
  [...]
}
```

The FindGameObjectInChildWithTag method is based on [THIS](https://answers.unity.com/questions/893966/how-to-find-child-with-tag.html), which iterates through the specified gameobject and all of its children. When iterating, it looks through all child-gameobjects, and returns the first child with the tag specified. 

```
public static GameObject FindGameObjectInChildWithTag (GameObject parent, string tag){
  Transform t = parent.transform;
  for (int i = 0; i < t.childCount; i++) {
    Debug.Log("Found first child. " + t.GetChild(i));
    Debug.Log("Child has the tag: " + t.GetChild(i).tag);
    if(t.GetChild(i).gameObject.tag == tag){
      Debug.Log("Found: " + t.GetChild(i).gameObject.name);
      return t.GetChild(i).gameObject;
    }    
  }   
  Debug.Log("Found nothing.");
  return null;
} 
```

In the code snippet specified above, the method grabs the transform of the parent gameobject to find all children associated with the gameobject. For each of these children, it uses the GetChild() method to look through the child objects of the gameobject. If the child is found to have the tag specified in the method header to be looked for, it returns this child object as a gameobject. This allows us to browse through a gameobject to look for certain tagged gameobjects to be used. 

Back in the Animate() method, we can now look at if we got an emitter-tagged gameobject:

```
void Animate(){
  objects.Add(emitter);
  if(emitter){
    Debug.Log("found an emitter. destroying it.");
    Destroy(emitter);
  } else {
    Debug.Log("No emitter found. Creating a new one on: " + editableObject);
    var attachedEffect = particleEffect;
    var particle = Instantiate(attachedEffect, editableObject.transform);
    particle.tag = "particle";
  }
}
```

We start by adding the emitter to the list of gameobjects. This ensures, that the emitter is destroyed when deleteall is called. If we found an emitter on the editableObject, we destroy it, which will stop the particle effects. Otherwise, we Instantiate a child-gameobject to the editableobject, and tag it as "particle", which will allow the emitter to be found by FindGameObjectInChildWithTag() method. Thus, we have now created a toggle animation feature for our gameobjects. 

To see a video of this in action, please see [HERE.](https://youtu.be/lSrOE4lnVjA)

Seperate link: https://youtu.be/lSrOE4lnVjA

### Exercise 2.2

In this exercise, we are asked to add a button, such that the user can change the appearance of an object, i.e. changing between different colors or textures. To implement this feature, we intend to create a menu similar to the object catalog, which instead contains a catalog of all the available materials to be used. The advantage of this approach is, that it allows for a lot of code re-use in the implementation phase, while also keeping the menu visuals consistent. Similarly to the object catalog in Exercise 2.1, we can create the MaterialCatalog by creating a Canvas called MaterialCanvas, adding a gameobject, we call UIBackgroundScrollableList with a scrollrect for horizontal scrolling, and add a GridContent child-object with an image as the background, a ContentSizeFitter to make everything fit in the list, a layout element specifier, and a horizontal layout group for defining the horizontal layout.  

![MATERIALCATALOG](https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/MaterialCatalog.PNG)

Compared to Exercise 1, the main change is that we have replaced the MenuButtonManager with a MaterialMenuManager. This is a very similar script with the key difference being, that it presents the method SelectMaterial(), which returns the material associated with the option rather than the prefab associated with the catalog option. To summarize, the menu is opened with:

```
// Open the material catalog.
  void openMaterialMenu(){
  styleCanvas.SetActive(true);
  isMenuOpen = true;
  Debug.Log("MENUUUUU");
}
```

Every time a button is pressed, we run the button's associated material handler (seen on the material catalog editor image on the right), which parses a DataHandler with a material, and closes itself again. 

```
    public Material material;
    [...]
    void selectMaterial(){
        DataHandler.Instance.material = material;
        Debug.Log("MATERIAL: " + material.name);
        // This line disables the menu. 
        this.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject.SetActive(false);
        isObjectNull = false;
        GameObject.Find("AR Cursor").GetComponent<RealObjectAdder>().closeMenu();
    }
```
The MaterialHandler parses the material in each button's handler's field through the DataHandler to the RealObject adder. This is saved in the following field in RealObjectAdder:

```
  [SerializeField] private Material mate;
```

After an option has been selected in the material menu, we run CloseMenu(), which closes the menu from the RealObjectAdder, and calls ChangeMaterial(), which is the method changing the shader:

```
  public void closeMenu(){
  isMenuOpen = false;
  Debug.Log("Closing menu. Changing materials.");
  changeMaterial();
        
  // Re-enable outline since you pressed somewhere to add a material. 
  var outline = editableObject.GetComponent<Outline>().enabled = true;
}
```

Here, you notice the changeMaterial method, which is responsible for changing the material of the editableObject. First, it finds the material, it is supposed to change the artifact to, and the gameobject to be changed. 

```
  public void changeMaterial(){
    Debug.Log("CHANGING MATEIRALS.");
    mate = DataHandler.Instance.material;
    GameObject toBeChanged = FindGameObjectInChildWithTag(shellObject, "physicalObject");
```

Next, we check, that we aren't changing the material of the planes, we place objects upon, or the empty gameobject used as a placeholder for the editableObject: 

```
if(shellObject.name != "AR Default Plane" || shellObject.name != "New Game Object")
```

Now, that we have confirmed, we can change the material on an actual artifact, we create a list of all MeshRenderers on the artifact. This is necessary to make sure, that we change the correct renderer regardless of if the artifact is wrapped in a parent gameobject or not. Then we change all associated materials to the renderer. To avoid cases, where we want to get the material from the object in the future, we run the following: 

List<MeshRenderer> rendererList = new List<MeshRenderer>();
  foreach (MeshRenderer objectRenderer in shellObject.GetComponentsInChildren<MeshRenderer>()) {
    if(objectRenderer.name != "ARPlane"){
    objectRenderer.material = mate;
    objectRenderer.sharedMaterial = mate;
}

Using the material and shared materials, we are able to update the material of the artifact itself as well as all components in the future, which will be attached to the artifact, and let them also have this material be applied. Thus, we can now change the appearance of our artifact by using different materials. 

To see a video of this in action, please see [HERE.](https://youtu.be/Rh_FySMDAGs)

### Other neat additions, that don't fit in an exercise. 

In addition to implementing the exercises, we also improved our rotate and moveobject methods from the previous assignment by adding a check for a method "isPointerOverUI()". This method returns if the user is touching a UI component, which we use for checking that the user only drags the artifacts to places they can see on the screen (together with the screen bounds), and don't lose track of the object. 

```
public bool isPointerOverUI(Touch touch){
  PointerEventData eventData = new PointerEventData(EventSystem.current);
  eventData.position = new Vector2(touch.position.x, touch.position.y);
  List<RaycastResult> results = new List<RaycastResult>();
  EventSystem.current.RaycastAll(eventData,results);
  return results.Count > 0;
}
```

## Conclusion

We have now successfully expanded our application to not only allow for adding multiple objects, but also 
