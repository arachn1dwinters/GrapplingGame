/* Movement and collision system inspired by https://gamesfromearth.medium.com/a-simple-2d-physics-system-for-platform-games-f430718ea77f
   & https://maddythorson.medium.com/celeste-and-towerfall-physics-d24bd2ae0fc5
   Thank you to Maddy Thorson and Games From Earth!
*/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using GrapplingGame.GameObjectsComponentsLevels.GameObjects;
using GrapplingGame.GameObjectsComponentsLevels.Helpers;
using static GrapplingGame.GameObjectsComponentsLevels.Helpers.GLOBALS;

namespace GrapplingGame.GameObjectsComponentsLevels.Components;
public class MovementComponent : Component
{
    // Movement and speed2
    int speed = 2;
    Vector2Int movement = new();
    int gravity;
    public bool rightSideUp = true;

    // The widht of the parent sprite
    int spriteWidth, spriteHeight;
    bool setDimensions;

    // The step count of every movement: spriteWidth + the width of the smallest possible tile - 1
    int xStep, yStep;

    bool grounded;

    public override string type { get; set; }

    public MovementComponent(GameObject parent) : base(parent)
    {
        // Set type 
        type = "MovementComponent";

        gravity = GRAVITY;
    }

    public override void Update(GameTime gameTime)
    {
        /*if ((bool)parent.GetAttributeVariable("AnimationAttribute", "setParentDimensions") && !setDimensions)
        {
            spriteWidth = parent.width;
            spriteHeight = parent.height;

            setDimensions = true;
        }*/

        // Set step counts
        xStep = spriteWidth * parent.sizeMultiplier.x + (12 - 1);
        yStep = spriteHeight * parent.sizeMultiplier.y + (12 - 1);

        // MoveX
        // Add these to the next to if statements if you decide to put animation back on:  && (bool)parent.GetAttributeVariable("AnimationAttribute", "setParentDimensions")
        if (Keyboard.GetState().IsKeyDown(Keys.A))
        {
            movement.x += -speed;

            // Run the animation
            /*parent.SetAttributeVariable("AnimationAttribute", "currentAnimation", 0);
            parent.SetAttributeVariable("AnimationAttribute", "playing", true);
            parent.SetAttributeVariable("AnimationAttribute", "looping", true);*/
        } else if (Keyboard.GetState().IsKeyDown(Keys.D))
        {
            movement.x += speed;

            // Run the animation
            /*parent.SetAttributeVariable("AnimationAttribute", "currentAnimation", 0);
            parent.SetAttributeVariable("AnimationAttribute", "playing", true);
            parent.SetAttributeVariable("AnimationAttribute", "looping", true);*/
        } else
        {
            /*parent.SetAttributeVariable("AnimationAttribute", "playing", false);
            parent.CallAttributeMethod("AnimationAttribute", "SetToDefault");*/
        }

        // MoveY
        // Add these to the next to if statements if you decide to put animation back on:  && (bool)parent.GetAttributeVariable("AnimationAttribute", "setParentDimensions")
        if (Keyboard.GetState().IsKeyDown(Keys.W))
        {
            movement.y += -speed;

            // Run the animation
            /*parent.SetAttributeVariable("AnimationAttribute", "currentAnimation", 0);
            parent.SetAttributeVariable("AnimationAttribute", "playing", true);
            parent.SetAttributeVariable("AnimationAttribute", "looping", true);*/
        } else if (Keyboard.GetState().IsKeyDown(Keys.S))
        {
            movement.y += speed;

            // Run the animation
            /*parent.SetAttributeVariable("AnimationAttribute", "currentAnimation", 0);
            parent.SetAttributeVariable("AnimationAttribute", "playing", true);
            parent.SetAttributeVariable("AnimationAttribute", "looping", true);*/
        } else
        {
            /*parent.SetAttributeVariable("AnimationAttribute", "playing", false);
            parent.CallAttributeMethod("AnimationAttribute", "SetToDefault");*/
        }

        // Final movements
        if (movement.x != 0)
        {
            MoveX(movement.x);
        }

        if (movement.y != 0)
        {
            MoveY(movement.y);
        }

        // Reset movement
        movement.y = 0; movement.x = 0;
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
                parent.position.x += i;
            } else
            {
                parent.position.x += (int)collision[1];
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
                parent.position.y += i;
            } else
            {
                parent.position.y += (int)collision[1];
                break;
            }
        }
    }

    public void SetToRightSideUp()
    {
            gravity = GRAVITY;
            rightSideUp = true;
    }

    public void SetToWrongSideUp()
    {
        gravity = -GRAVITY;
        rightSideUp = false;
    }
}