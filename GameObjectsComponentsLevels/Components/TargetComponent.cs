using System;
using System.Collections.Generic;
using GrapplingGame.GameObjectsComponentsLevels.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrapplingGame.GameObjectsComponentsLevels.Components;
public class TargetComponent : Component
{
    // Set as true when the mouse is within its radius
    /* When active is set to true do this:
        parent.cropped = false;
        parent.sprite = parent.parent.parent.targetActiveSprite;
        parent.rect = new(parent.position.X, parent.position.Y, 32, 32);
        parent.width = 32;
        parent.height = 32;
    */
    public bool Active;

    public TargetComponent(GameObject parent) : base(parent)
    {
        type = "TargetComponent";
        parent.parent.targets.Add(parent);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }
}