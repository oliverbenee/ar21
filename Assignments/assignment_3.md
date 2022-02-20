# Assignment 3 - Starting on your app (graded)

**Date**: 20.02.2022

**Group members participating**: Oliver Benée Petersen, Thorben Christopher Schmidt

**Activity duration**: 36 hours

15.02.2022: 2 hours Exercise 1 and adding buttons

17.02.2022: 6 hours Object selection & Fixing Excercise one

18.02.2022: 5 hours Solving excercise 2

19.02.2022: 13 hours fixing excercise 2 and Exercise 3.1

20.02.2022: 10 hours Fixing excercise 3.2

# Responses to "Explain" exercises. 

> 1.1.2: Explain how you fulfill the above requirements.

Regarding the button to enable adding an Object, we created an 'enablePlaceButton'. This button calls the function 'enablePlace', which toggles the state of the cursor and 'confirmButton' (used to place objects). The 'placeObject' button has not changed since excercise2 but uses raycast to instantiate a new object, also added to a list of GameObjects. To this MO we add a hidden component outline, which is later used for highlighting of selected MOs. We then used raycast to add objectselection. When an MO is selected via raycast, it is highlighted and saved in a GameObject field for later editing.

For the button to clear all objects, we iterate the GameObjects list and set their ActiveState to false, which makes all the objects invisible. We then clear the list of GameObjects. This technically doesn't delete the MOs, but prevents nullpointer exceptions we would otherwise get using the Destroy() function, since our MO is made up of multiple objects.

We reused our cursor raycast for the indication marker

We reused AR default planes for this, but made them invisible, for the user experience. They work more consistently in our case and help with debugging. The user can also not see them  since the mesh- and line-renderers are turned off.

![ALT](https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/Screenshot_20220218-003837_AR22.jpg)


> 2.1.3: Explain your solution

object selection is handled with raycast, whenever the ray collides with an MO, this is saved to a GameObject field. And the MO is outlined. 

The outline is based on the QuickOutline library. The library handles the drawing and updating of the outline for us. The benefit of this is that we can add the outline as a component to the MO.

An alternative to this would be changing the shader of the MO when selected. And changing it back when deselected.

> 2.2.2: Explain your solution.

This is handled using GameObject.setActive() together with the outline, which means it becomes visible when an object is selected. The selected MO can then be deleted by pressing the button. This checks that we have selected something and not selected the AR default plane. Then it is done as explained above, but the object is not removed from the GameObject list.

> 3.1.2: Explain your solution.

We assume that the user only uses one finger to drag MOs. We use raycast to measure the point on an AR plane that the user is tapping on and teleport the MO to that position.

> 3.2.2: Explain your solution. You must also - in detail - explain the math behind your solution (including some kind of sketch).

We assume that the first to touch x- and y-coordinates A1 and B1, which we use to calculate an vector by D1 = A1-B1. We do the same after detecting movement, with A2 and B2, which we use to calculate vector D2 = A2 - B2. We then turn D1 and D2 into angles using an atan2 function and do angle2-angle1=angle3 to calculate how far we should move the object. Wo then transform the object using rotation(angle3) in the axis perpendicular to the plane the object is placed on.
![ALT](https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/1200px-Atan2definition.svg.png)


## Goal & Plan
Build a functioning app, allowing placement, deletion, movement, and rotation of MOs.

## Results
Our app allows placement, deletion, movement, and rotation of MOs.

## Conclusion
We have successfully created an application that allows placement and basic manipulation of our MO.
