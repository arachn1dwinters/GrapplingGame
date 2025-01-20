using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GrapplingGame.GameObjectsComponentsLevels.Components;
using GrapplingGame.GameObjectsComponentsLevels.Helpers;
using GrapplingGame.GameObjectsComponentsLevels.Levels;
using System.Diagnostics;
using GrapplingGame;

namespace GrapplingGame.GameObjectsComponentsLevels.GameObjects;
public class GameObject
{
    // Components
    public List<Component> attributes;

    // Sprite and rect
    public Rectangle rect;
    public Texture2D sprite;
    public Point position;
    public Point sizeMultiplier;
    public Rectangle cropRect;
    public int height;
    public int width;
    public bool cropped;
    public Point topLeftCorner = new(0, 0);
    public bool dontRender;
    public float Rotation = 0;
    public Vector2 origin = new(0, 0);
    public bool Visible = true;

    // Parents
    public Level parent;

    // Name
    public string type;

    public bool collidable = true;

    // Regular GameObject
    public GameObject(Texture2D sprite, Point position, Point sizeMultiplier, string type, Level parent, List<string> attributesToAdd = null)
    {
        // Creating attributes, position, size, and sprite
        this.attributes = new();
        this.sprite = sprite;
        this.position = position;
        this.sizeMultiplier = sizeMultiplier;

        // Creating the rectangle
        rect = new(position.X, position.Y, sprite.Width * sizeMultiplier.X, sprite.Height * sizeMultiplier.Y);
        this.width = sprite.Width;
        this.height = sprite.Height;

        // Create attributes
        if (attributesToAdd != null)
        {
            foreach (string i in attributesToAdd)
            {
                AddComponent(i);
            }
        }

        // Set parent
        this.parent = parent;

        // Set name
        this.type = type;

        // Add this to the parent's list of Objects
        parent.GameObjects.Add(this);
    }

    // GameObject without a sprite
    public GameObject(Point size, Point topLeftCorner, Point position, Point sizeMultiplier, string type, Level parent, List<string> attributesToAdd = null)
    {
        // Creating attributes, position, size, and sprite
        this.attributes = new();
        this.position = position;
        this.sizeMultiplier = sizeMultiplier;

        // Creating the rectangle
        rect = new(position.X, position.X, size.X * sizeMultiplier.X, size.Y * sizeMultiplier.Y);
        width = size.X;
        height = size.Y;
        this.topLeftCorner = topLeftCorner;

        // Create attributes
        if (attributesToAdd != null)
        {
            foreach (string i in attributesToAdd)
            {
                AddComponent(i);
            }
        }

        // Set parent
        this.parent = parent;

        // Set name
        this.type = type;

        // Add this to the parent's list of Objects
        parent.GameObjects.Add(this);
    }

    // For a cropped object with a different size
    public GameObject(Texture2D sprite, Rectangle cropRect, Point size, Point topLeftCorner, Point position, Point sizeMultiplier, string type, Level parent, List<string> attributesToAdd = null)
    {
        // Creating attributes, position, size, and sprite
        this.attributes = new();
        this.sprite = sprite;
        this.position = position;
        this.sizeMultiplier = sizeMultiplier;

        // Creating the rectangle
        rect = new(position.X, position.Y, sprite.Width * sizeMultiplier.X, sprite.Height * sizeMultiplier.Y);
        cropped = true;
        this.cropRect = cropRect;
        width = size.X;
        height = size.Y;
        this.topLeftCorner = topLeftCorner;

        // Create attributes
        if (attributesToAdd != null)
        {
            foreach (string i in attributesToAdd)
            {
                AddComponent(i);
            }
        }

        // Set parent
        this.parent = parent;

        // Set name
        this.type = type;

        // Add this to the parent's list of Objects
        parent.GameObjects.Add(this);
    }

