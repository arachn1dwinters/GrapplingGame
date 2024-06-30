using System;
using System.Collections.Generic;

using GrapplingGame.GameObjectsComponentsLevels.Helpers;
using GrapplingGame.GameObjectsComponentsLevels.GameObjects;
using GrapplingGame;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace GrapplingGame.GameObjectsComponentsLevels.Levels;
public class Level
{
	public virtual List<GameObject> GameObjects { get; set; }
    public List<GameObject> targets = new();

	public virtual string tiledMap { get; set; }
	public GameManager parent;
	public int index;

    public GameObject Player;

    public GameObject CurrentActiveTarget;

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
                            if (CurrentActiveTarget != null)
                            {
                                SwitchTargets(CurrentActiveTarget, target);
                            }
                            else
                            {
                                CurrentActiveTarget = target;
                                target.SetAttributeVariable("TargetComponent", "Active", true);
                            }
                        }
                    }
                }
            }

            if (mouseState.LeftButton == ButtonState.Released && CurrentActiveTarget != null)
            {
                CurrentActiveTarget.SetAttributeVariable("TargetComponent", "Active", false);

                CurrentActiveTarget = null;
            }
        }
    }
	public virtual void Initialize() { }

	public void Remove()
	{
		parent.levels.Remove(this);
	}

    void SwitchTargets(GameObject previousTarget, GameObject newTarget)
    {
        // Put some sort of cool lerp animation here(later)
        newTarget.SetAttributeVariable("TargetComponent", "Active", true);
        previousTarget.SetAttributeVariable("TargetComponent", "Active", false);

        CurrentActiveTarget = newTarget;
    }
}