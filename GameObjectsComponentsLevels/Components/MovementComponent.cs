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
using System.Diagnostics;

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
    public bool Grounded;
    bool Climbing;

    public override string type { get; set; }

    // Adding this here because the movement component is exclusive to the player
    public GameObject GrappleGun;


    public MovementComponent(GameObject parent) : base(parent)
    {
        // Set type 
        type = "MovementComponent";

        gravity = GLOBALS.GRAVITY;
    }

    public override void FixedUpdate(GameTime gameTime)
    {
        base.FixedUpdate(gameTime);

        bool baseGrounded = Grounded;

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
        if ((Keyboard.GetState().IsKeyDown(Keys.Space) || Keyboard.GetState().IsKeyDown(Keys.W)) && Grounded)
        {
            velocity.Y = -10;
        }

        // Gravity
        if (!Grounded && !Climbing && velocity.Y < 10)
        {
            velocity.Y += 1;
        }

        if (velocity.Y > 0 && Grounded)
        {
            velocity.Y = 0;
        }
        movement.Y += velocity.Y;
        movement.X += velocity.X;

        // Final movements
        if (movement.X != 0)
        {
            MoveX(movement.X);
            CheckIfGrounded();
        }

        if (movement.Y != 0)
        {
            MoveY(movement.Y);
            CheckIfGrounded();
        }

        if (velocity.Y < 0 || movement.Y < 0)
        {
            Grounded = false;
        }

        // Reset movement
        movement.Y = 0; movement.X = 0;

        if (Grounded != baseGrounded)
        {
            parent.SetAttributeVariable("PlayerManager", "Grounded", Grounded);
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
            Point tipOfGrappleGun = (Point)GrappleGun.GetAttributeVariable("GrappleGunComponent", "TipOfGun");
            if (!(bool)collision[0])
            {
                parent.position.X += i;
                GrappleGun.position.X += i;
                tipOfGrappleGun.X += i;
                Climbing = false;
            } else
            {
                parent.position.X += (int)collision[1];
                GrappleGun.position.X += (int)collision[1];
                tipOfGrappleGun.X += (int)collision[1];

                if ((string)collision[2] == "ladder")
                {
                    movement.Y -= 10;
                    Climbing = true;
                } else
                {
                    Climbing = false;
                }
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
            Point tipOfGrappleGun = (Point)GrappleGun.GetAttributeVariable("GrappleGunComponent", "TipOfGun");
            if (!(bool)collision[0])
            {
                parent.position.Y += i;
                GrappleGun.position.Y += i;
                tipOfGrappleGun.Y += i;
            } else
            {
                parent.position.Y += (int)collision[1];
                GrappleGun.position.Y += (int)collision[1];
                tipOfGrappleGun.Y += (int)collision[1];
                break;
            }
        }
    }

    // Checks if the game object is Grounded. If so, it sets Grounded to be true
    void CheckIfGrounded()
    {
        List<object> collision = Collision.CheckYCollision(1, parent);
        if ((bool)collision[0])
        {
            Grounded = true;
        } else {
            Grounded = false;
        }
    }
}