# Assignment 1 - Unity basics (ungraded)

**Date**: [TODO: date]

**Group members participating**: Oliver Benée Petersen

**Activity duration**: [TODO: Insert hours spend]
- 4 Hrs Tuesday to reach task 3.4
- 1 Hrs Wednesday to complete task 3.4
- 2 Hrs Friday to complete task 3 & 4


Explanations:

3.1.9: Explain the differences between altering your script and adding a parent object. What will be the easiest to maintain if the object changes (e.g. size, shape, ...)?

3.3.3: Explain the difference between Start() and Update().
Similar to the setup and loop in arduino applications, start() is meant for any code that needs to be run at the start of the program, e.g. spawning the plane and cube, and update() is for any code, e.g. movement, or spawns, after starting, 

3.5.1: Remove the collider of the 3D plane. Explain what happens. Specifically, how does Raycast work?
Nothing spawns, as the ray cast from the camera, through the mouse position on the screen, doesn't collide with the plane anymore.

3.5.2: Add a rigid body to your midair-prefab. Explain what happens.
The prefab gains mass and drops down onto the plane.

3.5.3: With the rigid body on, try to change the collider, e.g. from a Box Collider to a Sphere collider. Explain what happens.
The object rolls around like a sphere, since that is the shape of the collider/hitbox.

3.6.1: Edit your Raycast-prefab, such that some text is displayed above the object. Explain how the text looks when you place objects at different locations.


3.6.2: Add a script to the text, such that it always faces the main camera of the scene. Hint: LookAt. Explain how the text now looks. If the text is facing towards the camera, but in the wrong direction, how can you fix that?
Text is facing away from the camera at all times. We fixed this by rotating it 180 degrees around the z axis





## Goal
Complete 1-4

## Plan
Complete 1-4

## Results
1-4 completed

## Conclusion
C# f*cking sucks

## References
404. References not found
