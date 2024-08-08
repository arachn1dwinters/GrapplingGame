// This attribute assumes that the sprite that the parent object is using is a spritesheet where each row is an animation
/* To set up this attribute, add the animation to your object, and use something like this example code to set it up:
        button.SetComponentVariable("AnimationComponent", "frameHeight", 16);
        button.SetComponentVariable("AnimationComponent", "frameWidth", 32);
        button.SetComponentVariable("AnimationComponent", "animationLengths", new List<int>()
        {
            2,
            2,
        });

To run an animation, use something like this code:
        button.SetComponentVariable("AnimationComponent", "currentAnimation", 0);
        button.SetComponentVariable("AnimationComponent", "playing", true);
        button.SetComponentVariable("AnimationComponent", "looping", true);
*/

using System.Collections.Generic;
using Microsoft.Xna.Framework;

using GrapplingGame.GameObjectsComponentsLevels.GameObjects;
using System;
using System.Diagnostics;

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
    // Events are ran at the end of an animation
    public List<Event> Events = new();

    string currentText;

    public double timeBetweenFrames;
    float timer;                

    public AnimationComponent(GameObject parent) : base(parent)
    {
        type = "AnimationComponent";

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
                    // Run any events associated with the current animation
                    foreach (Event e in Events)
                    {
                        if (e.AnimationIndex == currentAnimation)
                        {
                            e.Method();
                        }
                    }

                    if (looping)
                    {
                        // Restart
                        parent.cropRect = animation.frames[0];
                    } else
                    {
                        // Return to default frame
                        //parent.cropRect = animations[0].frames[0];
                        currentFrame = 0;
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

public class Event
{
    public Action Method;
    public int AnimationIndex = 0;

    public Event(Action method, int animationIndex)
    {
        this.Method = method;
        this.AnimationIndex = animationIndex;
    }
}