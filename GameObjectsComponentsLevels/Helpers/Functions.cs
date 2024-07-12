using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrapplingGame.GameObjectsComponentsLevels.Helpers;

public static class Functions
{
    public static void DrawLineBetween(
        this SpriteBatch spriteBatch,
        Point startPos,
        Point endPos,
        int thickness,
        Color color)
    // Somebody else wrote this lol and i forgot who but thank you
    {
        // Create a texture as wide as the distance between two points and as high as
        // the desired thickness of the line.
        var distance = (int)Vector2.Distance(new Vector2(startPos.X, startPos.Y), new Vector2(endPos.X, endPos.Y));
        var texture = new Texture2D(spriteBatch.GraphicsDevice, distance, thickness);

        // Fill texture with given color.
        var data = new Color[distance * thickness];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = color;
        }
        texture.SetData(data);

        // Rotate about the beginning middle of the line.
        var rotation = (float)Math.Atan2(endPos.Y - startPos.Y, endPos.X - startPos.X);
        var origin = new Vector2(0, thickness / 2);

        spriteBatch.Draw(
            texture,
            new Vector2(startPos.X, startPos.Y),
            null,
            Color.White,
            rotation,
            origin,
            1.0f,
            SpriteEffects.None,
            1.0f);
    }
    public static Point NormalizePoint(Point point)
    {
        double hypotenuse = Math.Sqrt(Math.Pow(point.X, 2) + Math.Pow(point.Y, 2));
        return new Point((int)Math.Floor(point.X / hypotenuse), (int)Math.Floor(point.Y / hypotenuse));
    }
}