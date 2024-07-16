using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using GrapplingGame.GameObjectsComponentsLevels.GameObjects;
using GrapplingGame.GameObjectsComponentsLevels.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrapplingGame.GameObjectsComponentsLevels.Components;
public class GrapplePhysicsComponent : Component
{
    public double Angle = 0;

    public double AngleIncrement = 0.1;
    double maxDistance = 1000;
    double distanceFromTarget;
    double angleIncrement;
    double angleVelocity;
    double scaleFactor;

    public bool Clockwise = false;

    public bool SetAngle;

    public Point MostRecentMovement;

    double currentPullingVelocity = 1;

    public GrapplePhysicsComponent(GameObject parent) : base(parent)
    {
        type = "GrapplePhysicsComponent";
    }

    public override void FixedUpdate(GameTime gameTime)
    {
        base.FixedUpdate(gameTime);

        if (parent.parent.CurrentActiveTarget != null)
        {
            TARGETTYPE targetType = (TARGETTYPE)parent.parent.CurrentActiveTarget.GetAttributeVariable("TargetComponent", "TargetType");
            if (targetType == TARGETTYPE.swing)
            {
                if (!SetAngle)
                {
                    distanceFromTarget = parent.parent.DistanceFromTarget;
                    scaleFactor = Math.Max(0.1, 1.0 - (distanceFromTarget / maxDistance));
                    angleIncrement = AngleIncrement * scaleFactor;
                    angleVelocity = 0;
                    SetAngle = true;
                }

                Angle = Clockwise ? Angle + angleIncrement + angleVelocity : Angle - angleIncrement - angleVelocity;

                Point newPlayerPosition = new Point(
                    (int)Math.Floor((Math.Cos(Angle) * parent.parent.DistanceFromTarget) + parent.parent.CurrentActiveTarget.position.X),
                    (int)Math.Floor((Math.Sin(Angle) * parent.parent.DistanceFromTarget) + parent.parent.CurrentActiveTarget.position.Y)
                );
                MostRecentMovement = newPlayerPosition - parent.position;

                Point playerMovement = (Point)parent.GetAttributeVariable("MovementComponent", "Movement");
                parent.SetAttributeVariable("MovementComponent", "Movement", playerMovement + MostRecentMovement);

                if (angleVelocity != 0)
                {
                    if (angleVelocity > 0)
                    {
                        angleVelocity -= 0.08;
                    }
                    else
                    {
                        angleVelocity += 0.08;
                    }
                }
            }
            else if (targetType == TARGETTYPE.pull)
            {
                Point targetPosition = parent.parent.CurrentActiveTarget.position;
                Point gunPosition = parent.parent.GrappleGun.position;
                Point gunTipPosition = (Point)parent.parent.GrappleGun.GetAttributeVariable("GrappleGunComponent", "TipOfGun");

                // The reason that we don't use parent.parent.DistanceToTarget here is that that is only created when the grappling hook latches on to the target, not updated every frame like this one.
                double distanceToTarget = Math.Sqrt(Math.Pow(targetPosition.X - parent.position.X, 2) + Math.Pow(targetPosition.Y - parent.position.Y, 2));

                double directionX = (targetPosition.X - gunTipPosition.X) / distanceToTarget;
                double directionY = (targetPosition.Y - gunTipPosition.Y) / distanceToTarget;

                if (currentPullingVelocity > 0) currentPullingVelocity = 20;

                int newX = (int)Math.Floor(gunPosition.X + directionX * currentPullingVelocity);
                int newY = (int)Math.Floor(gunPosition.Y + directionY * currentPullingVelocity);

                Point newGunPosition = new(newX, newY);
                Point actualMovement = newGunPosition - new Point(16, 16) - parent.position;

                Point playerMovement = (Point)parent.GetAttributeVariable("MovementComponent", "Movement");
                parent.SetAttributeVariable("MovementComponent", "Movement", playerMovement + actualMovement);

                if (distanceToTarget <= 104)
                {
                    currentPullingVelocity = 0;
                }
            }
        }
        else
        {
            if (SetAngle)
            {
                SetAngle = false;
            }

            currentPullingVelocity = 1;
        }

        if (Angle > 2 * Math.PI)
        {
            Angle -= 2 * Math.PI;
        }
    }

    // Function to convert angle increments(in radians) to the distance traveled by the player when that increment is added
    public static Point IncrementToDistance(double increment, double currentAngle, Levels.Level parent, GameObject currentActiveTarget)
    {
        return new Point(
                (int)Math.Floor(((Math.Cos(currentAngle + increment) * parent.DistanceFromTarget) + parent.CurrentActiveTarget.position.X) / 10),
                (int)Math.Floor(((Math.Sin(currentAngle + increment) * parent.DistanceFromTarget) + parent.CurrentActiveTarget.position.Y) / 10)
            );
    }

    // Function to convert distance traveled by the player(per frame) to an angle increment (in radians)
    public static Double DistanceToIncrement(Double distanceTraveled, Levels.Level parent)
    {
        int n = (int)Math.Floor(Math.PI / Math.Atan(distanceTraveled / (2 * parent.DistanceFromTarget)));
        return (2 * Math.PI) / n;
    }
}