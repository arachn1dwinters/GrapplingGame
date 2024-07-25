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
        TipOfGun = new(parent.position.X + 104, parent.position.Y + 32);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        foreach (GameObject target in parent.parent.targets)
        {
            if ((bool)target.GetAttributeVariable("TargetComponent", "Active"))
            {
                // Rotate to point at target
                Point posToPointAt = target.position + new Point(16, 16);
                int dX = posToPointAt.X - parent.position.X;
                int dY = posToPointAt.Y - parent.position.Y;

                parent.Rotation = (float)Math.Atan2(dY, dX);
                TipOfGun.X = (int)(parent.position.X + 104 * Math.Cos(parent.Rotation));
                TipOfGun.Y = (int)(parent.position.Y + 104 * Math.Sin(parent.Rotation));

                parent.position = parent.parent.Player.position + new Point(16, 16);

                parent.Visible = true;
                break;
            } else
            {
                parent.Visible = false;
            }
        }
    }
}