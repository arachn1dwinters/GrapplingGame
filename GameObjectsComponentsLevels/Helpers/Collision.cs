using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using GrapplingGame.GameObjectsComponentsLevels.Components;
using GrapplingGame.GameObjectsComponentsLevels.GameObjects;

namespace GrapplingGame.GameObjectsComponentsLevels.Helpers;
public class Collision
{
    public static List<object> CheckXCollision(int amount, GameObject parent)
    {
        bool xColliding = false;
        int xCollisionRemainder = 0;
        string colliderType = "";

        // Add that step as long as it isn't colliding with anything
        if (Math.Sign(amount) <= 0)
        {
            // Check leftmost edge
            for (int e = 0; e < parent.height; e++)
            {
                Point point = new(parent.rect.X + amount, parent.rect.Y + e);

                // Loop through every object in the scene
                foreach (GameObject obj in parent.parent.GameObjects)
                {
                    if (obj.collidable)
                    {
                        Rectangle newObjRect = new(obj.rect.X + obj.topLeftCorner.X, obj.rect.Y + obj.topLeftCorner.Y, obj.width, obj.height);

                        // Check if we are colliding with that object
                        if (obj.cropped)
                        {
                            if (newObjRect.Contains(point) && obj != parent)
                            {
                                xColliding = true;
                                foreach (Component attribute in obj.attributes)
                                {
                                    attribute.OnCollision();
                                }
                                if (parent.rect.X != newObjRect.X + obj.width)
                                {
                                    xCollisionRemainder = -(parent.rect.X - (newObjRect.X + obj.width));
                                }
                                else
                                {
                                    xCollisionRemainder = 0;
                                }
                                colliderType = obj.type;
                                break;
                            }
                        }
                        else
                        {
                            if (newObjRect.Contains(point) && obj != parent)
                            {
                                xColliding = true;
                                foreach (Component attribute in obj.attributes)
                                {
                                    attribute.OnCollision();
                                }
                                if (parent.rect.X != newObjRect.X + obj.width)
                                {
                                    xCollisionRemainder = -(parent.rect.X - (newObjRect.X + obj.width));
                                }
                                else
                                {
                                    xCollisionRemainder = 0;
                                }
                                colliderType = obj.type;
                                break;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            // Check rightmost edge
            for (int e = 0; e < parent.height; e++)
            {
                Point point = new(parent.rect.X + parent.width + amount, parent.rect.Y + e);

                // Loop through every object in the scene
                foreach (GameObject obj in parent.parent.GameObjects)
                {
                    if (obj.collidable)
                    {
                        Rectangle newObjRect = new(obj.rect.X + obj.topLeftCorner.X, obj.rect.Y + obj.topLeftCorner.Y, obj.width, obj.height);

                        // Check if we are colliding with that object
                        if (obj.cropped)
                        {
                            if (newObjRect.Contains(point) && obj != parent)
                            {
                                xColliding = true;
                                foreach (Component attribute in obj.attributes)
                                {
                                    attribute.OnCollision();
                                }
                                if (parent.rect.X + parent.width != newObjRect.X)
                                {
                                    xCollisionRemainder = newObjRect.X - (parent.rect.X + parent.width);
                                }
                                else
                                {
                                    xCollisionRemainder = 0;
                                }
                                colliderType = obj.type;
                                break;
                            }
                        }
                        else
                        {
                            if (newObjRect.Contains(point) && obj != parent)
                            {
                                xColliding = true;
                                foreach (Component attribute in obj.attributes)
                                {
                                    attribute.OnCollision();
                                }
                                if (parent.rect.X + parent.width != newObjRect.X)
                                {
                                    xCollisionRemainder = newObjRect.X - (parent.rect.X + parent.width);
                                }
                                else
                                {
                                    xCollisionRemainder = 0;
                                }
                                colliderType = obj.type;
                                break;
                            }
                        }
                    }
                }
            }
        }

        return new()
        {
            xColliding,
            xCollisionRemainder,
            colliderType
        };
    }

    public static List<object> CheckYCollision(int amount, GameObject parent)
    {
        bool yColliding = false;
        int yCollisionRemainder = 0;
        string colliderType = "";

        if (Math.Sign(amount) < 0)
        {
            // Check upper edge
            for (int e = 0; e < parent.width; e++)
            {
                Point point = new(parent.rect.X + e, parent.rect.Y + amount);

                // Loop through every object in the scene
                foreach (GameObject obj in parent.parent.GameObjects)
                {
                    if (obj.collidable)
                    {
                        Rectangle newObjRect = new(obj.rect.X + obj.topLeftCorner.X, obj.rect.Y + obj.topLeftCorner.Y, obj.width, obj.height);
                        // Check if we are colliding with that object
                        if (obj.cropped)
                        {
                            if (newObjRect.Contains(point) && obj != parent)
                            {
                                yColliding = true;
                                foreach (Component attribute in obj.attributes)
                                {
                                    attribute.OnCollision();
                                }
                                if (parent.rect.Y != newObjRect.Y + obj.height)
                                {
                                    yCollisionRemainder = -(parent.rect.Y - (newObjRect.Y + obj.height));
                                }
                                else
                                {
                                    yCollisionRemainder = 0;
                                }
                                colliderType = obj.type;
                                break;
                            }
                        }
                        else
                        {
                            if (newObjRect.Contains(point) && obj != parent)
                            {
                                yColliding = true;
                                foreach (Component attribute in obj.attributes)
                                {
                                    attribute.OnCollision();
                                }
                                if (parent.rect.Y != newObjRect.Y + obj.height)
                                {
                                    yCollisionRemainder = -(parent.rect.Y - (newObjRect.Y + obj.height));
                                }
                                else
                                {
                                    yCollisionRemainder = 0;
                                }
                                colliderType = obj.type;
                                break;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            // Check lower edge
            for (int e = 0; e < parent.width; e++)
            {
                Point point = new(parent.rect.X + e, parent.rect.Y + parent.height + amount);

                // Loop through every object in the scene
                foreach (GameObject obj in parent.parent.GameObjects.ToArray())
                {
                    if (obj.collidable)
                    {
                        Rectangle newObjRect = new(obj.rect.X + obj.topLeftCorner.X, obj.rect.Y + obj.topLeftCorner.Y, obj.width, obj.height);

                        // Check if we are colliding with that object
                        if (obj.cropped)
                        {
                            if (newObjRect.Contains(point) && obj != parent)
                            {
                                yColliding = true;
                                foreach (Component attribute in obj.attributes)
                                {
                                    attribute.OnCollision();
                                }
                                if (parent.rect.Y + parent.height != newObjRect.Y)
                                {
                                    yCollisionRemainder = newObjRect.Y - (parent.rect.Y + parent.height);
                                }
                                else
                                {
                                    yCollisionRemainder = 0;
                                }
                                colliderType = obj.type;
                            }
                        }
                        else
                        {
                            if (newObjRect.Contains(point) && obj != parent)
                            {
                                yColliding = true;
                                foreach (Component attribute in obj.attributes)
                                {
                                    attribute.OnCollision();
                                }
                                if (parent.rect.Y + parent.height != newObjRect.Y)
                                {
                                    yCollisionRemainder = newObjRect.Y - (parent.rect.Y + parent.height);
                                }
                                else
                                {
                                    yCollisionRemainder = 0;
                                }
                                colliderType = obj.type;
                            }
                        }
                    }
                }
            }
        }
        
        return new()
        {
            yColliding,
            yCollisionRemainder,
            colliderType
        };
    }
}