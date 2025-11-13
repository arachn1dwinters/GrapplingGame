#include "definitions.hpp"

// GameObject functions
void GameObject::Move(Point Movement) {
    Pos.X += Movement.X;
    Pos.Y += Movement.Y;
};

static double DecideAngle(Point Pos, Point Origin) {
    double Y = (Origin.Y - Pos.Y);
    double X = -(Origin.X - Pos.X);

    if (X == 0) {
        if (Y > 0) return M_PI / 2;
        else { return 3 * M_PI / 2; };
    }

    return atan2(Y, X);
};

static Point DecidePoint(Point Origin, double Angle, double DistanceFromOrigin) {
    double X = DistanceFromOrigin * cos(Angle) + Origin.X;
    double Y = -DistanceFromOrigin * sin(Angle) + Origin.Y;
    return {X, Y};
}

void GameObject::ApplyPhysics() {
    if (UsesPhysics) {
        if (!Swinging) {
            FallingVelocity += 1;
            Pos.Y += FallingVelocity;
        } else {
            Point DistanceFromTarget = {TargetPos.X - Pos.X, TargetPos.Y - Pos.Y};
            double RopeLength = sqrt(pow(DistanceFromTarget.X, 2) + pow(DistanceFromTarget.Y, 2));
            
            CurrentAngle = DecideAngle(Pos, TargetPos);
            CurrentAngle = CurrentAngle + AngleIncrement;
            Pos = DecidePoint(TargetPos, CurrentAngle, RopeLength);
        }
    } 
};