    // For a cropped object
    public GameObject(Texture2D sprite, Rectangle cropRect, Point position, Point sizeMultiplier, string type, Level parent, List<string> attributesToAdd = null)
    {
        // Creating attributes, position, size, and sprite
        this.attributes = new();
        this.sprite = sprite;
        this.position = position;
        this.sizeMultiplier = sizeMultiplier;

        // Creating the rectangle
        rect = new(position.X, position.Y, sprite.Width * sizeMultiplier.X, sprite.Height * sizeMultiplier.Y);
        cropped = true;
        this.cropRect = cropRect;
        this.width = cropRect.Width;
        this.height = cropRect.Height;

        // Create attributes
        if (attributesToAdd != null)
        {
            foreach (string i in attributesToAdd)
            {
                AddComponent(i);
            }
        }

        // Set parent
        this.parent = parent;

        // Set name
        this.type = type;

        // Add this to the parent's list of Objects
        parent.GameObjects.Add(this);
    }

    public void AddComponent(string componentName)
    {
        /*Here we use the assembly qualified name of the desired attribute, something like
        MirrorImage.GameObject.MovementAttribute, MirrorImage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null*/
        try
        {
            string attributeToInstantiate = $"GrapplingGame.GameObjectsComponentsLevels.Components.{componentName}, GrapplingGame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
            var attributeType = Type.GetType(attributeToInstantiate);
            var instantiatedAttribute = Activator.CreateInstance(attributeType, this);
            Component newAttribute = (Component)instantiatedAttribute;

            // Make sure that the Actor doesn't already have an attribute of this type
            foreach (Component attribute in attributes)
            {
                if (attribute.type == newAttribute.type)
                {
                    Debug.WriteLine("You can't add an attribute that has already been added to this object!");
                    break;
                }

            }

            // Add the attribute which we just created
            attributes.Add(newAttribute);
        }
        catch
        {
            Console.WriteLine($"We couldn't find the component {componentName}.");
        }
    }

    // Call a specific method of an attribute
    public void CallComponentMethod(string attributeType, string method, object[] parameters = null)
    {
        // Loop through all attributes
        foreach (Component attribute in attributes)
        {
            // Check if the looped attribute matches the attribute passed in the parameter. Remember, there can be only one of each type of attribute per GameObject.
            if (attribute.type == attributeType)
            {
                // Invoke the method that the user passed
                try
                {
                    attribute.GetType().GetMethod(method).Invoke(attribute, parameters);
                }
                catch
                {
                    Debug.WriteLine("We couldn't find the method " + method + " in the " + attributeType + " type.");
                }
                break;
            }
        }
    }

    // Call a specific object method of an attribute
    public object CallComponentObjectMethod(string attributeType, string method, object[] parameters = null)
    {
        // Loop through all attributes
        foreach (Component attribute in attributes)
        {
            // Check if the looped attribute matches the attribute passed in the parameter. Remember, there can be only one of each type of attribute per GameObject.
            if (attribute.type == attributeType)
            {
                // Invoke the method that the user passed
                try
                {
                    return attribute.GetType().GetMethod(method).Invoke(attribute, parameters);
                }
                catch
                {
                    Debug.WriteLine("We couldn't find the method " + method + " in the " + attributeType + " type.");
                }
                break;
            }
        }
        return null;
    }

    // Edit a variable of an attribute
    public void SetComponentVariable(string attributeType, string variable, object newVariable)
    {
        // Loop through all attributes
        foreach (Component attribute in attributes)
        {
            // Check if the looped attribute matches the attribute passed in the parameter. Remember, there can be only one of each type of attribute per GameObject.
            if (attribute.type == attributeType)
            {
                // Set the variable
                try
                {
                    attribute.GetType().GetField(variable).SetValue(attribute, newVariable);
                }
                catch
                {
                    Debug.WriteLine("We couldn't find the variable " + variable + " in the " + attributeType + " type.");
                }
                break;
            }
        }
    }

    public object GetComponentVariable(string attributeType, string variable)
    {
        foreach (Component attribute in attributes)
        {
            if (attribute.type == attributeType)
            {
                try
                {
                    return attribute.GetType().GetField(variable).GetValue(attribute);
                }
                catch
                {
                    Debug.WriteLine("We couldn't find the variable " + variable + " in the " + attributeType + " type.");
                }
            }
        }
        return null;
    }

    public void Remove()
    {
        parent.GameObjects.Remove(this);
    }
}