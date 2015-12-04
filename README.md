# BrainFuzz
An extention to the brainf*** language.

##What is it?

BrainFuzz is a brainf*** interpreter written in C# that supports serveral new extentions.

##A bit about brainf***

Brainf*** is an esoteric programming language which consists of just 8 characters. It operates on an array of cells all initialized to 0 and a pointer.

Op | Function
--- | ---
\> | Move pointer Right
\< | Move pointer Left
\+ | Increase value of Cell at pointer
\- | Decrease value of Cell at pointer
\[ | If cell at pointer's value is not zero, continue to next instruction otherwise jump command after corresponding ']'
\] | If cell at pointer's value is zero, continue to next instruction otherwise jump command after corresponding '['
\. | Print value of cell at pointer as a character
, | Read in a character to the cell at pointer

##BrainFuzz Extensions

BrainFuzz adds the followind new commands:

Op | Function
--- | ---
\( | If cell at pointer is 0 jump to the corresponding ')'
\) | Jump to point of '('
\! | Inverts if and while conditions, also inputs a string into cells if used before ' " '
; | Reads a number from input and puts it's numerical value in cell at pointer
\: | Outputs the current cells value as a number NOT ascii e.g. cell is 65, outputs 65 not 'A'
" | Outputs everything up until corresponding ' " '
\^ | Saves current cells value in a temp register
= | Current cell becomes the value in the temp register
