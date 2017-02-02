#include <spi4teensy3.h>
#include <hidboot.h>
#include <usbhub.h>
#include <xinput.h>
#include "mouse_handler.h"

USB usb;
USBHub hub(&usb);
HIDBoot<USB_HID_PROTOCOL_MOUSE> hid_mouse(&usb);
uint32_t next_time;
MouseHandler mouse_handler;
XINPUT controller(NO_LED);

void setup() {
  Serial.begin(115200);
  pinMode(15, INPUT);
  pinMode(16, INPUT);

  usb.Init();
       
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
      //Keyboard.set_key1(KEY_RIGHT);
      controller.stickUpdate(STICK_LEFT, 32000, 0);
    } else {
      //Keyboard.set_key1(KEY_LEFT);
      controller.stickUpdate(STICK_LEFT, -32000, 0);
    }
  } else {
    //Keyboard.set_key1(0);
    controller.stickUpdate(STICK_LEFT, 0, 0);
  }
  
  if(digitalRead(15) == HIGH) {
    //Keyboard.set_modifier(MODIFIERKEY_SHIFT);
    controller.buttonUpdate(BUTTON_A, true);
  } else {
    //Keyboard.set_modifier(0);
    controller.buttonUpdate(BUTTON_A, false);
  }
  
  if(digitalRead(16) == HIGH) {
    //Keyboard.set_key2(KEY_ENTER);
    controller.buttonUpdate(BUTTON_B, true);
  } else {
    //Keyboard.set_key2(0);
    controller.buttonUpdate(BUTTON_B, false);
  }
  
  //Keyboard.send_now();
  controller.sendXinput();

  //Receive data
  controller.receiveXinput();
  
}

void loop() {
  process_mouse();
  delay(10);
}

