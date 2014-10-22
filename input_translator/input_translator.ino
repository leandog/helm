#include <hidboot.h>
#include <usbhub.h>
#include "mouse_handler.h"

int MAX_MOUSE_RANGE = 254;
int MAX_JOYSTICK_RANGE = 1023;

USB usb;
USBHub hub(&usb);
HIDBoot<HID_PROTOCOL_MOUSE> hid_mouse(&usb);
uint32_t next_time;
MouseHandler mouse_handler;

void setup() {
  Serial.begin( 115200 );

  if (usb.Init() == -1)
    Serial.println("OSC did not start.");

  delay( 200 );
  next_time = millis() + 5000;

  hid_mouse.SetReportParser(0,(HIDReportParser*)&mouse_handler);
}

int loops_without_travel = 0;
void loop() {
  if (loops_without_travel > 10000) {
    loops_without_travel = 0;
    mouse_handler.reset_travel();
  }
  
  usb.Task();
  int y_travel = mouse_handler.get_y_travel();
  if (y_travel == 0) {
    loops_without_travel++;
  }
  
  unsigned long y_positive = (MAX_MOUSE_RANGE / 2) + y_travel + 1;
  unsigned long y_translated = (y_positive * 100) / MAX_MOUSE_RANGE;
  unsigned long joystick_x_translated = y_translated * MAX_JOYSTICK_RANGE;
  unsigned long joystick_x = joystick_x_translated / 100;
  Joystick.X(joystick_x);
}

