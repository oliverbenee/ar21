# Assignment 2 - AR Foundation basics (ungraded)

**Date**: 13.02.2022

**Group members participating**: Oliver Benée Petersen, Thorben Christopher Schmidt

**Activity duration**: [TODO: Insert hours spend]

08.02.22: 6 Hours Excercise 1

09.02.22: 3 Hours set up raycast with default AR planes

10.02.22: 5 hours stuck on not working raycast

11.02.22: 6½ hours getting raycast etc to work


# Responses to "Explain" exercises. 

> 1.2.3: what you see in the application. What are being tracked in the physical world?

Light yellow Areas with black borders, visualizing the found AR planes overlaid onto the real world.

![ALT](https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/Screenshot_20220209-114831_My_project.jpg)


> 2.1.2: what is the size of your object in the physical world? How does unity's units relate to real world units?

20 units or 15cm when compared by spawning it next to a real world object. This would mean 1 unit is roughly 0.75cm

![ALT](https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/Screenshot_20220211-163419_AR22.jpg)

> 2.1.3: what happens if you insert a cube on top of a cube?

The cast Ray hits the existing cube and instantiates a cube at the other cube's surface

> 2.2.2: why your objects fall through the ground?

Rigid bodies are affected by gravity, but do not have a plane to collide with, which would stop them.

> 2.2.3: Try to place an object on top of another object. Explain what happens and why?

The objects land on each others and fall over

> 2.2.4 p1: Explain the difference between the two different raycast functions (Unity's build-in and AR Foundation's)?
When will you use each function?
Can you combine them in a meaningful way?
If you want to use gravity in your project, how can you change the design of the debug planes to look nicer?

Unity's Raycast can be used for defaultARPlanes, which allow physics use, AR foundations on the other hand work without AR default planes, and can thus be used for anything where an object doesn't require  physical interaction with anything. In our case, we can use the AR foundation raycast, to spawn the object at a specific spot with our indication marker, and then have the object collide with the debug planes, which are invisble as we removed the material from the mesh renderer and removed the line renderer. 

If we delete the mesh renderer entirely and not just its material, collision stops working. So we have to disable it. 
If we delete the line renderer's materials or try to disable it, the black lines from the planes are still visible. We are not sure why. But collision still works if we remove the Line Renderer entirely. 

> 2.5.3: Explain and demonstrate the objects you have created.

We have decided to use a simple bike model. It is a bike

![ALT](https://gitlab.au.dk/au598997/ar21/-/raw/main/Images/Screenshot_20220211-181639_AR22.jpg)

> 3.1.4:what happens when you move the physical image. 

> 3.2.1: how does the two MO's look compared to each other? Why do they look this way?

> 3.3.3: does this give you additional possibilities to adding objects to your world? How? Where? Why?





## Goal
[TODO: goal of this weeks exercises]

## Plan
[TODO: plan of this weeks exercises]

## Results
[TODO: results for this weeks exercises]

## Conclusion
[TODO: conclusions of this weeks exercises]

## References
[TODO: used references]
