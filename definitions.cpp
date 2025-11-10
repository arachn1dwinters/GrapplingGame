#include "definitions.hpp"

// GameObject functions
void GameObject::Move(Point Movement) {
    Pos.X += Movement.X;
    Pos.Y += Movement.Y;
};

void GameObject::ApplyPhysics(Point TargetPos = DEFAULTSWINGTARGET) {
    if (UsesPhysics) {
        if (!Swinging) {
            CurrentFallingVelocity += 2;
            Pos.Y += CurrentFallingVelocity;
        } e
            Point Distance = {TargetPos.X - Pos.X, TargetPos.Y - Pos.Y};
            SwingTypes SwingType = (Distance.X > 0) ? SWINGRIGHT : (Distance.X == 0) ? HANG : (DistanceX < 0) ? SWINGLEFT;
        }
    } 
};

// Misc. definitions
Point DEFAULTSWINGTARGET = {640, 100};
