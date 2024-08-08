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
    // Movement and speed
    readonly int speed = 10;
    public Point Movement = new();
    public bool rightSideUp = true;

    // The step count of every Movement: spriteWidth + the width of the smallest possible tile - 1
    int xStep, yStep;

    // Physics and Velocity
    public Point Velocity;
    public int Gravity = 12;
    public int BaseGravity = 10;
    public bool Grounded;
    public bool Ceilinged;
    public bool LeftWalled;
    public bool RightWalled;
    bool climbing;
    public bool Grappling;
    public bool Pulling;

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
        xStep = parent.width * parent.sizeMultiplier.X + (16 - 1);
        yStep = parent.height * parent.sizeMultiplier.Y + (16 - 1);

        // Move X
        if (!Grappling)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                Movement.X += -speed;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                Movement.X += speed;
            }
        }

        // Gravity
        if (!Grounded && !climbing && !Grappling)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                Gravity = BaseGravity + 5;
            } else
            {
                Gravity = BaseGravity;
            }

            if (Velocity.Y < Gravity)
            {
                Velocity.Y += 1;
            }
        }

        if (Grounded)
        {
            Velocity.Y = 0;
        }

        // Jump
        if ((Keyboard.GetState().IsKeyDown(Keys.Space) || Keyboard.GetState().IsKeyDown(Keys.W)) && Grounded)
        {
            Velocity.Y = -10;
        }

        // Momentum on the X axis
        if (Velocity.X != 0)
        {
            if (!Grounded) Velocity.X = Velocity.X < 0 ? Velocity.X + 1 : Velocity.X - 1;

            if (Grounded)
            {
                int xVelocity = Math.Abs(Velocity.X);
                Velocity.X = Velocity.X < 0 ? Velocity.X + xVelocity : Velocity.X - xVelocity;
            }
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
            CheckIfGrounded(new());
        }

        if (Movement.Y != 0)
        {
            MoveY(Movement.Y);
            CheckIfGrounded(new());
        }


        if (Velocity.Y < 0 || Movement.Y < 0)
        {
            Grounded = false;
        }

        // Move camera
        //Camera.Instance.TargetCenter = parent.position;

        // Reset Movement
        Movement.Y = 0; Movement.X = 0;

        if (Grounded != baseGrounded)
        {
            parent.SetComponentVariable("PlayerManager", "Grounded", Grounded);
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
            Point tipOfGrappleGun = (Point)parent.parent.GrappleGun.GetComponentVariable("GrappleGunComponent", "TipOfGun");
            if (!(bool)collision[0])
            {
                parent.position.X += i;
                parent.parent.GrappleGun.position.X += i;
                parent.parent.GrappleGun.SetComponentVariable("GrappleGunComponent", "TipOfGun", new Point(tipOfGrappleGun.X + i, tipOfGrappleGun.Y));
                climbing = false;
            }
            else
            {
                parent.position.X += (int)collision[1];
                parent.parent.GrappleGun.position.X += (int)collision[1];
                tipOfGrappleGun.X += (int)collision[1];
                parent.parent.GrappleGun.SetComponentVariable("GrappleGunComponent", "TipOfGun", new Point(tipOfGrappleGun.X + (int)collision[1], tipOfGrappleGun.Y));

                if ((string)collision[2] == "ladder")
                {
                    Movement.Y -= 10;
                    climbing = true;
                }
                else
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
        // Add the remainder.

        steps.Add(amount % yStep);

        // Loop through every step
        foreach (int i in steps)
        {
            // Add that step as long as it isn't colliding with anything
            List<object> collision = Collision.CheckYCollision(i, parent);
            Point tipOfGrappleGun = (Point)parent.parent.GrappleGun.GetComponentVariable("GrappleGunComponent", "TipOfGun");
            if (!(bool)collision[0])
            {
                parent.position.Y += i;
                parent.parent.GrappleGun.position.Y += i;
                parent.parent.GrappleGun.SetComponentVariable("GrappleGunComponent", "TipOfGun", new Point(tipOfGrappleGun.X, tipOfGrappleGun.Y + i));
            }
            else
            {
                parent.position.Y += (int)collision[1];
                parent.parent.GrappleGun.position.Y += (int)collision[1];
                tipOfGrappleGun.Y += (int)collision[1];
                parent.parent.GrappleGun.SetComponentVariable("GrappleGunComponent", "TipOfGun", new Point(tipOfGrappleGun.X, tipOfGrappleGun.Y + (int)collision[1]));
                break;
            }
        }
    }

    // Check if the game object is touching a wall. If so, set Grounded to be true
    void CheckIfGrounded(MouseState mouseState)
    {
        // Check if Grounded
        List<object> collision = Collision.CheckYCollision(1, parent);
        if ((bool)collision[0])
        {
            Grounded = true;
            if (Velocity.Y > 0) Velocity.Y = 0;
        }
        else
        {
            Grounded = false;
        }

        // Check if Ceilinged
        collision = Collision.CheckYCollision(-1, parent);
        if ((bool)collision[0])
        {
            Ceilinged = true;
            if (Velocity.Y < 0) Velocity.Y = 0;
        }
        else
        {
            Ceilinged = false;
        }

        // Check if RightWalled
        collision = Collision.CheckXCollision(1, parent);
        if ((bool)collision[0])
        {
            RightWalled = true;
            if (Velocity.X > 0) Velocity.X = 0;
        }
        else
        {
            RightWalled = false;
        }

        // Check if LeftWalled
        collision = Collision.CheckXCollision(-1, parent);
        if ((bool)collision[0])
        {
            LeftWalled = true;
            if (Velocity.X < 0) Velocity.X = 0;
        }
        else
        {
            LeftWalled = false;
        }
    }
}