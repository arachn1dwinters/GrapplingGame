using System;
using System.Collections.Generic;
using System.Diagnostics;
using GrapplingGame.GameObjectsComponentsLevels.Components;
using GrapplingGame.GameObjectsComponentsLevels.GameObjects;
using GrapplingGame.GameObjectsComponentsLevels.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GrapplingGame.GameObjectsComponentsLevels.Levels;
public class Level
{
    // Children
    public virtual List<GameObject> GameObjects { get; set; }

    // Tiles
    public virtual string tiledMap { get; set; }

    // Gameplay
    public GameObject Player;
    public Point PlayerSpawn;
    public GameObject GrappleGun;
    public GameObject CurrentActiveTarget;
    public GameObject HoveredTarget; 
    public int DistanceFromTarget;
    public Point levelOrigin;
    public Point MousePosition;
    public Point TargetMousePosition;
    public bool CanConnect = true;
    int mouseSpeed = 50;
    public List<GameObject> targets = new();

    public Level(GameManager parent, bool respawn)
    {
        GameObjects = new();

        if (!respawn)
        {
            parent.levels.Add(this);
        }
    }

    public virtual void Update()
    {
        MouseState mouseState = Mouse.GetState();
        Vector2 mousePos = GameManager.Instance.OrthographicCamera.ScreenToWorld(new Vector2(mouseState.X, mouseState.Y));

        if (CurrentActiveTarget == null)
        {
            List<GameObject> hoveredTargets = new();
            int hoverDistance = 2000;
            foreach (GameObject target in targets)
            {
                int distance = (int)Math.Floor(Math.Pow(mousePos.X - target.position.X, 2) + Math.Pow(mousePos.Y - target.position.Y, 2));
                if (distance < hoverDistance)
                {
                    hoveredTargets.Add(target);
                }
            }

            if (hoveredTargets.Count > 1)
            {
                foreach (GameObject target in hoveredTargets)
                {
                    int distance = (int)Math.Floor(Math.Pow(mousePos.X - target.position.X, 2) + Math.Pow(mousePos.Y - target.position.Y, 2));
                    int bestDistance = HoveredTarget != null ? (int)Math.Floor(Math.Pow(mousePos.X - HoveredTarget.position.X, 2) + Math.Pow(mousePos.Y - HoveredTarget.position.Y, 2)) : hoverDistance;
                    HoveredTarget = distance < bestDistance ? target : HoveredTarget;
                    TargetMousePosition = HoveredTarget.position;
                }
            }
            else if (hoveredTargets.Count > 0)
            {
                HoveredTarget = hoveredTargets[1];
                TargetMousePosition = HoveredTarget.position;
            }
        }

        if (!(bool)Player.GetAttributeVariable("MovementComponent", "Grounded") && !(bool)Player.GetAttributeVariable("MovementComponent", "Ceilinged") && !(bool)Player.GetAttributeVariable("MovementComponent", "RightWalled") && !(bool)Player.GetAttributeVariable("MovementComponent", "LeftWalled") && CanConnect)
        {
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (CurrentActiveTarget == null && HoveredTarget != null)
                {
                    SwitchTarget(HoveredTarget);
                    Point playerVelocity = (Point)Player.GetAttributeVariable("MovementComponent", "Velocity");
                    Point playerMovement = (Point)Player.GetAttributeVariable("MovementComponent", "Movement");
                    Point actualPlayerVelocity = playerVelocity + playerMovement;
                    Player.SetAttributeVariable("GrapplePhysicsComponent", "AngleIncrement",
                        Math.Min(GrapplePhysicsComponent.DistanceToIncrement(Math.Sqrt(Math.Pow(actualPlayerVelocity.X, 2) + Math.Pow(actualPlayerVelocity.Y, 2)), this), 0.1));

                    bool clockwise = false;
                    if ((playerVelocity.Y < 0 && Player.position.X < CurrentActiveTarget.position.X + CurrentActiveTarget.width / 2) || (playerVelocity.Y > 0 && Player.position.X > CurrentActiveTarget.position.X + CurrentActiveTarget.width / 2))
                    {
                        clockwise = true;
                    }
                    Player.SetAttributeVariable("GrapplePhysicsComponent", "Clockwise", clockwise);

                    Player.SetAttributeVariable("MovementComponent", "Velocity", new Point(0, 0));
                }
            }
            else
            {
                if (CurrentActiveTarget != null)
                {
                    Disconnect(mouseState);
                }
            }
        }

        if ((bool)Player.GetAttributeVariable("MovementComponent", "Grounded") || (bool)Player.GetAttributeVariable("MovementComponent", "Ceilinged") || (bool)Player.GetAttributeVariable("MovementComponent", "RightWalled") || (bool)Player.GetAttributeVariable("MovementComponent", "LeftWalled"))
        {
            if (CurrentActiveTarget != null)
            {
                Disconnect(mouseState);
                CanConnect = false;
            }
        } else if (mouseState.LeftButton == ButtonState.Released)
        {
            CanConnect = true;
        }

        // Mouse stuff
        if (MousePosition != TargetMousePosition)
        {
            int targetDistance = (int)Math.Floor(Math.Sqrt(Math.Pow(TargetMousePosition.X - MousePosition.X, 2) + Math.Pow(TargetMousePosition.Y - MousePosition.Y, 2)));
            Vector2 targetDistancePoint = Vector2.Normalize(new(TargetMousePosition.X - MousePosition.X, TargetMousePosition.Y - MousePosition.Y));
            if (targetDistance > mouseSpeed)
            {
                Vector2 movementVector = targetDistancePoint * new Vector2(mouseSpeed, mouseSpeed);
                MousePosition += movementVector.ToPoint();
            } else
            {
                Vector2 movementVector = targetDistancePoint * new Vector2(targetDistance, targetDistance);
                MousePosition += movementVector.ToPoint();
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
            if (deltaX < 0) rot += Math.PI;
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

        TargetMousePosition = CurrentActiveTarget.position;
    }

    public void Disconnect(MouseState mouseState)
    {
        Player.SetAttributeVariable("MovementComponent", "Grappling", false);
        if ((TARGETTYPE)CurrentActiveTarget.GetAttributeVariable("TargetComponent", "TargetType") == TARGETTYPE.swing)
        {
            Player.SetAttributeVariable("MovementComponent", "Velocity", (Point)Player.GetAttributeVariable("GrapplePhysicsComponent", "MostRecentMovement") * new Point(2, 2));
        }
        CurrentActiveTarget.SetAttributeVariable("TargetComponent", "Active", false);
        CurrentActiveTarget = null;
    }
}