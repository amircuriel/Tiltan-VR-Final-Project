# Tiltan-VR-Final-Project
In the project are 2 games:

1. The VR Game is a skit-shooting game where you need to shoot clay pots with your rifle. You move between stations by teleporting to a new fixed position.

2. The MR game is also a skit-shooting game, but now placed in your room, with PORTALS THAT ARE RANDOMLY PLACED ON YOUR ROOM'S WALLS, which shoot bouncing pots all over the place.

IMPORTANT: Since I did not have access to a VR Headset with the SDK required to scan & auto-generate a room, I created the entire MR game "blind", meaning I had no way to test it with the actual hardware.

Now, I did follow A LOT of tutorials and manuals to hopefully ensure it works with the correct headset, using components like MRUK, EffectMesh, the entire Meta Building Blocks library for passthrough etc. and more.

IN THE CASE IT DOES NOT AUTO-GENERATE A ROOM FOR YOU (which will hopefully not break the game), and instead generates nothing, go to the MRUK object in the hierarchy, and change the "Data Source" field to "Prefab".

<img width="800" height="400" alt="Instructions" src="https://github.com/user-attachments/assets/57af7c5d-c458-46dc-b52e-4bf318491465" />

This will ensure that a placeholder room will be used instead, and in that room you can test the "MR" gameplay I've created.

Thank you for your patience.
