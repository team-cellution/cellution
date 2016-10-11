///Cell Movement

if mouse_check_button_pressed(mb_left)
{
    destination[0] = mouse_x;
    destination[1] = mouse_y;
    move_towards_point(destination[0], destination[1], 2);
}

d = point_distance(x, y, destination[0], destination[1]);

var test;
test = 5;
