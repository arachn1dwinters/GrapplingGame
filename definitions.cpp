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
};

void GameObject::DecideIncrement() {
    int FinalIncrementMultiplier = CurrentlySwingingRight ? 1 : -1;
    if (AngleIncrement == 0) {
        // Flip direction
        FinalIncrementMultiplier = -FinalIncrementMultiplier;
        CurrentlySwingingRight = !CurrentlySwingingRight;
    }

    Point DistanceFromTarget = {TargetPos.X - Pos.X, TargetPos.Y - TargetPos.Y};
    int IncrementIncrementMultiplier;
    if ((DistanceFromTarget.X > 0 && !CurrentlySwingingRight) || (DistanceFromTarget.X < 0 && CurrentlySwingingRight)) {
        IncrementIncrementMultiplier = -1;
    } else if ((DistanceFromTarget.X > 0 && CurrentlySwingingRight) || (DistanceFromTarget.X < 0  && !CurrentlySwingingRight)) {
        IncrementIncrementMultiplier = 1;
    }

    double IncrementIncrement = 0.0005 * M_PI * IncrementIncrementMultiplier;
    AngleIncrement = FinalIncrementMultiplier * (AngleIncrement + IncrementIncrement);
};

void GameObject::ApplyPhysics() {
    if (UsesPhysics) {
        if (!Swinging) {
            FallingVelocity += 1;
            Pos.Y += FallingVelocity;
        } else {
            Point DistanceFromTarget = {TargetPos.X - Pos.X, TargetPos.Y - Pos.Y};
            double RopeLength = sqrt(pow(DistanceFromTarget.X, 2) + pow(DistanceFromTarget.Y, 2));
            
            DecideIncrement();
            CurrentAngle = DecideAngle(Pos, TargetPos);
            CurrentAngle = CurrentAngle + AngleIncrement;
            Pos = DecidePoint(TargetPos, CurrentAngle, RopeLength);
        }
    } 
};
