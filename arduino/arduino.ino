#include <Thread.h>
//My simple Thread
Thread myThread = Thread();

#include <IRremote.h>
IRsend irsend;

#include <CmdMessenger.h>
// Attach a new CmdMessenger object to the default Serial port
CmdMessenger cmdMessenger = CmdMessenger(Serial);
// This is the list of recognized commands. These can be commands that can either be sent or received. 
// In order to receive, attach a callback function to these events
enum Commands
{
  kSetSamsung,
  kSetSamsungResponse, 
  kLEDEffect,
  kLEDEffectResponse,
  kLEDOnForMillis,
  kLEDOnForMillisResponse,  
  kLEDOn,
  kLEDOnResponse,
  kLEDOnRange,
  kLEDOnRangeResponse,  
  kLEDOff,
  kLEDOffResponse,  
  kLEDBlink,
  kLEDBlinkResponse,
  kResponse,  
  kStatus,
};

// I connected 7 LEDs to the arduino
// if you just want to send IR, disable it here
#define USE_LEDS true
int led1 = A3;
int led2 = A5;
int led3 = A4;
int led4 = 2;
int led5 = 4;
int led6 = 5;
int led7 = 6;

int ledPins[] = {led1, led2, led3, led4, led5, led6, led7};
int ledsAmount = 7;

int currentEffectRepetitions = 1;
int currentEffectValue1;
int currentEffectValue2;

void setup() {
  Serial.begin(9600);
  
  // Adds newline to every command
  cmdMessenger.printLfCr();   

  // Attach my application's user-defined callback methods
  attachCommandCallbacks();

  // Send the status to the PC that says the Arduino has booted
  // Note that this is a good debug function: it will let you also know 
  // if your program had a bug and the arduino restarted  
  cmdMessenger.sendCmd(kStatus,"Arduino has started!");  
  
  #if USE_LEDS
    setupLEDs();
    startLedEffectKnightRider();
  #endif
}

void loop() {
   // Process incoming serial data, and perform callbacks
  cmdMessenger.feedinSerialData();  
  
  // checks if thread should run
  if(myThread.shouldRun()) {
    myThread.run();
  }
}

void setupLEDs() {
  pinMode(led1, OUTPUT);  
  pinMode(led2, OUTPUT);
  pinMode(led3, OUTPUT);  
  pinMode(led4, OUTPUT);
  pinMode(led5, OUTPUT); 
  pinMode(led6, OUTPUT);
  pinMode(led7, OUTPUT);
}

// Callbacks define on which received commands we take action
void attachCommandCallbacks() {
  // Attach callback methods
  cmdMessenger.attach(OnUnknownCommand);
  cmdMessenger.attach(kSetSamsung, OnSetSamsung);
  cmdMessenger.attach(kLEDEffect, OnLedEffect);
  cmdMessenger.attach(kLEDOnForMillis, OnLedOnForMillis);  
  cmdMessenger.attach(kLEDOn, OnLedOn);    
  cmdMessenger.attach(kLEDOnRange, OnLedOnRange);      
  cmdMessenger.attach(kLEDOff, OnLedOff);     
  cmdMessenger.attach(kLEDBlink, OnLedBlink);    
}

// Called when a received command has no attached function
void OnUnknownCommand() {
  cmdMessenger.sendCmdStart(kResponse);  
  // first argument is for success or not
  // 1 = success
  // 2 = failure
  cmdMessenger.sendCmdArg(0);
  cmdMessenger.sendCmdArg("Command without attached callback");
  cmdMessenger.sendCmdEnd();    
}

void OnSetSamsung() {
  
  String value = cmdMessenger.readStringArg();
  
  char string[value.length() + 1];
  value.toCharArray(string, value.length() + 1);
  unsigned long n = strtoul(string, NULL, 0);
  
  irsend.sendSamsung(n, 32);
  
  cmdMessenger.sendCmdStart(kSetSamsungResponse);  
  // first argument is for success or not
  // 1 = success
  // 2 = failure
  cmdMessenger.sendCmdArg(1);
  cmdMessenger.sendCmdArg(value);
  cmdMessenger.sendCmdArg(n);
  cmdMessenger.sendCmdEnd();      
}

void OnLedEffect() {
 if(! USE_LEDS) { 
   sendLEDsDisabledMessage();
   return;
 }
 
 turnLEDsOff();
 
 int effectIndex = cmdMessenger.readInt16Arg();
 currentEffectRepetitions = cmdMessenger.readInt16Arg(); 
 
 switch(effectIndex) {
   case 0:
     myThread.onRun(ledEffectKnightRider);
     myThread.enabled = true;
   break;
   case 1:
     startLedEffectLandingZone();
   break;   
 }
  
 cmdMessenger.sendCmdStart(kLEDEffectResponse);  
 // first argument is for success or not
 // 1 = success
 // 2 = failure
 cmdMessenger.sendCmdArg(1);
 cmdMessenger.sendCmdArg(effectIndex);

 cmdMessenger.sendCmdEnd();      
}

