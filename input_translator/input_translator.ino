#include <spi4teensy3.h>
#include <hidboot.h>
#include <usbhub.h>
#include "mouse_handler.h"

USB usb;
USBHub hub(&usb);
HIDBoot<HID_PROTOCOL_MOUSE> hid_mouse(&usb);
uint32_t next_time;
MouseHandler mouse_handler;

void setup() {
  //Serial.begin(115200);
  pinMode(15, INPUT);
  pinMode(16, INPUT);

  usb.Init();
   //if (usb.Init() == -1)
       //Serial.println("OSC did not start.");
       
  delay( 200 );
  next_time = millis() + 5000;

  hid_mouse.SetReportParser(0,(HIDReportParser*)&mouse_handler);
}

void process_mouse() {
  mouse_handler.reset_travel();
  usb.Task();
  int y_travel = mouse_handler.get_y_travel();

  //Serial.println(y_travel);
  
  int abs_y_travel = abs(y_travel);
  if(abs_y_travel > 3) {
    if(y_travel < 0) {
      Keyboard.set_key1(KEY_RIGHT);
    } else {
      Keyboard.set_key1(KEY_LEFT);
    }
  } else {
    Keyboard.set_key1(0);
  }
  
  if(digitalRead(15) == HIGH) {
    Keyboard.set_modifier(MODIFIERKEY_SHIFT);
  } else {
    Keyboard.set_modifier(0);
  }
  
  if(digitalRead(16) == HIGH) {
    Keyboard.set_key2(KEY_ENTER);
  } else {
    Keyboard.set_key2(0);
  }
  
  Keyboard.send_now();
  
}

void loop() {
  process_mouse();
  delay(10);
}

