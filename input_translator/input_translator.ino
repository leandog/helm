#include <hidboot.h>
#include <usbhub.h>
#include <Encoder.h>
#include "mouse_handler.h"

int joy_x = 512;
int joy_right_trigger = 512;

USB usb;
USBHub hub(&usb);
HIDBoot<HID_PROTOCOL_MOUSE> hid_mouse(&usb);
uint32_t next_time;
MouseHandler mouse_handler;

Encoder myEnc(12, 13);
long oldPosition  = -999;

void setup() {

  //Serial.begin( 115200 );
  
  Joystick.useManualSend(true);
  
  pinMode(14, INPUT_PULLUP);
  pinMode(11, OUTPUT);

  //if (usb.Init() == -1)
    //Serial.println("OSC did not start.");

  delay( 200 );
  next_time = millis() + 5000;


  hid_mouse.SetReportParser(0,(HIDReportParser*)&mouse_handler);
}


void read_encoder() {
  long newPosition = myEnc.read();
  if (newPosition != oldPosition) {
    if (oldPosition > newPosition) {
      if(joy_right_trigger - 10 >= 0) {
        joy_right_trigger = joy_right_trigger - 10;
      }
    } else {
      if(joy_right_trigger + 10 <= 1023) {
       joy_right_trigger = joy_right_trigger + 10;
      }
    }
    oldPosition = newPosition;
  }
}

/*
void process_mouse() {
  mouse_handler.reset_travel();
  usb.Task();
  int y_travel = mouse_handler.get_y_travel();
  y_travel = y_travel * -1;
  Mouse.move(y_travel, 0);
}
*/

void process_joystick() {
  usb.Task();
  
  int y_travel = mouse_handler.get_y_travel();
   
  if (y_travel < 0) {
    joy_x = 0;
  }
  
  if (y_travel > 0) {
    joy_x = 1023;
  }
  
  if (mouse_handler.get_checks_without_travel() > 50) {
    joy_x = 512;
    mouse_handler.reset_travel();
  }
  
  Joystick.X(joy_x);
  Joystick.Y(joy_x);
  Joystick.sliderRight(joy_right_trigger);

  if(digitalRead(14) == LOW) {
    digitalWrite(11, HIGH);
    Joystick.button(1, 1);
  } else {
    digitalWrite(11, LOW);
    Joystick.button(1, 0);
  }
    
  Joystick.send_now();
}


void loop() {
  //process_mouse();
  read_encoder();
  process_joystick();
}

