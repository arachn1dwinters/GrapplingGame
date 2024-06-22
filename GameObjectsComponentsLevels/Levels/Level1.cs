using System;
using System.Collections.Generic;
using GrapplingGame.GameObjectsComponentsLevels.Helpers;
using GrapplingGame.GameObjectsComponentsLevels.GameObjects;
using GrapplingGame;

using Microsoft.Xna.Framework.Input;

namespace GrapplingGame.GameObjectsComponentsLevels.Levels
{
    public class Level1 : Level
    {
        public Level1(GameManager parent, bool respawn) : base(parent, respawn) { }
        public override string tiledMap { get; set; }

        GameObject player;

        public override void Initialize()
        {
            tiledMap = "Level1.tmx";
            parent.CreateMap(tiledMap);

            player = new(parent.playerSprite, new Vector2Int(50, 50), new Vector2Int(1, 1), "player", this, new()
            {
                "MovementComponent",
                "PhysicsComponent",
                //"AnimationAttribute"
            });
            /*player.SetAttributeVariable("AnimationAttribute", "frameHeight", 16);
            player.SetAttributeVariable("AnimationAttribute", "frameWidth", 16);
            player.SetAttributeVariable("AnimationAttribute", "animationLengths", new List<int>()
            {
                4,
            });*/
        }

        public override void Update()
        {

        }
    }
}