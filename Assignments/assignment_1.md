# Assignment 1 - Unity basics (ungraded)

**Date**: Feb. 4th, 2022

**Group members participating**: Oliver Benée Petersen, Thorben Christopher Schmidt

**Activity duration**: 10 hrs
- 4 Hrs Tuesday to reach task 3.4
- 1 Hrs Wednesday to complete task 3.4
- 2 Hrs Friday to complete task 3 & 4
- 3 Hrs finding out what we are supposed to write in the report and where. 

# Responses to "Explain" exercises. 

> 3.1.9: Explain the differences between altering your script and adding a parent object. What will be the easiest to maintain if the object changes (e.g. size, shape, ...)?

When we add a child object to a parent-object, we instance it relatively to the parent object itself. In context of the 3D Cube and 2D Plane, this means, that if we were to alter or move the Plane, the cube would also follow the plane. The position of the cube is relative to the plane. As an example, if we use absolute coordinates, if we were to change the attributes of cube, such as its size, we could have to change the coordinates of the position of the cube, to prevent it from clipping the plane. 

> 3.3.3: Explain the difference between Start() and Update().

Similar to the setup and loop in arduino applications, start() is meant for any code that needs to be run once at the start of the program. We specifically used start() to spawn the cube, its plane, and the associated MoveAround script. 
Meanwhile, update() can be used for checking and updating states at runtime, such as registering button presses, and performing their associated actions. For example, we use update() to listen for button presses from WASD, and perform their related movements on the 3D cube. If we were to use start() instead, we would only listen for WASD-movements during frame-1 of the scene, and never listen for new user input. 

> 3.5.1: Remove the collider of the 3D plane. Explain what happens. Specifically, how does Raycast work?

If we remove the collider from the plane, the plane no longer has collision physics. The 3D Cube will fall through the floor after being instantiated, and we can no longer instantiate new raycast-objects on the plane. The midair object will also fall through the floor (assuming it has mass). 

> 3.5.2: Add a rigid body to your midair-prefab. Explain what happens.

The prefab gains mass and drops down onto the plane.

> 3.5.3: With the rigid body on, try to change the collider, e.g. from a Box Collider to a Sphere collider. Explain what happens.

If we change the box-collider to a sphere collider, the midair-object rolls around like a sphere, since it does not respond to interactions with the shape of the object, but the shape of the collider. 

> 3.6.1: Edit your Raycast-prefab, such that some text is displayed above the object. Explain how the text looks when you place objects at different locations.

By default, the text will only face in the same direction, that of the angle 0 degrees. Thus, if we spawn the raycast-prefab at certain locations, the text will be unreadable. Likewise, if the prefab falls over it will also be unreadable. 

> 3.6.2: Add a script to the text, such that it always faces the main camera of the scene. Hint: LookAt. Explain how the text now looks. If the text is facing towards the camera, but in the wrong direction, how can you fix that?

Initially, the text was facing directly away from the camera at all times. It could be seen as "mirrored". We fixed this by rotating the text 180 degrees around the z axis. 

## Goal
Complete Exercise 1-4

## Plan
Implement the required code and functionality presented in Exercise 1-4

## Results
We have implemented the requested code for Exercise 1-4. We will now poceed to present a concept pitch for our augmented-reality project.

# BikeStorm
With _BikeStorm_, we want to inspire first-semester IT Product Design students for their first product design project - the _Pimp my Bike_ project.
In this project, the students are asked to design and add various electronics to their bicycle in order to improve it with respect to traffic, its visual appeal, the riding experience, or its other qualities. 
To inspire students in their brainstorming process, we propose _BikeStorm_ as a tool, where users can visualize various electronic attachments on their bicycle, with the idea of letting users see what these attachments would look like on the bike itself. 

![Sketch of the intended functionality](https://gitlab.au.dk/-/ide/project/au598997/ar21/tree/assignment_1/-/20220205_115600.jpg/ "sketch")

## Conclusion
We have created an application in Unity that allows us to place various basic objects and move them using the WASD keys. These objects can have various shapes, textures, and attributes, such as colors and text above. 
