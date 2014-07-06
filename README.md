infrared-tv-control
===================

Control your tv from a pc with an arduino and an infrared led

The windows command line tool allows you to do the following:

**send samsung tv command**

ircontrol.exe -c 0xE0E040BF

**Available commands**

0xE0E040BF TV_Power                 
0xE0E0807F TV_Source                

0xE0E0C837 channel switch

0xE0E0D827 TV
0xE0E09768 HDMI1

0xE0E022DD favourite channel list

0xE0E048B7 TV_Chan_up               
0xE0E008F7 TV_Chan_down             
		  
0xE0E0E01F Vol+

0xE0E0D02F Vol-

0xE0E0F00F Mute

0xE0E058A7 TV Settings Menu

0xE0E01AE5 Return

0xE0E020DF TV_1                     
0xE0E0A05F TV_2                     
0xE0E0609F TV_3                     
0xE0E010EF TV_4                     
0xE0E0906F TV_5                     
0xE0E050AF TV_6                     
0xE0E030CF TV_7                     
0xE0E0B04F TV_8                     
0xE0E0708F TV_9                     
0xE0E08877 TV_0                     
		  
0xE0E0F807 TV_Info                  
0xE0E0B44B TV_Exit                  
0xE0E006F9 TV_Arrow_up              
0xE0E08679 TV_Arrow_down            
0xE0E0A659 TV_Arrow_left            
0xE0E046B9 TV_Arrow_right           
0xE0E016E9 TV_Enter                 
0xE0E036C9 TV_Red                   
0xE0E028D7 TV_Green                 
0xE0E0A857 TV_Yellow                
0xE0E06897 TV_Blue                  
0xE0E0C03F TV_Sleep  


## Optional command for the LEDs I attached to the arduino ##

**turn LED on**

ircontrol.exe -s ledon -c 0 

-c LED index (-1 for all)


**turn LED off**

ircontrol.exe -s ledoff -c 0 

-c LED index (-1 for all)


**turn LED on for a certain time**

ircontrol.exe -s ledonformillis -c 1 -l 5000 

-c LED index (-1 for all)

-l = milliseconds


**blink LED**

ircontrol.exe -s ledblink -c 5 -l 15 -m 90

-c LED index (-1 for all)

-l = blink interval in milliseconds

-m = repetitions


**LED Effects**

ircontrol.exe -s ledeffect -c 0 -m 5

-c effect index (0 = Knight Rider, 1 = Landing Zone)

-m = repetitions 
