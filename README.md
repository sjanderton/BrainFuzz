# BrainFuzz
An extention to the brainfuck language.

##What is it?

BrainFuzz is a brainfuck interpreter written in C# that supports serveral new extentions.

##A bit about brainfuck

Brainfuck is an esoteric programming language which consists of just 8 characters. It operates on an array of cells all initialized to 0 and a pointer.

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

BrainFuzz adds the following new commands:

Op | Function
--- | ---
\( | If cell at pointer is 0 jump to the corresponding ')'
\) | Jump to point for '('
\! | Inverts if and while conditions, also inputs a string into cells if used before ' " '
; | Reads a number from input and puts its numerical value in cell at pointer
\: | Outputs the current cell's value as a number NOT ascii e.g. cell is 65, outputs 65 not 'A'
" | Outputs everything up until corresponding ' " '. Does not affect cells.
\^ | Saves current cell's value in a temp register
= | Current cell becomes the value in the temp register
