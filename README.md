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
