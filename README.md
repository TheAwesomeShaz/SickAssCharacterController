# SickAssCharacterController
Trying to make some mechanics like climbing system, slow down time archery etc. 
Using Unity's new Input System

## Climbing Branch

In this Branch we only focus on movement and parkour, it includes motion matching stuff too.
By the end of it might have a solid third person movement system atleast.


### Stuff in the Branch
- Has all the stuff in Parkour branch plus:
- Added Ledge Limiting
- Added Ladder Climb
- Added Stop Running into Walls (Which broke the ladder Climb)
- Code is getting a lil FONKY like ![image](https://github.com/TheAwesomeShaz/SickAssCharacterController/assets/51862748/0c8186df-f390-4de5-82d8-c02949e8c0ab) 
- Make the Code Not FONKY by using a **State Machine Pattern**

### Todo
- Complete the climbing system however it is (it has circular dependencies so probably dont make that mistake)
- Maybe try adding the wallrun thing etc too? (Woah there fella u are too ambitious fix the structure first)

### Note
- This is an Experimental branch so stuff wont work well ofc
- Creating a Separate Branch for the State Machine Controller now
