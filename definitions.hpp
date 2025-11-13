#ifndef DEFINITIONS
#define DEFINITIONS

#include <allegro5/allegro.h>
#include <allegro5/allegro_font.h>
#include <allegro5/allegro_primitives.h>
#include <iostream>
#include <math.h>
using namespace std;

// Game-wide structs etc.
struct Point {
    double X, Y;
};

// Game functions
void Update();
void Draw(ALLEGRO_FONT *font, ALLEGRO_MOUSE_STATE state);

// GameObject class, functions
class GameObject {
    public:
        void Move(Point Movement);
        Point Pos = {915, 400};

        void ApplyPhysics();
        bool UsesPhysics = true;
        bool Swinging = false;
        double FallingVelocity = 0; // only used on falling physics objects
        double AngleIncrement = 0.005 * M_PI; // Radians, only used on swinging physics objects
        double CurrentAngle; // Testing purposes
        Point TargetPos = {615, 400};
};

enum SwingTypes {
    RIGHT,
    LEFT,
    HANG
};

#endif
