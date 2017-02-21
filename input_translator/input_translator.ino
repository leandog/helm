#include <spi4teensy3.h>
#include <hidboot.h>
#include <usbhub.h>
#include <xinput.h>
#include "mouse_handler.h"

const int MAX_STICK_TRAVEL = 32000;
void process_mouse_as_xinput();

USB usb;
USBHub hub(&usb);
HIDBoot<USB_HID_PROTOCOL_MOUSE> hid_mouse(&usb);
MouseHandler mouse_handler;
XINPUT controller(NO_LED);

void setup() {
  pinMode(15, INPUT);
  pinMode(16, INPUT);

  usb.Init();
  
  delay(200);
  
  hid_mouse.SetReportParser(0,(HIDReportParser*)&mouse_handler);
}

void loop() {
  process_mouse_as_xinput();
  delay(10);
}

void process_mouse_as_xinput() {
  mouse_handler.reset_travel();
  usb.Task();
  
  int y_travel = mouse_handler.get_y_travel();
  int stick_travel = 0;
  if(abs(y_travel) > 3) {
    stick_travel = y_travel < 0 ? MAX_STICK_TRAVEL : -MAX_STICK_TRAVEL;
  }
  
  controller.stickUpdate(STICK_LEFT, stick_travel, 0);
  controller.buttonUpdate(BUTTON_A, digitalRead(15) == HIGH);
  controller.buttonUpdate(BUTTON_B, digitalRead(16) == HIGH);
  controller.sendXinput();
  controller.receiveXinput();
}
