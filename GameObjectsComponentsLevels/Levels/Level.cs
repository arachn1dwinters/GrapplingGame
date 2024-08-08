using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
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

    // Screens
    public virtual string TiledMap { get; set; }
    public virtual List<Screen> Screens { get; set; }
    public int ScreenIndex = 0;
    public string FolderName;
    public GameObject Curtain;
    bool CurtainDown;

    // Gameplay
    public GameObject Player;
    public Point PlayerSpawn;
    public GameObject GrappleGun;
    public GameObject CurrentActiveTarget;
    public GameObject HoveredTarget; 
    public int DistanceFromTarget;
    public Point MousePosition;
    public Point TargetMousePosition;
    public bool CanConnect = true;
    int mouseSpeed = 50;
    public List<GameObject> Targets = new();

    public Level(GameManager parent, bool respawn)
    {
        GameObjects = new();

        if (!respawn)
        {
            parent.levels.Add(this);
        }

        Curtain = new(GameManager.Instance.CurtainSheet, new Point(0, 0), new Point(4, 4), "Curtain", this, new()
        {
            "AnimationComponent"
        })
        {
            collidable = false,
            Visible = false
        };

        Curtain.SetComponentVariable("AnimationComponent", "frameWidth", 400);
        Curtain.SetComponentVariable("AnimationComponent", "frameHeight", 225);
        Curtain.SetComponentVariable("AnimationComponent", "animationLengths", new List<int>()
        {
            16,
            16,
        });
        Curtain.SetComponentVariable("AnimationComponent", "Events", new List<Event>()
        {
            new(NextScreen, 0)
        });
        Curtain.SetComponentVariable("AnimationComponent", "timeBetweenFrames", 0.03);
    }

    public virtual void Update()
    {
        MouseState mouseState = Mouse.GetState();
        Vector2 mousePos = GameManager.Instance.OrthographicCamera.ScreenToWorld(new Vector2(mouseState.X, mouseState.Y));

        if (CurrentActiveTarget == null)
        {
            List<GameObject> hoveredTargets = new();
            int hoverDistance = 2000;
            foreach (GameObject target in Targets)
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

        if (!(bool)Player.GetComponentVariable("MovementComponent", "Grounded") && !(bool)Player.GetComponentVariable("MovementComponent", "Ceilinged") && !(bool)Player.GetComponentVariable("MovementComponent", "RightWalled") && !(bool)Player.GetComponentVariable("MovementComponent", "LeftWalled") && CanConnect)
        {
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (CurrentActiveTarget == null && HoveredTarget != null)
                {
                    SwitchTarget(HoveredTarget);
                    Point playerVelocity = (Point)Player.GetComponentVariable("MovementComponent", "Velocity");
                    Point playerMovement = (Point)Player.GetComponentVariable("MovementComponent", "Movement");
                    Point actualPlayerVelocity = playerVelocity + playerMovement;
                    Player.SetComponentVariable("GrapplePhysicsComponent", "AngleIncrement",
                        Math.Min(GrapplePhysicsComponent.DistanceToIncrement(Math.Sqrt(Math.Pow(actualPlayerVelocity.X, 2) + Math.Pow(actualPlayerVelocity.Y, 2)), this), 0.1));

                    bool clockwise = false;
                    if ((playerVelocity.Y < 0 && Player.position.X < CurrentActiveTarget.position.X + CurrentActiveTarget.width / 2) || (playerVelocity.Y > 0 && Player.position.X > CurrentActiveTarget.position.X + CurrentActiveTarget.width / 2))
                    {
                        clockwise = true;
                    }
                    Player.SetComponentVariable("GrapplePhysicsComponent", "Clockwise", clockwise);

                    Player.SetComponentVariable("MovementComponent", "Velocity", new Point(0, 0));
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

        if ((bool)Player.GetComponentVariable("MovementComponent", "Grounded") || (bool)Player.GetComponentVariable("MovementComponent", "Ceilinged") || (bool)Player.GetComponentVariable("MovementComponent", "RightWalled") || (bool)Player.GetComponentVariable("MovementComponent", "LeftWalled"))
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

        // Next screen
        if (ScreenIndex + 1 != Screens.Count && !Screens[ScreenIndex].Bounds.Intersects(Player.rect))
        {
            ScreenIndex += 1;

            Curtain.Visible = true;
            Curtain.position = GameManager.Instance.OrthographicCamera.ScreenToWorld(Vector2.Zero).ToPoint();
            Curtain.SetComponentVariable("AnimationComponent", "currentAnimation", 0);
            Curtain.SetComponentVariable("AnimationComponent", "playing", true);
        }

        if (CurtainDown)
        {
            Curtain.SetComponentVariable("AnimationComponent", "currentAnimation", 1);
            Curtain.SetComponentVariable("AnimationComponent", "playing", true);
            CurtainDown = false;
        }
    }

    public void NextScreen()
    {
        CurtainDown = true;
        Targets.Clear();
        TiledMap = FolderName + Screens[ScreenIndex].Path;
        GameManager.Instance.CreateMap(TiledMap);

        Player.position = PlayerSpawn;
        GrappleGun.position = PlayerSpawn + new Point(32, 32);
        Player.SetComponentVariable("MovementComponent", "Velocity", Point.Zero);
        Camera.Instance.TargetCenter = new(GameManager.Instance.HalfScreenWidth, GameManager.Instance.HalfScreenHeight);
        GameManager.Instance.OrthographicCamera.Position = Vector2.Zero;
        GameManager.Instance.SpriteTint = Color.White;
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

        Player.SetComponentVariable("GrapplePhysicsComponent", "Angle", rot);

        target.SetComponentVariable("TargetComponent", "Active", true);
        CurrentActiveTarget = target;
        Player.SetComponentVariable("MovementComponent", "Grappling", true);

        TargetMousePosition = CurrentActiveTarget.position;
    }

    public void Disconnect(MouseState mouseState)
    {
        Player.SetComponentVariable("MovementComponent", "Grappling", false);
        if ((TARGETTYPE)CurrentActiveTarget.GetComponentVariable("TargetComponent", "TargetType") == TARGETTYPE.swing)
        {
            Player.SetComponentVariable("MovementComponent", "Velocity", (Point)Player.GetComponentVariable("GrapplePhysicsComponent", "MostRecentMovement") * new Point(2, 2));
        }
        CurrentActiveTarget.SetComponentVariable("TargetComponent", "Active", false);
        CurrentActiveTarget = null;
    }
}

public struct Screen
{
    public Rectangle Bounds;
    public string Path; // In general, another string with the name of the level folder (e.g. "Level1/") will be added to this one.

    public Screen(Rectangle Bounds, string Path)
    {
        this.Bounds = Bounds;
        this.Path = Path;
    }
}