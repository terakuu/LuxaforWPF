# LuxaforWPF

Simple WPF application for controlling Luxafor Led

Application listens for changes in selected text file. It reads the first line of selected file when file is modified. Text file can be of course be modified from different ways so this is simple way to integrate Luxafor to other systems. Luxafor color can also be set manually for example imply do not disturb status. 

Color buttons (buttons under Set Color):
- set color for the led and the ellipse, and stops listening the file.

Listen File button:
- starts to listen the file again (if you set color with color button, you need to press Listen File  button if you want to start listening changes in the file again)
-changes in the file change color/ blinking of ellipse and Luxafor - device.

Turn off button:
-Set Luxafor led color to 0,0,0

File:
Enable Device:
-find Luxafor device
-Shows Messagebox  w/ error message, if no luxafor devices are connected to computer

Select File:
-Select file  to listen for changes
-application remembers your selection,  so no need to select the file again and again when starting the application next time.

Syntax for text file:
r,g,b, speed, repeatCount
examples:
set color:
255,0,40
blink:
255,0,40, 10, 40
status request:
status

Status request: writes status of the device to result.txt-file (if it doesnt exist, it is created)  located in same folder as the file it listens.

example: 
201605021200221234: Color : 255,0,0. Status : Not listening phone.
