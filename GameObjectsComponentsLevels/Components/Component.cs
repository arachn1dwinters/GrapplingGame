using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using GrapplingGame.GameObjectsComponentsLevels.GameObjects;

namespace GrapplingGame.GameObjectsComponentsLevels.Components;
public abstract class Component
{
	public GameObject parent;

	public virtual string type { get; set; }

	public Component(GameObject parent)
	{
		// Set parent and initialize
		this.parent = parent;
	}

	float fixedUpdateTimer;
	// Will be called during the update function of Game1
	public virtual void Update(GameTime gameTime)
	{
		fixedUpdateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
		if (fixedUpdateTimer > 1 / 24) {
			FixedUpdate(gameTime);
			fixedUpdateTimer = 0;
		}
	}

	// Will be called 24 times a second
	public virtual void FixedUpdate(GameTime gameTime){}

	// Will be called whenever the parent GameObject is collided with
	public virtual void OnCollision() {}
}