Welcome to Unity, 470 students

For this potential fields lab, you don't need to know very much about unity, but I'll go over a few things to make your life easier.

For starters, I'd recommend you set the code editor to Monodevelop. Do this by navigating to Edit -> Pereference -> External Tools, and then set External Script Editor to MonoDevelop. The Default, Visual Studios can be buggy.

In Unity there are things called "GameObjects" (Any selectable object in the scene) and "scripts" (C# or Javascript code attached to those gameobjects)
You can open scripts by either double clicking on the script in the "project" window or by double clicking on the script in the inspector when the gameobject is selected.

Each GameObject has a "transform", which contains its position, rotation, and scale information. Click on any of the shapes in the scene, you should be able to then see the transform variables in the "Inspector Tab".
You can move Gameobjects by selecting them, pressing "W" and then using the inscene colored arrows attached to it.
You can duplicate GameObjects by pressing "Ctrl" + "D"

I have written two relavant start scripts for you to work with. One is called "field" which should be attached to all of the shapes in the scene except the sphero. Click on one and in the inspector window, you can see its public variables. These variables have no effect on anything and you don't even have to use them if you don't want. Although I would recommend you at least use the fieldType variable, so you can distinguish between your attractive and repulsive fields.
Do not modify anything in the Field Script. It is simply there to serve as tags for your things.

The script you will really care about is the "Robot Controller".
I have provided a bunch of base functions inside that will let you control it manually as well as access all of the "fields" in the scene in a bunch of different ways. Those fields do nothing right now. You can get the location(Vector3 (x,y,z)) of the field's Gameobject by calling "fieldA.getLocation();"

In the update function(which gets called every frame) you will need to implement your potential fields. Inside it you should see these lines of code:
Vector3 toMove = getManualInput ();
move (toMove);

Between them you will need to figure out what Vector3 (x,y,z) you will need to add to the "toMove" variable. The Y variable in the Vector3 isn't important since we aren't making it fly or jump. 

You are required to use the current "move()" function as it is. It simply pushes the sphero in the given direction so that it will have angular and forward momentum. its meant to replicate the sphero physics. For instance, if I pass in "move(new Vector3(1,0,1))" it will be pushed in the NorthEast direction.

There is also a public global "speed" variable. Use it if you like.

If you set any variables in the inspector, it will override whatever default values you have in the script itself.

The Manual control works by having the A&D keys rotating the robot's direction clockwise and counterclockwise. The W&S keys then move it forward and backwards. There is an arrow attached to it to show where it is currently pointed.

You can print out to the console window for debugging purposes with "Debug.Log("Print this out");"

In Order to make it so your robot with collide with an object, click on it and check the box in the Collider (Sphere, Box, or capsule Colliders)

Good luck and let me know if you have any questions:
Matthew Johnson
nosnhojdm@gmail.com

 

