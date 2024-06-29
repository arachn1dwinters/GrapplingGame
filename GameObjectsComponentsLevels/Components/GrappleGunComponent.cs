using System;
using System.Collections.Generic;
using GrapplingGame.GameObjectsComponentsLevels.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrapplingGame.GameObjectsComponentsLevels.Components;
public class GrappleGunComponent : Component
{
    public Point TipOfGun;

    public GrappleGunComponent(GameObject parent) : base(parent)
    {
        type = "GrappleGunComponent";
        TipOfGun = new(parent.position.X + 104, 32);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        foreach (GameObject target in parent.parent.targets)
        {
            if ((bool)target.GetAttributeVariable("TargetComponent", "Active"))
            {
                // Rotate to point at target
                int dX = target.position.X - parent.position.X;
                int dY = target.position.Y - parent.position.Y;

                parent.Rotation = (float)Math.Atan2(dY, dX);
                parent.parent.ActiveTarget = target;
                break;
            }

            parent.parent.ActiveTarget = null;
        }
    }
}