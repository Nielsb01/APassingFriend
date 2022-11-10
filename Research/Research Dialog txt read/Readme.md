<h1>How to make dialog</h1>

<h2>Intro text</h2>
<p>When you interact with an npc the first thing you'll see is the intro text.
This text will be the first thing in you document. After the intro text put -- let the system know this is where the intro text ends.
</p>
<h2>Dialog choice</h2>
<p>
Dialog will consits of a choice made by the player, followed by a set of dialog from the npc.
To add a dialog choice for the player add an # infront of your player choice text.
If you want this option to end the dialog after being player add an $ after this dialog choice.
</p>
<h2>Dialog text</h2>
<p>
To add text to your dialog add a * with a number. under a dialog choice.
Every number is one screen of text for that dialog choice.
</p>

<h2>Example</h2>
<p>
This is how dialog can be made:

This is the intro to this dialog

-- 

#this is a dialog choice 1 that can be chosen by the player

*1 This is dialog that will be said by the npc for choice 1

*2 This is dialog that will be said by the npc for choice 1

*3 This is dialog that will be said by the npc for choice 1

*4 This is dialog that will be said by the npc for choice 1

*5 This is dialog that will be said by the npc for choice 1

#this is a dialog choice 2 that can be chosen by the player that will end the conversation$

*1 This is dialog that will be said by the npc for choice 2

*2 This is dialog that will be said by the npc for choice 2

*3 This is dialog that will be said by the npc for choice 2

*4 This is dialog that will be said by the npc for choice 2

*5 This is dialog that will be said by the npc for choice 2
</p>