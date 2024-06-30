using System;
using System.Collections.Generic;
using GrapplingGame.GameObjectsComponentsLevels.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrapplingGame.GameObjectsComponentsLevels.Components;
public class GrapplePhysicsComponent : Component
{
    public GrapplePhysicsComponent(GameObject parent) : base(parent)
    {
        type = "GrapplePhysicsComponent";
    }

    public override void FixedUpdate(GameTime gameTime)
    {
        base.FixedUpdate(gameTime);

        if (parent.parent.CurrentActiveTarget != null)
        {
            parent.CallAttributeMethod("MovementComponent", "MoveX",  new object[] {5});
        }
    }
}