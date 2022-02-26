# Assignment 4 - Interaction techniques (graded)

**Date**: [TODO: date]

**Group members participating**: [TODO: insert names]

**Activity duration**: 
- About 18 hours on Exercise 1. 

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

The menu is activated through the "+" button seen on the image with the menu off. It runs a method openMenu() which enables the menu. 

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

This solves the menu background, but now we need to populate the menu with content. We do so by creating a gameobject, we called GridContent. It has the components ContentSizeFitter, LayoutElement, and HorizontalLayoutGroup, which creates a horizontal layout of all menu options, in a horizontal layout, that can be scrolled sideways, thanks to the UIBackgroundScrollableList. We can now start populating the menu with options. We do this by creating Button-gameobjects as children. For reference, see the image below: 

![BUTTONOBJECTS](https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/buttonexamplewithmanager.PNG)

When a menu option is pressed, we trigger a MenuButtonManager:

```
public GameObject artifact;

void selectObject(){
    DataHandler.Instance.artifact = artifact;
    Debug.Log("ARTIFACT: " + artifact.name);
}
```

Each MenuButtonManager contains a GameObject field, containing the associated artifact, it can instance. It also contains an IsObjectNull, which is used as a failsafe to guard against having the manager parse an empty gameobject. This is checked at runtime, but we serialized the field nonetheless for testing. Thanks to the fact, that each button has its own MenuButtonManager with an associated artifact, our solution can handle "a lot" of objects by just scaling up the scrollable menu, by adding new options for each new artifact, and letting the MenuButtonManager be responsible for converting the button's associated prefab into a gameobject.

Although the MenuButtonManager contains the gameobject, it is the responsibility of the RealObjectAdder to instantiate the artifact. Thus, to parse the data between each MenuButtonManager, we parse a DataHandler, containing the GameObject to be spawned. 

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

To summarize, for every button press, the MenuButtonManager sets all new DataHandler instances to contain the artifact to be instantiated, and closes itself. Then, when the RealObjectAdder wants to place an object, it just instantiates the artifact held by the DataHandler: 

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

This now fulfills the requirements of allowing multiple objects to be placed by having a menu pop up from the bottom with a list of all available gameobjects to place. Given that the list can be infitetely expanded and scrolled in, it allows handling of "a lot" of objects. 

To see a video demonstrating the final result, click [HERE.](https://youtu.be/Lztn7zc0sDE)

Seperate link: https://youtu.be/Lztn7zc0sDE

### Exercise 2.1

In this exercise, we want to extend the funcitonality of a gameobject through an animation added to a selected object. The intent is, that whenever a gameobject is selected, we want to add an option to create an effect on the gameobject, that is directly attached to it, and independent of the rest of the objects. 

To solve this, we intend to create a button, which will appear once an object is seleted, which 

[TODO: results for this weeks exercises]

## Conclusion
[TODO: conclusions of this weeks exercises]

## References
[TODO: used references]
