/* Movement system inspired by https://gamesfromearth.medium.com/a-simple-2d-physics-system-for-platform-games-f430718ea77f
* & https://maddythorson.medium.com/celeste-and-towerfall-physics-d24bd2ae0fc5
* Thank you to Maddy Thornson and Games From Earth!
*/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using GrapplingGame.GameObjectsComponentsLevels.GameObjects;
using GrapplingGame.GameObjectsComponentsLevels.Helpers;

namespace GrapplingGame.GameObjectsComponentsLevels.Components;
public class MovementComponent : Component
{
    // Movement and speed2
    readonly int speed = 4;
    Point movement = new();
    public bool rightSideUp = true;

    // The step count of every movement: spriteWidth + the width of the smallest possible tile - 1
    int xStep, yStep;

    // Physics and velocity
    Point velocity;
    int gravity;
    public bool grounded;

    public override string type { get; set; }

    // Adding this here because the movement component is exclusive to the player
    public GameObject GrappleGun;


    public MovementComponent(GameObject parent) : base(parent)
    {
        // Set type 
        type = "MovementComponent";

        gravity = GLOBALS.GRAVITY;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        bool baseGrounded = grounded;

        // Set step counts
        xStep = parent.width * parent.sizeMultiplier.X + (12 - 1);
        yStep = parent.height * parent.sizeMultiplier.X + (12 - 1);

        // Move X
        if (Keyboard.GetState().IsKeyDown(Keys.A))
        {
            movement.X += -speed;
        } else if (Keyboard.GetState().IsKeyDown(Keys.D))
        {
            movement.X += speed;
        }

        // Jump
        if ((Keyboard.GetState().IsKeyDown(Keys.Space) || Keyboard.GetState().IsKeyDown(Keys.W)) && grounded)
        {
            velocity.Y = -10;
        }

        // Velocity
        if (!grounded && velocity.Y < 10)
        {
            velocity.Y += 1;
        }

        if (velocity.Y > 0 && grounded)
        {
            velocity.Y = 0;
        }
        movement.Y += velocity.Y;
        movement.X += velocity.X;

        // Final movements
        if (movement.X != 0)
        {
            MoveX(movement.X);
        }

        if (movement.Y != 0)
        {
            MoveY(movement.Y);
            CheckIfGrounded();
        }

        if (velocity.Y < 0 || movement.Y < 0)
        {
            grounded = false;
        }

        // Reset movement
        movement.Y = 0; movement.X = 0;

        if (grounded != baseGrounded)
        {
            parent.SetAttributeVariable("PlayerManager", "grounded", grounded);
        }
    }

    public void MoveX(int amount)
    {
        List<int> steps = new();

        if (amount > xStep)
        {
            // Loop through every possible step
            for (int i = 0; i <= amount / xStep; i++)
            {
                steps.Add(xStep);
            }
        }
        // Add the remainder
        steps.Add(amount % xStep);

        // Loop through every step
        foreach (int i in steps)
        {
            List<object> collision = Collision.CheckXCollision(i, parent);
            if (!(bool)collision[0])
            {
                parent.position.X += i;
                GrappleGun.position.X += i;
            } else
            {
                parent.position.X += (int)collision[1];
                GrappleGun.position.X += (int)collision[1];
                break;
            }
        }
    }

    // The same MoveX function, but flipped for the Y axis
    public void MoveY(int amount)
    {
        List<int> steps = new();

        if (amount > yStep)
        {
            // Loop through every possible step
            for (int i = 0; i <= amount / yStep; i++)
            {
                // Add that step as long as it isn't colliding with anything
                steps.Add(yStep);
            }
        }
        // Add the remainder
        steps.Add(amount % yStep);

        // Loop through every step
        foreach (int i in steps)
        {
            // Add that step as long as it isn't colliding with anything
            List<object> collision = Collision.CheckYCollision(i, parent);

            if (!(bool)collision[0])
            {
                parent.position.Y += i;
                GrappleGun.position.Y += i;
            } else
            {
                parent.position.Y += (int)collision[1];
                GrappleGun.position.Y += (int)collision[1];
                break;
            }
        }
    }

    // Checks if the game object is grounded. If so, it sets grounded to be true
    void CheckIfGrounded()
    {
        List<object> collision = Collision.CheckYCollision(1, parent);
        if ((bool)collision[0])
        {
            grounded = true;
        } else {
            grounded = false;
        }
    }
}