void OnLedOnForMillis() {
 if(! USE_LEDS) { 
   sendLEDsDisabledMessage();
   return;
 }
 
 // led index
 currentEffectValue1 = cmdMessenger.readInt16Arg();
 
 int millisToKeepOn = cmdMessenger.readInt16Arg(); 
 
 switch(currentEffectValue1) {
   case -1:
     // turn on all leds
     turnLEDsOn();
   break;
   default:
     turnLEDOn(currentEffectValue1);
   break;   
 }
 
 myThread.setInterval(millisToKeepOn); 
 myThread.enabled = true;  
 myThread.onRun(ledOnForMillisOver);
    
 cmdMessenger.sendCmdStart(kLEDOnForMillisResponse);  
 // first argument is for success or not
 // 1 = success
 // 2 = failure
 cmdMessenger.sendCmdArg(1);
 cmdMessenger.sendCmdArg(currentEffectValue1);

 cmdMessenger.sendCmdEnd();      
}

void ledOnForMillisOver() {
 myThread.enabled = false;  
  
 switch(currentEffectValue1) {
   case -1:
     // turn off all leds
     turnLEDsOff();
   break;
   default:
     turnLEDOff(currentEffectValue1);
   break;   
 }  
}

void OnLedOn() {
 if(! USE_LEDS) { 
   sendLEDsDisabledMessage();
   return;
 }

 // led index
 currentEffectValue1 = cmdMessenger.readInt16Arg();

 switch(currentEffectValue1) {
   case -1:
     // turn on all leds
     turnLEDsOn();
   break;
   default:
     turnLEDOn(currentEffectValue1);
   break;   
 }
    
 cmdMessenger.sendCmdStart(kLEDOnResponse);  
 // first argument is for success or not
 // 1 = success
 // 2 = failure
 cmdMessenger.sendCmdArg(1);
 cmdMessenger.sendCmdArg(currentEffectValue1);

 cmdMessenger.sendCmdEnd();      
}

void OnLedOnRange() {
 if(! USE_LEDS) { 
   sendLEDsDisabledMessage();
   return;
 }

 // led start index
 currentEffectValue1   = cmdMessenger.readInt16Arg();
 currentEffectValue2   = cmdMessenger.readInt16Arg(); 
 int millisToKeepOn    = cmdMessenger.readInt16Arg();
 
 if(currentEffectValue1 <= 0 || currentEffectValue1 >= ledsAmount) { 
   sendErrorMessageForType(kLEDOnRangeResponse);
 }
 
 if(currentEffectValue2 < currentEffectValue1 || currentEffectValue2 <= 0 || currentEffectValue2 >= ledsAmount) { 
   sendErrorMessageForType(kLEDOnRangeResponse);
 } 

 turnLEDsOff();
 for(int i = currentEffectValue1; i < currentEffectValue2; i++) {
     turnLEDOn(i);
 }
 
 if(millisToKeepOn > 0) {
   myThread.setInterval(millisToKeepOn); 
   myThread.enabled = true;  
   myThread.onRun(ledOnRangeOver);
 }
 sendSuccessMessageForType(kLEDOnRangeResponse);    
}

void ledOnRangeOver() {
   for(int i = currentEffectValue1; i < currentEffectValue2; i++) {
       turnLEDOff(i);
   }  
}

void OnLedOff() {
 if(! USE_LEDS) { 
   sendLEDsDisabledMessage();
   return;
 }

 // led index
 currentEffectValue1 = cmdMessenger.readInt16Arg();

 switch(currentEffectValue1) {
   case -1:
     // turn on all leds
     turnLEDsOff();
   break;
   default:
     turnLEDOff(currentEffectValue1);
   break;   
 }
    
 cmdMessenger.sendCmdStart(kLEDOffResponse);  
 // first argument is for success or not
 // 1 = success
 // 2 = failure
 cmdMessenger.sendCmdArg(1);
 cmdMessenger.sendCmdArg(currentEffectValue1);

 cmdMessenger.sendCmdEnd();      
}

void OnLedBlink() {
 if(! USE_LEDS) { 
   sendLEDsDisabledMessage();
   return;
 }

 // led index
 currentEffectValue1 = cmdMessenger.readInt16Arg();  
 int blinkInterval = cmdMessenger.readInt16Arg();
 currentEffectRepetitions = cmdMessenger.readInt16Arg();
 
 startLedBlink(currentEffectValue1, blinkInterval);
    
 cmdMessenger.sendCmdStart(kLEDBlinkResponse);  
 // first argument is for success or not
 // 1 = success
 // 2 = failure
 cmdMessenger.sendCmdArg(1);
 cmdMessenger.sendCmdArg(blinkInterval);

 cmdMessenger.sendCmdEnd();      
}

// BLINK START

void startLedBlink(int ledIndex, int blinkInterval) {
    
   myThread.setInterval(blinkInterval);
    
   setupLedBlink(ledIndex);  
   myThread.enabled = true;  
   myThread.onRun(ledBlink);   
}

