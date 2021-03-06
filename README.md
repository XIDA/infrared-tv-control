infrared-tv-control
===================

Control your tv (currently only Samsung TVs are supported) from a pc with an Arduino and an infrared led.   
         
You can also turn on / off the seven LEDs from the pc.
You can use the LEDs for different purposes.   
For example when a download finishes or an emails arrives.   


![](https://github.com/XIDA/infrared-tv-control/raw/master/assets/images/projectoverview/overview_01.jpg)

## Demo videos ##

**IR Control**  
[![Control your tv from the windows command line ](http://img.youtube.com/vi/n77kAdKRz4Q/0.jpg)](https://www.youtube.com/watch?v=n77kAdKRz4Q)

**Change LEDs**  
[![Control Arduino LEDs from the windows command line ](http://img.youtube.com/vi/wypyrgvDYOo/0.jpg)](https://www.youtube.com/watch?v=wypyrgvDYOo)


## Setup ##

1. Connect the Arduino to the IR LED and the 7 LEDs as described in the layout files
	- [pcb layout top](https://github.com/XIDA/infrared-tv-control/raw/master/assets/images/projectoverview/overview_02.jpg)
	- [pcb layout bottom](https://github.com/XIDA/infrared-tv-control/raw/master/assets/images/projectoverview/overview_03.jpg)
	
2. Download the latest release source code zip: [Latest release](https://github.com/XIDA/infrared-tv-control/releases/latest) and unzip it

3. Do the following with the files from the release:
	1. Install the Arduino libraries from the release found here \arduino\libraries   
([How to install libraries](http://arduino.cc/en/Guide/Libraries) (See section "Manual installation")
	2. Upload the Arduino sketch \arduino\arduino.ino to your Arduino    
   

4. Download the latest release of the command line utlity zip (commandlinetool.zip): [Latest release](https://github.com/XIDA/infrared-tv-control/releases/latest) and unzip it
5. Open a command line window and have a look at the available commands:  
[windows command line tool](https://github.com/XIDA/infrared-tv-control/tree/master/windows)
