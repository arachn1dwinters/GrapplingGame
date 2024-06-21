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

	// Will be called during the update function of Game1
	public virtual void Update(GameTime gameTime){}

	// Will be called whenever the parent GameObject is collided with
	public virtual void OnCollision() {}
}