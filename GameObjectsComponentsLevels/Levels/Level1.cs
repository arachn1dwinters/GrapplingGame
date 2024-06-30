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

        public override void Initialize()
        {
            tiledMap = "Level1.tmx";
            parent.CreateMap(tiledMap);

            player = new(parent.playerSprite, new Point(50, 50), new Point(1, 1), "player", this, new()
            {
                "MovementComponent",
                "GrapplePhysicsComponent"
            });
            Player = player;

            grappleGun = new(parent.grapplingGunSprite, new Point(66, 66), new Point(1, 1), "Grappling Gun", this, new()
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
        }
    }
}