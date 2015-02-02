#include "mouse_handler.h"

void MouseHandler::OnMouseMove(MOUSEINFO *mouse_info) {
  y_travel = mouse_info->dY;
  if (y_travel != 0) {
    checks_without_travel = 0;
  }
}

void MouseHandler::reset_travel() {
  y_travel = 0;
  checks_without_travel = 0;
}

int MouseHandler::get_y_travel() {
  checks_without_travel++;
  return y_travel;
}

int MouseHandler::get_checks_without_travel() {
  return checks_without_travel;
}