void setupLedBlink(int ledIndex) {
  currentEffectValue1 = ledIndex;
  
  // store if it is currently on or off
  currentEffectValue2 = 1;    
}


void ledBlink() {
    if(currentEffectRepetitions == 0) {
      turnLEDsOff();
      myThread.enabled = false;
      return;
    }
  
  // led index
  switch(currentEffectValue1) {
    case -1:
      if(currentEffectValue2 > 0) {
        turnLEDsOn();
      } else {
        turnLEDsOff();
      }
    break;
    default:
      if(currentEffectValue2 > 0) {
        turnLEDOn(currentEffectValue1);
      } else {
        turnLEDOff(currentEffectValue1);
      }      
    break;   
  }  

  if(currentEffectValue2 > 0) {
    currentEffectValue2 = -1;
  } else {
    currentEffectValue2 = 1; 
    currentEffectRepetitions--;
  } 
}

// BLINK END

// General CmdMessenger Messages START

void sendLEDsDisabledMessage() {  
    cmdMessenger.sendCmdStart(kLEDEffectResponse);  
    // first argument is for success or not
    // 1 = success
    // 2 = failure
    cmdMessenger.sendCmdArg(0);
    cmdMessenger.sendCmdArg("LEDs are disabled");
    cmdMessenger.sendCmdEnd();    
}

void sendErrorMessageForType(int cType) {
 cmdMessenger.sendCmdStart(cType);  
 // first argument is for success or not
 // 1 = success
 // 2 = failure
 cmdMessenger.sendCmdArg(2);
 cmdMessenger.sendCmdEnd();  
}

void sendSuccessMessageForType(int cType) {
 cmdMessenger.sendCmdStart(cType);  
 // first argument is for success or not
 // 1 = success
 // 2 = failure
 cmdMessenger.sendCmdArg(1);
 cmdMessenger.sendCmdEnd();  
}

// General CmdMessenger Messages END


// General LED Functions START

void turnLEDsOff() {
    for(int i = 0; i < ledsAmount; i++) {
        digitalWrite(ledPins[i], LOW);
     }  
}

void turnLEDsOn() {
    for(int i = 0; i < ledsAmount; i++) {
        digitalWrite(ledPins[i], HIGH);
     }  
}

void turnLEDOn(int index) {
  if(index >= ledsAmount) { return; }
  
  digitalWrite(ledPins[index], HIGH);
}

void turnLEDOff(int index) {
  if(index >= ledsAmount) { return; }
  
  digitalWrite(ledPins[index], LOW);
}

// General LED Functions END


// Effects START

void startLedEffectKnightRider() {
    
   myThread.setInterval(40);
    
   setupLedEffectKnightRider();  
   myThread.enabled = true;  
   myThread.onRun(ledEffectKnightRider);   
}

void setupLedEffectKnightRider() {
  currentEffectValue1 = 0;
  currentEffectValue2 = 1;    
}


void ledEffectKnightRider() {
    if(currentEffectRepetitions == 0) {
      turnLEDsOff();
      myThread.enabled = false;
      return;
    }
  
    int lastLedPin;
    
    if(currentEffectValue2 > 0) {
      if(currentEffectValue1 == ledsAmount) {
        currentEffectValue2 = -1;
      }
    } else if(currentEffectValue2 < 0) {
      if(currentEffectValue1 < 0) {
        currentEffectValue2 = 1;
        currentEffectRepetitions--;
      }
    }
    
    if(currentEffectValue2 > 0) {
      if(currentEffectValue1 == 0) {
        lastLedPin = ledsAmount - 1;
      } else {
        lastLedPin = currentEffectValue1 - 1;
      }
    } else {
        if(currentEffectValue1 == ledsAmount) {
            lastLedPin = 0;
        } else {
            lastLedPin = currentEffectValue1 + 1;
        }      
    }
    
    digitalWrite(ledPins[currentEffectValue1], HIGH);
    digitalWrite(ledPins[lastLedPin], LOW);
    
    if(currentEffectValue2 > 0) {
      currentEffectValue1++;
    } else {
      currentEffectValue1--;      
    }
 
}

void startLedEffectLandingZone() {
      
    myThread.setInterval(100);
    
    setupLedEffectLandingZone();   
    myThread.enabled = true;  
    myThread.onRun(ledEffectLandingZone);
}

void setupLedEffectLandingZone() {
  currentEffectValue1 = 0;
  currentEffectValue2 = ledsAmount - 1;     
}

void ledEffectLandingZone() {
    
    if(currentEffectRepetitions == 0) {
      turnLEDsOff();
      myThread.enabled = false;
      return;
    }
  
    if(currentEffectValue1 > currentEffectValue2) {
      turnLEDsOff();
      setupLedEffectLandingZone();
      currentEffectRepetitions--;
      return;
    }
    
    digitalWrite(ledPins[currentEffectValue1], HIGH);
    digitalWrite(ledPins[currentEffectValue2], HIGH); 

    currentEffectValue1++;    
    currentEffectValue2--;
}

// Effects END


