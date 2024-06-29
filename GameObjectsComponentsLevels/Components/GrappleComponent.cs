using System;
using System.Collections.Generic;
using GrapplingGame.GameObjectsComponentsLevels.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrapplingGame.GameObjectsComponentsLevels.Components;
public class GrappleComponent : Component
{
    public GrappleComponent(GameObject parent) : base(parent)
    {
        type = "GrappleComponent";
    }

    public override void FixedUpdate(GameTime gameTime)
    {
        base.FixedUpdate(gameTime);

        if (parent.parent.ActiveTarget != null)
        {
            
        }
    }
}