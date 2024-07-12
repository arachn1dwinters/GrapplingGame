
using System;
using System.Collections.Generic;
using System.Diagnostics;
using GrapplingGame.GameObjectsComponentsLevels.Components;
using GrapplingGame.GameObjectsComponentsLevels.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GrapplingGame.GameObjectsComponentsLevels.Levels;
public class Level
{
    public virtual List<GameObject> GameObjects { get; set; }
    public List<GameObject> targets = new();

    public virtual string tiledMap { get; set; }
    public int index;

    public GameObject Player;
    public GameObject GrappleGun;

    public GameObject CurrentActiveTarget;
    public int DistanceFromTarget;

    public Level(GameManager parent, bool respawn)
    {
        GameObjects = new();

        if (!respawn)
        {
            parent.levels.Add(this);
            index = parent.levels.IndexOf(this);
        }
    }

    public virtual void Update()
    {
        MouseState mouseState = Mouse.GetState();
        Point mousePos = new(mouseState.X, mouseState.Y);

        if (!(bool)Player.GetAttributeVariable("MovementComponent", "Grounded"))
        {
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                foreach (GameObject target in targets)
                {
                    int distance = (int)Math.Floor(Math.Pow(mousePos.X - target.position.X, 2) + Math.Pow(mousePos.Y - target.position.Y, 2));
                    if (distance < 700 && CurrentActiveTarget == null)
                    {
                        SwitchTarget(target);
                        Point playerVelocity = (Point)Player.GetAttributeVariable("MovementComponent", "Velocity");
                        Player.SetAttributeVariable("GrapplePhysicsComponent", "AngleIncrement",
                            Math.Min(GrapplePhysicsComponent.DistanceToIncrement(Math.Sqrt(Math.Pow(playerVelocity.X, 2) + Math.Pow(playerVelocity.Y, 2)), this), 0.1));
                        bool clockwise = false;
                        if ((playerVelocity.Y < 0 && Player.position.X < CurrentActiveTarget.position.X + CurrentActiveTarget.width / 2) || (playerVelocity.Y > 0 && Player.position.X > CurrentActiveTarget.position.X + CurrentActiveTarget.width / 2))
                        {
                            clockwise = true;
                        }
                        Player.SetAttributeVariable("GrapplePhysicsComponent", "Clockwise", clockwise);
                    }
                }
            }
            else
            {
                if (CurrentActiveTarget != null)
                {
                    Player.SetAttributeVariable("MovementComponent", "Grappling", false);
                    Player.SetAttributeVariable("MovementComponent", "Velocity", Player.GetAttributeVariable("GrapplePhysicsComponent", "MostRecentMovement"));
                    CurrentActiveTarget.SetAttributeVariable("TargetComponent", "Active", false);
                    CurrentActiveTarget = null;
                }
            }
        }
    }
    public virtual void Initialize() { }

    public void Remove()
    {
        GameManager.Instance.levels.Remove(this);
    }

    void SwitchTarget(GameObject target)
    {
        Point targetDistancePoint = GrappleGun.position - target.position;
        DistanceFromTarget = (int)Math.Sqrt(Math.Pow(targetDistancePoint.X, 2) + Math.Pow(targetDistancePoint.Y, 2));

        double rot;
        double deltaX = Player.position.X - target.position.X;
        double deltaY = Player.position.Y - target.position.Y;

        if (deltaX != 0)
        {
            rot = Math.Atan(deltaY / deltaX);
            if (deltaX < 0)
            {
                rot += Math.PI;
            }
        }
        else
        {
            if (deltaY > 0)
            {
                rot = Math.PI / 2;
            }
            else
            {
                rot = -Math.PI / 2;
            }
        }

        Player.SetAttributeVariable("GrapplePhysicsComponent", "Angle", rot);

        target.SetAttributeVariable("TargetComponent", "Active", true);
        CurrentActiveTarget = target;
        Player.SetAttributeVariable("MovementComponent", "Grappling", true);
    }
}
