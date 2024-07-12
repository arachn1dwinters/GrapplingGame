/* Movement and collision system inspired by https://gamesfromearth.medium.com/a-simple-2d-physics-system-for-platform-games-f430718ea77f
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
    // Movement and speed
    readonly int speed = 2;
    public Point Movement = new();
    public bool rightSideUp = true;

    // The step count of every Movement: spriteWidth + the width of the smallest possible tile - 1
    int xStep, yStep;

    // Physics and Velocity
    public Point Velocity;
    int gravity;
    public bool Grounded;
    bool climbing;
    public bool Grappling;

    public override string type { get; set; }

    public MovementComponent(GameObject parent) : base(parent)
    {
        // Set type 
        type = "MovementComponent";
    }

    public override void FixedUpdate(GameTime gameTime)
    {
        base.FixedUpdate(gameTime);

        bool baseGrounded = Grounded;

        // Set step counts
        xStep = parent.width * parent.sizeMultiplier.X + (12 - 1);
        yStep = parent.height * parent.sizeMultiplier.X + (12 - 1);

        // Move X
        if (!Grappling)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                Movement.X += -speed;
            } else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                Movement.X += speed;
            } if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                Movement.X += -speed;
            } else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                Movement.X += speed;
            } if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                Movement.X += -speed;
            } else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                Movement.X += speed;
            } if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                Movement.X += -speed;
            } else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                Movement.X += speed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                Movement.X += -speed;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                Movement.X += speed;
            }
        }

        // Jump
        if ((Keyboard.GetState().IsKeyDown(Keys.Space) || Keyboard.GetState().IsKeyDown(Keys.W)) /*&& Grounded*/)
        {
            Velocity.Y = -10;
        }
        
        // Momentum on the X axis
        if (Velocity.X != 0)
        {
            Velocity.X = Velocity.X -= 1;
        }

        // Gravity
        if (!Grounded && !climbing && !Grappling && Velocity.Y < 10)
        {
            Velocity.Y += 1;
        }

        if (Velocity.Y > 0)
        {
            if (Grounded || Grappling || climbing)
            {
                Velocity.Y = 0;
            }
        }
        Movement.Y += Velocity.Y;
        Movement.X += Velocity.X;

        // Final Movements
        if (Movement.X != 0)
        {
            MoveX(Movement.X);
            CheckIfGrounded();
        }

        if (Movement.Y != 0)
        {
            MoveY(Movement.Y);
            CheckIfGrounded();
        }

        if (Velocity.Y < 0 || Movement.Y < 0)
        {
            Grounded = false;
        }

        // Reset Movement
        Movement.Y = 0; Movement.X = 0;

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
            Point tipOfGrappleGun = (Point)parent.parent.GrappleGun.GetAttributeVariable("GrappleGunComponent", "TipOfGun");
            if (!(bool)collision[0])
            {
                parent.position.X += i;
                parent.parent.GrappleGun.position.X += i;
                tipOfGrappleGun.X += i;
                climbing = false;
            } else
            {
                parent.position.X += (int)collision[1];
                parent.parent.GrappleGun.position.X += (int)collision[1];
                tipOfGrappleGun.X += (int)collision[1];

                if ((string)collision[2] == "ladder")
                {
                    Movement.Y -= 10;
                    climbing = true;
                } else
                {
                    climbing = false;
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
            Point tipOfGrappleGun = (Point)parent.parent.GrappleGun.GetAttributeVariable("GrappleGunComponent", "TipOfGun");
            if (!(bool)collision[0])
            {
                parent.position.Y += i;
                parent.parent.GrappleGun.position.Y += i;
                tipOfGrappleGun.Y += i;
            } else
            {
                parent.position.Y += (int)collision[1];
                parent.parent.GrappleGun.position.Y += (int)collision[1];
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