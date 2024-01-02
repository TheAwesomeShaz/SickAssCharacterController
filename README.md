# SickAssCharacterController
Trying to make some mechanics like climbing system, slow down time archery etc. 
Using Unity's new Input System

## State Machine Branch

In this Branch we only focus on refactoring the code and adding a  state machine pattern to it
By the end of it might have a solid third person movement system atleast.
This can be then merged into main to then continue on with other stuff and 
the older implementations of parkour and Ledge Climbing can be deleted.

### Stuff in the Branch
- Currently the same stuff as Ledge and Climbing

### Todo
- Transfer the whole system into a State Machine Pattern (DONE)
- Make the Animations for Movement Work Fluidly (DONE)
- Make the Parkour actions into Different states (DONE)
- Add a FALLING state that is currently missing here (DONE)
- Remove ALL BOOLS if possible (Almost one)

### Note
- Do not STOP (well...)
