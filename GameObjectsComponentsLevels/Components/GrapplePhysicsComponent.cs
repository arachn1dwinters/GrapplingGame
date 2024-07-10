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

    double currentPullingVelocity = 0;

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
                Point actualMovement = newPlayerPosition - parent.position;

                MostRecentMovement = actualMovement;

                parent.position = newPlayerPosition;
                parent.parent.GrappleGun.position = newPlayerPosition + new Point(16, 16);

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
            } else if (targetType == TARGETTYPE.pull)
            {
                Point targetPosition = parent.parent.CurrentActiveTarget.position;
                Point gunPosition = parent.parent.GrappleGun.position;

                double distanceToTarget = Math.Sqrt(Math.Pow(targetPosition.X - gunPosition.X, 2) + Math.Pow(targetPosition.Y - gunPosition.Y, 2));

                // Normalize the direction vector
                double directionX = (targetPosition.X - gunPosition.X) / distanceToTarget;
                double directionY = (targetPosition.Y - gunPosition.Y) / distanceToTarget;

                // Define the pulling speed (the rate at which the player moves towards the target)
                currentPullingVelocity += 1;

                // Move the player towards the target
                int newX = (int)Math.Floor(gunPosition.X + directionX * currentPullingVelocity);
                int newY = (int)Math.Floor(gunPosition.Y + directionY * currentPullingVelocity);

                Point newGunPosition = new Point(newX, newY);
                Point actualMovement = newGunPosition - new Point(16, 16) - parent.position;

                parent.position = newGunPosition - new Point(16, 16);
                parent.parent.GrappleGun.position = newGunPosition;

                // Optionally, stop the player when they reach the target
                if (distanceToTarget <= currentPullingVelocity)
                {
                    parent.position = targetPosition - new Point(16, 16);
                    parent.parent.GrappleGun.position = targetPosition;
                }
            }
        }
        else
        {
            if (SetAngle)
            {
                SetAngle = false;
            }

            currentPullingVelocity = 0;
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
        return (2 * Math.PI)/n;
    }
}