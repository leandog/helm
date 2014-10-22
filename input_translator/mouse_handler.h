#include <hidboot.h>

class MouseHandler : public MouseReportParser { 
  int y_travel;
public:
  virtual void OnMouseMove(MOUSEINFO *mouse_info);
  void reset_travel();
  int get_y_travel();
};
