using System;
using System.Collections.Generic;
using GrapplingGame.GameObjectsComponentsLevels.GameObjects;
using Microsoft.Xna.Framework.Graphics;

namespace GrapplingGame.GameObjectsComponentsLevels.Components;
public class ComponentName : Component
{
    public ComponentName(GameObject parent) : base(parent)
    {
        type = "[ComponentName]";
    }
}