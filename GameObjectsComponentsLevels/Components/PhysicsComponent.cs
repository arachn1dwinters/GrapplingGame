using System;
using System.Collections.Generic;
using GrapplingGame.GameObjectsComponentsLevels.GameObjects;
using GrapplingGame.GameObjectsComponentsLevels.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrapplingGame.GameObjectsComponentsLevels.Components;
public class PhysicsComponent : Component
{
    public PhysicsComponent(GameObject parent) : base(parent)
    {
        type = "PhysicsComponent";
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // Gravity
        Vector2Int currentMovement = new();
        parent.SetAttributeVariable("MovementComponent", "movement", new Vector2Int(currentMovement.x, currentMovement.y + 10));
    }
}