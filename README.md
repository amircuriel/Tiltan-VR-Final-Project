# Tiltan-VR-Final-Project
In the project are 2 games, which I've built to the best of my ability to answer all sections of the final brief:

1. The VR Game is a skit-shooting game where you need to shoot flying clay pots with your rifle. You move between stations by teleporting to a new fixed position. The pots are launched at random pitchs and angles, and you need to shoot at least 10 out of 30 to win the game. **Object Interactivity** includes pressing a "physical" button and wielding/manipulating a physical gun.

3. The MR game is also a skit-shooting game like the first, but now taking place in your room. The MR elements include manually placing the start button on your table, and randomly generating portals on the room's walls for the pots to come out of (using the Meta MR SDK plugins and components, detailed below). The pots also bounce, which leads to a fun and different experience than to first game.

Both game scenes are found in the "Scenes" folder in the project root. Unfortunately, I hadn't the time to add a main menu or a reset button, so simply restart the game in the editor to play again.

IMPORTANT: Since I did not have access to a VR Headset with the SDK required to scan & auto-generate a room, I created the entire MR game "blind", meaning I had no way to test it with the actual hardware.

Now, I did follow A LOT of tutorials and manuals to hopefully ensure it works with the correct headset, using components like MRUK, EffectMesh, the entire Meta Building Blocks library for passthrough etc. and more.

IN THE CASE IT DOES NOT AUTO-GENERATE A ROOM FOR YOU (which will hopefully not break the game), and instead generates nothing, go to the MRUK object in the hierarchy, and change the "Data Source" field to "Prefab".

<img width="800" height="400" alt="Instructions" src="https://github.com/user-attachments/assets/57af7c5d-c458-46dc-b52e-4bf318491465" />

This will ensure that a placeholder room will be used instead, and in that room you can test the "MR" gameplay I've created.

Thank you for your patience.
