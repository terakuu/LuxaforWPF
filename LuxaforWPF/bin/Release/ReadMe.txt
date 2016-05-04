
Application listens for changes in selected txt- file. It reads the first line of selected file when file is modified. 

Color buttons (buttons under Set Color ):
- set color for the led and the ellipse, and stops listening the file.

Listen File- button:
- starts to listen the file again ( if you set color with color button, you need to press listen file - button if you want to start listening changes in the file again)
-changes in the file change color/ blinking of ellipse and Luxafor - device.

Turn off- button
-Set Luxafor- devices color to 0,0,0

File:
Enable Device:
-find Luxafor-device
-Shows Messagebox  w/ error message, if no luxafor devices are connected to computer

Select File:
-Select file  to listen for changes
-application remembers your selection,  so no need to select the file again and again when starting the application next time.


Syntax for txt-file:
r,g,b ,speed, repeatCount
examples:
set color:
255,0,40
blink:
255,0,40, 10, 40
status request:
status

status request : writes status of the device to result.txt-file (if it doesnt exist, it is created)  located in same folder as the file it listens.

example: 
201605021200221234 : Color :255,0,0 . Status :Not listening phone.