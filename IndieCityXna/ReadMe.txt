=====================================
# Indie City Xna Code - Release 1.0 #
=====================================

This is the first GIT release of what we hope will become a larger collection of useful functionality for making porting from XBLIG to PC simple.

This release contains two folders:

 - IndieCityXna
 - BlankShell

================
# IndieCityXna #
================

This is a collection of code that could eventually be released as a number of assemblies. They provide core functionality that should be useful to anyone coming from XBLIG to PC and to people starting with XNA for the first time. This could possibly do with better organisation to seperate the stuff that is useful for porting to PC from 360 from the rest of the code.

Common:
=======

Display Manager
---------------
Class for handling resizable windows and full screen display modes with automatic bordering of 4:3 and 16:9 modes etc. Now takes the desktop aspect ratio into account when looking for suitable full-screen modes. Can also provide a matrix for use with SpriteBatch.Begin() calls that will scale your game down for lower resolution rendering

Input Manager
-------------
Class for handling keyboard, mouse and joypad input

Pointer
-------
Simple class for displaying a mouse pointer and handling translating the position from desktop to render space for easy in-game processing

Timer System
------------
Some helper functions for calculating frame rate and providing a simple time step multiplier that you can use to handle updating when your game isn't always running at a fixed framerate


IndieCity Specific:
===================

ICAchievements
--------------
Code for displaying "achievement unlocked" type messages when using the indiecity wrapper
This code has a usage example within the blankshell project.

ICLeaderboards
--------------
Code for displaying a leaderboard browser when using the indiecity wrapper (not finished in this version)
This code has a usage example within the blankshell project.

ICMessages
--------------
Code for displaying Pop-up messages for things like losing the connection to indiecity. Pretty simple code, but should be quite useful.
This code has a usage example within the blankshell project.


GameObject:
===========

Functionality for creating and managing simple 2d game objects, not really all that useful if you already have a game written.. but might be nice for XNA newbies - the example game uses these, so you might as well have the code :)


GameState:
==========

Provides simple game state management. Originally based on microsofts screen manager example but improved (?) quite a bit. Again, you probably already have this - but if not, this should get you started!


Save:
=====
this will be a complete cross-platform "evil list" tested save and load solution. It's currently half finished.


TileMap:
========
some code for drawing parallax tile maps. This ties into a map editor I've written and we'll release the source code for that later. For now it just draws a simple background so you can see stuff working in a "game" type example. Feel free to use it for your next game once the editor ships though! it's on us.



==============
# BlankShell #
==============
An incredibly simple "game" example that uses all of the above code, displays a menu, does some loading screen stuff and plays a "game". Contains an example indiecity setup, leaderboard display and achievement processign and display.


