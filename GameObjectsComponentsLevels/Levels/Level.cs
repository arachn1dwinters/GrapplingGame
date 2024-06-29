using System;
using System.Collections.Generic;

using GrapplingGame.GameObjectsComponentsLevels.Helpers;
using GrapplingGame.GameObjectsComponentsLevels.GameObjects;
using GrapplingGame;

namespace GrapplingGame.GameObjectsComponentsLevels.Levels;
public class Level
{
	public virtual List<GameObject> GameObjects { get; set; }
    public List<GameObject> targets = new();

	public virtual string tiledMap { get; set; }
	public GameManager parent;
	public int index;

    public GameObject Player;

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

	public virtual void Update(){ }
	public virtual void Initialize() { }

	public void Remove()
	{
		parent.levels.Remove(this);
	}
}