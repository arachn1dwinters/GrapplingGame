using System;
using System.Collections.Generic;
using System.Diagnostics;
using GrapplingGame.GameObjectsComponentsLevels.GameObjects;
using GrapplingGame.GameObjectsComponentsLevels.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrapplingGame.GameObjectsComponentsLevels.Components;
public class GrapplePhysicsComponent : Component
{
    public double Angle = 0;
    public Point MostRecentMovement;

    public GrapplePhysicsComponent(GameObject parent) : base(parent)
    {
        type = "GrapplePhysicsComponent";
    }

    public override void FixedUpdate(GameTime gameTime)
    {
        base.FixedUpdate(gameTime);

        if (parent.parent.CurrentActiveTarget != null)
        {
            parent.SetAttributeVariable("MovementComponent", "Grappling", true);
            // Move player in a circle around the active target.
            Angle += 0.1;
            Point newPlayerPosition = new((int)Math.Floor((Math.Cos(Angle) * parent.parent.DistanceFromTarget) + parent.parent.CurrentActiveTarget.position.X), (int)Math.Floor((Math.Sin(Angle) * parent.parent.DistanceFromTarget) + parent.parent.CurrentActiveTarget.position.Y));
            Point actualMovement = newPlayerPosition - parent.position;
            MostRecentMovement = new Point(actualMovement.X, actualMovement.Y);
            parent.position = newPlayerPosition;
            parent.parent.GrappleGun.position = newPlayerPosition + new Point(16, 16);
        }
    }
}