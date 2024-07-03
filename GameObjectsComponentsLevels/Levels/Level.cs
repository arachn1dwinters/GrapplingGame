using System;
using System.Collections.Generic;

using GrapplingGame.GameObjectsComponentsLevels.Helpers;
using GrapplingGame.GameObjectsComponentsLevels.GameObjects;
using GrapplingGame;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using GrapplingGame.GameObjectsComponentsLevels.Components;

namespace GrapplingGame.GameObjectsComponentsLevels.Levels;
public class Level
{
	public virtual List<GameObject> GameObjects { get; set; }
    public List<GameObject> targets = new();

	public virtual string tiledMap { get; set; }
	public GameManager parent;
	public int index;

    public GameObject Player;
    public GameObject GrappleGun;

    public GameObject CurrentActiveTarget;
    public int DistanceFromTarget;

    public Level(GameManager parent, bool respawn)
	{
		GameObjects = new();
		this.parent = parent;

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
            foreach (GameObject target in targets)
            {
                if (target != CurrentActiveTarget)
                {
                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        int distance = (int)Math.Floor(Math.Pow(mousePos.X - target.position.X, 2) + Math.Pow(mousePos.Y - target.position.Y, 2));
                        if (distance < 700)
                        {
                            DistanceFromTarget = (int)Math.Sqrt(Math.Pow(GrappleGun.position.X - target.position.X, 2) + Math.Pow(GrappleGun.position.Y - target.position.Y, 2));
                            CurrentActiveTarget = target;
                            SwitchTargets(target, CurrentActiveTarget);

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
                            } else
                            {
                                if (deltaY > 0)
                                {
                                    rot = Math.PI / 2;
                                } else
                                {
                                    rot = -Math.PI / 2;
                                }
                            }

                            Player.SetAttributeVariable("GrapplePhysicsComponent", "Angle", rot);
                        }
                    }
                }
            }

            if (mouseState.LeftButton == ButtonState.Released && CurrentActiveTarget != null)
            {
                CurrentActiveTarget.SetAttributeVariable("TargetComponent", "Active", false);
                CurrentActiveTarget = null;
                Point playerVelocity = (Point)Player.GetAttributeVariable("MovementComponent", "Velocity");
                playerVelocity.Y = 10;

                Player.SetAttributeVariable("MovementComponent", "Grappling", false);
                Player.SetAttributeVariable("MovementComponent", "Velocity", Player.GetAttributeVariable("GrapplePhysicsComponent", "MostRecentMovement"));
            }
        }
    }
	public virtual void Initialize() { }

	public void Remove()
	{
		parent.levels.Remove(this);
	}

    void SwitchTargets(GameObject newTarget, GameObject previousTarget = null)
    {
        if (previousTarget != null)
        {
            previousTarget.SetAttributeVariable("TargetComponent", "Active", false);
            DistanceFromTarget = (int)Math.Sqrt(Math.Pow(Player.position.X - newTarget.position.X, 2) + Math.Pow(Player.position.Y - newTarget.position.Y, 2));

            CurrentActiveTarget = newTarget;
        } else
        {
            CurrentActiveTarget = newTarget;
        }
        newTarget.SetAttributeVariable("TargetComponent", "Active", true);
    }
}