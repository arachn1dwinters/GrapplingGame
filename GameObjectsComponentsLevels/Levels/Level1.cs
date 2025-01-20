using System;
using System.Collections.Generic;
using GrapplingGame.GameObjectsComponentsLevels.Helpers;
using GrapplingGame.GameObjectsComponentsLevels.GameObjects;
using GrapplingGame;
using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace GrapplingGame.GameObjectsComponentsLevels.Levels
{
    public class Level1 : Level
    {
        public Level1(GameManager parent, bool respawn) : base(parent, respawn) { }
        public override string TiledMap { get; set; }
        public override List<Screen> Screens { get; set; }

        GameObject player;
        GameObject grappleGun;

        public override void Initialize()
        {
            player = new(GameManager.Instance.playerSprite, PlayerSpawn, new Point(1, 1), "player", this, new()
            {
                "MovementComponent",
                "GrapplePhysicsComponent",
            });
            Player = player;


            grappleGun = new(GameManager.Instance.grapplingGunSprite, PlayerSpawn + new Point(16, 16), new Point(1, 1), "Grappling Gun", this, new()
            {
                "GrappleGunComponent"
            })
            {
                collidable = false,
                origin = new Vector2(0, 16)
            };
            GrappleGun = grappleGun;
        }

        public override void Update()
        {
            base.Update();
        }
    }
}