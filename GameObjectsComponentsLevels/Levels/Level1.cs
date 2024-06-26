using System;
using System.Collections.Generic;
using GrapplingGame.GameObjectsComponentsLevels.Helpers;
using GrapplingGame.GameObjectsComponentsLevels.GameObjects;
using GrapplingGame;
using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Input;

namespace GrapplingGame.GameObjectsComponentsLevels.Levels
{
    public class Level1 : Level
    {
        public Level1(GameManager parent, bool respawn) : base(parent, respawn) { }
        public override string tiledMap { get; set; }

        GameObject player;
        GameObject grappleGun;
        GameObject currentActiveTarget;

        public override void Initialize()
        {
            tiledMap = "Level1.tmx";
            parent.CreateMap(tiledMap);

            player = new(parent.playerSprite, new Point(50, 50), new Point(1, 1), "player", this, new()
            {
                "MovementComponent",
                //"AnimationAttribute"
            });
            /*player.SetAttributeVariable("AnimationAttribute", "frameHeight", 16);
            player.SetAttributeVariable("AnimationAttribute", "frameWidth", 16);
            player.SetAttributeVariable("AnimationAttribute", "animationLengths", new List<int>()
            {
                4,
            });*/
            grappleGun = new(parent.grapplingGunSprite, new Point(50, 66), new Point(1, 1), "Grappling Gun", this, new()
            {
                "GrappleGunComponent"
            })
            {
                collidable = false,
                origin = new Vector2(0, 16)
            };

            player.SetAttributeVariable("MovementComponent", "GrappleGun", grappleGun);
        }

        public override void Update()
        {
            base.Update();

            MouseState mouseState = Mouse.GetState();
            Point mousePos = new(mouseState.X, mouseState.Y);
            foreach (GameObject target in targets)
            {
                if (target != currentActiveTarget)
                {
                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        int distance = (int)Math.Floor(Math.Pow(mousePos.X - target.position.X, 2) + Math.Pow(mousePos.Y - target.position.Y, 2));
                        if (distance < 700)
                        {
                            if (currentActiveTarget != null)
                            {
                                switchTargets(currentActiveTarget, target);
                            } else
                            {
                                currentActiveTarget = target;
                                target.SetAttributeVariable("TargetComponent", "Active", true);
                            }
                        }
                    }
                }
            }
        }

        void switchTargets(GameObject previousTarget, GameObject newTarget)
        {
            // Put some sort of cool lerp animation here(later)
            newTarget.SetAttributeVariable("TargetComponent", "Active", true);
            previousTarget.SetAttributeVariable("TargetComponent", "Active", false);

            currentActiveTarget = newTarget;
        }
    }
}