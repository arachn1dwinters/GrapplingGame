using Microsoft.Xna.Framework;
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

namespace GrapplingGame;
public class GameManager : Game
{
    GraphicsDeviceManager _graphics;
    SpriteBatch _spriteBatch;

    // Texture2Ds
    public Texture2D playerSprite;
         
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
    public List<GameObject> tiles;


    public GameManager()
    {
        _graphics = new GraphicsDeviceManager(this);

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();

        _graphics.PreferredBackBufferWidth = 600;
        _graphics.PreferredBackBufferHeight = 600;
        _graphics.ApplyChanges();

        //var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 400, 400);

        Level1 level1 = new(this, false);
        CurrentLevel = level1;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        playerSprite = this.Content.Load<Texture2D>("Player");

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
            obj.rect.X = obj.position.x;
            obj.rect.Y = obj.position.y;

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
                    _spriteBatch.Draw(obj.sprite, obj.rect, Color.White);
                } else
                {
                    {
                        _spriteBatch.Draw(obj.sprite, new Rectangle(obj.rect.X, obj.rect.Y, obj.width * obj.sizeMultiplier.x, obj.height * obj.sizeMultiplier.y), obj.cropRect, Color.White);
                    }
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
        Console.WriteLine("Creating map " + tiledLevel + "!");

        // Remove all current tiles
        foreach (GameObject obj in tiles)
        {
            obj.Remove();
        }

        // Tiles
        // Set the "Copy to Output Directory" property of these two files to `Copy if newer`
        // by clicking them in the solution explorer.
        string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        _map = new TiledMap(path + "/" + Content.RootDirectory + "/" + tiledLevel);
        _tileset = new TiledTileset(path + "/" + Content.RootDirectory + "/Tiletest.tsx");

        // Not the best way to do this but it works. It looks for "exampleTileset.xnb" file
        // which is the result of building the image file with "Content.mgcb".
        _tilesetTexture = Content.Load<Texture2D>("tiletest");

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

                float x = (i % _map.Width) * _map.TileWidth;
                float y = (float)Math.Floor(i / (double)_map.Width) * _map.TileHeight;

                Rectangle tilesetRec = new(_tileWidth * column, _tileHeight * row, _tileWidth, _tileHeight);

                GameObject newTile = new(_tilesetTexture, tilesetRec, new Vector2Int((int)x, (int)y), new Vector2Int(1, 1), "tile", currentLevel);

                /*switch (gid)
                {
                    // Add special tile code here
                }*/
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