# New Content Guide

This branch provides a template folder for a new day in the 30 Day of Microsoft Graph samples set.

- To add a new console app sample it is expected that your sample build upon the setup given in the [Base console application setup](./base-console-app) exercise. This gives all of the exercises a common base and reduces repetiton.
- The [Day NN - Template](./dayNN-template) folder is provided to give a skeleton into which you can slot the code and instructions for your exercise. Please make a copy of this template folder and rename the copy to match your exercise.
  - Ex. `Day 01 - Microsoft Graph`
- Where possible avoid adding too much code into `Program.cs` particularly in the `Main` method.  
- Avoid any naming conflicts with code from existing days, if you would like to build upon work or modify a helper from a previous day it is suggested that you provide the intended final code for that helper class to ensure that a user who may not have done that day is not negativly impacted.
- Please update the Table of contents for the Readme file in your exercise
- Please update the Table of contents in the root [Readme.md](Readme.md) to link to your exercise.