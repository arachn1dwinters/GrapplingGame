#ifndef DEFINITIONS
#define DEFINITIONS

#include <allegro5/allegro5.h>
#include <allegro5/allegro_font.h>
#include <allegro5/allegro_primitives.h>
#include <iostream>
#include <numbers>
using namespace std;

// Game-wide structs etc.
struct Point {
    int X, Y;
};

// Game functions
void Update();
void Draw(ALLEGRO_FONT *font);

// GameObject class, functions
class GameObject {
    public:
        void Move(Point Movement);
        Point Pos = {100, 100};

        void ApplyPhysics();
        bool UsesPhysics = true;
        bool Swinging = false;
        int CurrentFallingVelocity = 0; // only used on falling physics objects
        int CurrentAngleIncrement = 0.1 * std::numbers::pi; // Radians, only used on swinging physics objects
};

enum SwingTypes {
    RIGHT,
    LEFT,
    HANG
}

#endif
