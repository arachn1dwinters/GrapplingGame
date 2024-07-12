﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using TiledCS;

using GrapplingGame.GameObjectsComponentsLevels.GameObjects;
using GrapplingGame.GameObjectsComponentsLevels.Levels;
using GrapplingGame.GameObjectsComponentsLevels.Components;
using GrapplingGame.GameObjectsComponentsLevels.Helpers;
using System.Diagnostics;

namespace GrapplingGame;
public class GameManager : Game
{
    GraphicsDeviceManager _graphics;
    SpriteBatch _spriteBatch;

    // Texture2Ds
    public Texture2D playerSprite;
    public Texture2D grapplingGunSprite;
    public Texture2D targetActiveSprite;

    // Fonts
    public SpriteFont pixelmix;

    // Levels
    Level currentLevel;
    public Level CurrentLevel
    {
        get { return currentLevel; }
        set
        {
            currentLevel = value;
            currentLevel.Initialize();
        }
    }
    public List<Level> levels = new();

    // Tiles
    private TiledMap _map;
    private TiledTileset _tileset;
    private Texture2D _tilesetTexture;
    private int _tileWidth;
    private int _tileHeight;
    private int _tilesetTilesWide;
    private int _tilesetTilesHeight;
    public List<GameObject> tiles = new();

    // Singleton stuff
    public static GameManager Instance;

    public GameManager()
    {
        _graphics = new GraphicsDeviceManager(this);

        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        // Singleton stuff
        if (Instance == null) Instance = this;
    }

    protected override void Initialize()
    {
        base.Initialize();

        _graphics.PreferredBackBufferWidth = 1080;
        _graphics.PreferredBackBufferHeight = 1080;
        _graphics.ApplyChanges();

        //var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 400, 400);

        Level1 level1 = new(this, false);
        CurrentLevel = level1;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        playerSprite = this.Content.Load<Texture2D>("Player");
        grapplingGunSprite = Content.Load<Texture2D>("grappling_gun");
        targetActiveSprite = Content.Load<Texture2D>("TargetActive");

        // Add it to the desktop
        /*_desktop = new Desktop();
        _desktop.Root = grid;*/
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);

        foreach (GameObject obj in currentLevel.GameObjects.ToArray())
        {
            // Set the rect of the GameObject
            obj.rect.X = obj.position.X;
            obj.rect.Y = obj.position.Y;

            // Run the update functions of every Component of every GameObject
            if (obj.attributes != null)
            {
                foreach (Component a in obj.attributes)
                {
                    a.Update(gameTime);
                }
            }
        }
        currentLevel.Update();
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        //var transformMatrix = _camera.GetViewMatrix();
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Render Gameobjects
        foreach (GameObject obj in currentLevel.GameObjects)
        {
            if (obj.sprite != null && !obj.dontRender)
            {
                if (obj.cropped == false)
                {
                    _spriteBatch.Draw(obj.sprite, new Microsoft.Xna.Framework.Vector2(obj.position.X, obj.position.Y), null, Color.White, obj.Rotation, obj.origin, new Microsoft.Xna.Framework.Vector2(obj.sizeMultiplier.X, obj.sizeMultiplier.Y), SpriteEffects.None, 1);
                }
                else
                {
                    _spriteBatch.Draw(obj.sprite, new Rectangle(obj.rect.X, obj.rect.Y, obj.width * obj.sizeMultiplier.X, obj.height * obj.sizeMultiplier.Y), obj.cropRect, Color.White);
                }
            }

            if (obj.type == "target")
            {
                if ((bool)obj.GetAttributeVariable("TargetComponent", "Active"))
                {
                    Point startPos = (Point)CurrentLevel.GrappleGun.GetAttributeVariable("GrappleGunComponent", "TipOfGun");
                    Point endPos = new(obj.position.X + 16, obj.position.Y + 16);
                    Functions.DrawLineBetween(_spriteBatch, startPos, endPos, 3, Color.White);
                }
            }
        }

        _spriteBatch.End();

        // UI
        //_desktop.Render();

        base.Draw(gameTime);
    }

    public void CreateMap(string tiledLevel)
    {
        // Remove all current tiles
        foreach (GameObject obj in tiles)
        {
            obj.Remove();
        }

        // Tiles
        // Set the "Copy to Output Directory" property of these two files to `Copy if newer`
        // by clicking them in the solution explorer.
        string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        _map = new TiledMap(path + "/" + Content.RootDirectory + "/tilemaps/" + tiledLevel);
        _tileset = new TiledTileset(path + "/" + Content.RootDirectory + "/tilemaps/Tileset.tsx");

        _tilesetTexture = Content.Load<Texture2D>("tilemaps/tileset");

        _tileWidth = _tileset.TileWidth;
        _tileHeight = _tileset.TileHeight;

        // Amount of tiles on each row (left right)
        _tilesetTilesWide = _tileset.Columns;
        // Amount of tiles on each column (up down)
        _tilesetTilesHeight = _tileset.TileCount / _tileset.Columns;

        // Make tiles into GameObjects
        for (var i = 0; i < _map.Layers[0].data.Length; i++)
        {
            int gid = _map.Layers[0].data[i];

            // Empty tile, do nothing
            if (gid == 0)
            {

            }
            else
            {
                int tileFrame = gid - 1;

                var tile = _map.GetTiledTile(_map.Tilesets[0], _tileset, gid);

                int column = tileFrame % _tilesetTilesWide;
                int row = (int)Math.Floor((double)tileFrame / (double)_tilesetTilesWide);

                int x = (i % _map.Width) * _map.TileWidth;
                int y = (i / _map.Width) * _map.TileHeight;

                Rectangle tilesetRec = new(_tileWidth * column, _tileHeight * row, _tileWidth, _tileHeight);

                GameObject newTile = new(_tilesetTexture, tilesetRec, new Point((int)x, (int)y), new Point(1, 1), "tile", currentLevel);

                switch (gid)
                {
                    // Add special tile code here
                    case 2:
                        newTile.type = "target";
                        newTile.AddAttribute("TargetComponent");
                        newTile.SetAttributeVariable("TargetComponent", "TargetType", TARGETTYPE.swing);
                        currentLevel.targets.Add(newTile);
                        break;
                    case 3:
                        newTile.type = "ladder";
                        break;
                    case 4:
                        newTile.type = "target";
                        newTile.AddAttribute("TargetComponent");
                        newTile.SetAttributeVariable("TargetComponent", "TargetType", TARGETTYPE.pull);
                        currentLevel.targets.Add(newTile);
                        break;
                }
            }
        }
    }

    /*public void AddLevelUI(List<object> UIElements)
     {
         foreach (object UIObj in UIElements)
         {
             grid.Widgets.Add((Widget)UIObj);
         }
     }*/
}