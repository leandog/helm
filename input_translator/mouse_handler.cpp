#include "mouse_handler.h"

void MouseHandler::OnMouseMove(MOUSEINFO *mouse_info) {
  y_travel = mouse_info->dY;
}

void MouseHandler::reset_travel() {
  y_travel = 0;
}

int MouseHandler::get_y_travel() {
  return y_travel;
}
