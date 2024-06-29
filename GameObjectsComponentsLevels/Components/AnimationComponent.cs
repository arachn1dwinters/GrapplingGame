// This attribute assumes that the sprite that the parent object is using is a spritesheet where each row is an animation
/* To set up this attribute, add the animation to your object, and use something like this example code to set it up:
        button.SetAttributeVariable("AnimationAttribute", "frameHeight", 16);
        button.SetAttributeVariable("AnimationAttribute", "frameWidth", 32);
        button.SetAttributeVariable("AnimationAttribute", "animationLengths", new List<int>()
        {
            2,
            2,
        });

To run an animation, use something like this code:
        parent.SetAttributeVariable("AnimationAttribute", "currentAnimation", 0);
        parent.SetAttributeVariable("AnimationAttribute", "playing", true);
        parent.SetAttributeVariable("AnimationAttribute", "looping", true);
*/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using GrapplingGame.GameObjectsComponentsLevels.GameObjects;

namespace GrapplingGame.GameObjectsComponentsLevels.Components;
public class AnimationComponent : Component
{
    public override string type { get; set; }

    // Frames
    public int frameHeight, frameWidth;
    public List<int> animationLengths;
    public bool setParentDimensions;

    // Animation Logic
    public bool playing;
    public bool looping;
    public int currentAnimation;
    int currentFrame;
    List<Animation> animations;

    string currentText;

    public double timeBetweenFrames;
    float timer;                

    public AnimationComponent(GameObject parent) : base(parent)
    {
        type = "MovementComponent";

        // Frames/Animations
        parent.cropped = true;
        timeBetweenFrames = Helpers.GLOBALS.TIMEBETWEENFRAMES;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        currentText = timer.ToString();

        // Create the animations list once the animation lengths have been established
        if (animationLengths != null && animations == null)
        {
            CreateAnimationList();
            parent.cropRect = animations[0].frames[0];
        }

        // Create default crop rect
        if (frameHeight != 0 && frameWidth != 0 && !setParentDimensions)
        {
            // Add parent's height and width
            parent.height = frameHeight;
            parent.width = frameWidth;
            setParentDimensions = true;
        }

        if (playing)
        {
            // run animation
            if (timer > timeBetweenFrames)
            {
                timer = 0;

                Animation animation = animations[currentAnimation];

                if (currentFrame == animation.length)
                {
                    if (looping)
                    {
                        // Restart
                        parent.cropRect = animation.frames[0];
                    } else
                    {
                        // Return to default frame
                        parent.cropRect = animations[0].frames[0];
                        playing = false;
                    }
                    currentFrame = 0;
                } else
                {
                    parent.cropRect = animation.frames[currentFrame];
                }
                currentFrame++;
            }
        }
    }

    void CreateAnimationList()
    {
        animations = new();

        // Loop through all animations
        for (int i = 0; i < animationLengths.Count; i++)
        {
            // Create animation
            Animation newAnimation = new(animationLengths[i]);
            animations.Add(newAnimation);

            // Add frames
            for (int e = 0; e < newAnimation.length; e++)
            {
                newAnimation.frames.Add(new(e * frameWidth, i * frameHeight, frameWidth, frameHeight));
            }
        }
    }

    public void SetToDefault()
    {
        if (animations != null)
        {
            parent.cropRect = animations[0].frames[0];
        }
    }

    struct Animation
    {
        public int length;
        public List<Rectangle> frames;

        public Animation(int length)
        {
            frames = new();
            this.length = length;
        }
    }
}