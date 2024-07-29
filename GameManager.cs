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

using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System.IO.Pipes;

namespace GrapplingGame;
public class GameManager : Game
{
    GraphicsDeviceManager _graphics;
    SpriteBatch _spriteBatch;

    // Content
    public Texture2D playerSprite;
    public Texture2D grapplingGunSprite;
    public Texture2D targetActiveSprite;
    public Texture2D cursorImage;
    public SpriteFont pixelify;

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
    private Texture2D _specialTilesetTexture;
    private int _tileWidth;
    private int _tileHeight;
    private int _tilesetTilesWide;
    private int _tilesetTilesHeight;
    public List<GameObject> tiles = new();

    // Singleton stuff
    public static GameManager Instance;
    public Camera Camera;

    // Camera
    public OrthographicCamera OrthographicCamera;
    public float HalfScreenWidth;
    public float HalfScreenHeight;

    public GameManager()
    {
        _graphics = new GraphicsDeviceManager(this);

        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        // Singleton stuff
        Instance ??= this;
        Camera = new();
    }

    protected override void Initialize()
    {
        base.Initialize();

        _graphics.PreferredBackBufferWidth = 1600;
        _graphics.PreferredBackBufferHeight = 900;
        _graphics.ApplyChanges();

        var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 1600, 900);
        OrthographicCamera = new OrthographicCamera(viewportAdapter);
        HalfScreenWidth = 800;
        HalfScreenHeight = 450;

        Level1 level1 = new(this, false);
        CurrentLevel = level1;

        //IsMouseVisible = false;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        playerSprite = this.Content.Load<Texture2D>("Player");
        grapplingGunSprite = Content.Load<Texture2D>("grappling_gun");
        targetActiveSprite = Content.Load<Texture2D>("TargetActive");
        cursorImage = Content.Load<Texture2D>("Cursor");
        pixelify = Content.Load<SpriteFont>("Pixelify");

        // Add it to the desktop
        /*_desktop = new Desktop();
        _desktop.Root = grid;*/
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);

        Camera.Instance.Update(gameTime);

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

        var transformMatrix = OrthographicCamera.GetViewMatrix();
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix : transformMatrix);

        _spriteBatch.Draw(cursorImage, new Rectangle(currentLevel.MousePosition, new Point(32, 32)), Color.White);

        // Render Gameobjects
        foreach (GameObject obj in currentLevel.GameObjects)
        {
            if (obj.Visible)
            {
                if (obj.sprite != null && !obj.dontRender)
                {
                    if (obj.cropped == false)
                    {
                        _spriteBatch.Draw(obj.sprite, new Vector2(obj.position.X, obj.position.Y), null, Color.White, obj.Rotation, obj.origin, new Vector2(obj.sizeMultiplier.X, obj.sizeMultiplier.Y), SpriteEffects.None, 1);
                    }
                    else
                    {
                        _spriteBatch.Draw(obj.sprite, new Rectangle(obj.rect.X, obj.rect.Y, obj.width * obj.sizeMultiplier.X, obj.height * obj.sizeMultiplier.Y), obj.cropRect, Color.White);
                    }
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

            if (currentLevel.Player != null)
            {
                Point currentPlayerVelocity = (Point)currentLevel.Player.GetAttributeVariable("MovementComponent", "Velocity");
                _spriteBatch.DrawString(pixelify, $"{currentLevel.CanConnect}", OrthographicCamera.ScreenToWorld(new Vector2(0, 0)), Color.White);
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

        string path = "";
        FileStream specialFileStream;
        FileStream fileStream;
        try
        {
            path = Path.GetDirectoryName(AppContext.BaseDirectory);

            _map = new TiledMap(path + "\\" + Content.RootDirectory + "\\tilemaps\\" + tiledLevel);
            _tileset = new TiledTileset(path + "\\" + Content.RootDirectory + "\\tilemaps\\Tileset.tsx");

            specialFileStream = new FileStream(path + "\\" + Content.RootDirectory + "\\tilemaps\\special_tileset.png", FileMode.Open);
            fileStream = new FileStream(path + "\\" + Content.RootDirectory + "\\tilemaps\\tileset.png", FileMode.Open);
        } catch
        {
            path = Path.GetDirectoryName(AppContext.BaseDirectory).Replace("\\win-x64", "");
            _map = new TiledMap(path + "\\" + Content.RootDirectory + "\\tilemaps\\" + tiledLevel);
            _tileset = new TiledTileset(path + "\\" + Content.RootDirectory + "\\tilemaps\\Tileset.tsx");

            specialFileStream = new FileStream(path + "\\" + Content.RootDirectory + "\\tilemaps\\special_tileset.png", FileMode.Open);
            fileStream = new FileStream(path + "\\" + Content.RootDirectory + "\\tilemaps\\tileset.png", FileMode.Open);
        }
        Texture2D specialTilesetTexture = Texture2D.FromStream(GraphicsDevice, specialFileStream);
        specialFileStream.Dispose();

        Texture2D tilesetTexture = Texture2D.FromStream(GraphicsDevice, fileStream);
        fileStream.Dispose();

        _tileWidth = _tileset.TileWidth;
        _tileHeight = _tileset.TileHeight;

        _tilesetTilesWide = _tileset.Columns;
        _tilesetTilesHeight = _tileset.TileCount / _tileset.Columns;

        for (var i = 0; i < _map.Layers[1].data.Length; i++)
        {
            int gid = _map.Layers[1].data[i];

            if (gid != 0)
            {
                int tileFrame = gid - 1;

                var tile = _map.GetTiledTile(_map.Tilesets[0], _tileset, gid);

                int column = tileFrame % _tilesetTilesWide;
                int row = (int)Math.Floor((double)tileFrame / (double)_tilesetTilesWide);

                int x = (i % _map.Width) * _map.TileWidth;
                int y = (i / _map.Width) * _map.TileHeight;

                Rectangle tilesetRec = new(_tileWidth * column, _tileHeight * row, _tileWidth, _tileHeight);

                GameObject newTile = new(tilesetTexture, tilesetRec, new Point((int)x, (int)y), new Point(1, 1), "tile", currentLevel);
            }
        }

        // Now, we loop through the special tiles layer.
        // For some reason, the gids in this tileset start at 1600
        for (var i = 0; i < _map.Layers[2].data.Length; i++)
        {
            int gid = _map.Layers[2].data[i];

            if (gid != 0)
            {
                int tileFrame = gid - 1601;

                var tile = _map.GetTiledTile(_map.Tilesets[1], _tileset, gid - 1600);

                int column = tileFrame % _tilesetTilesWide;
                int row = (int)Math.Floor((double)tileFrame / (double)_tilesetTilesWide);

                int x = (i % _map.Width) * _map.TileWidth;
                int y = (i / _map.Width) * _map.TileHeight;

                Rectangle tilesetRec = new(_tileWidth * column, _tileHeight * row, _tileWidth, _tileHeight);

                GameObject newTile = new(specialTilesetTexture, tilesetRec, new Point((int)x, (int)y), new Point(1, 1), "tile", currentLevel);

                switch (gid - 1600)
                {
                    // Add special tile code here
                    case 1:
                        newTile.Remove();
                        currentLevel.PlayerSpawn = new Point(x, y);
                        break;
                    case 2:
                        newTile.type = "target";
                        newTile.AddAttribute("TargetComponent");
                        newTile.SetAttributeVariable("TargetComponent", "TargetType", TARGETTYPE.swing);

                        currentLevel.targets.Add(newTile);
                        break;
                    case 3:
                        newTile.type = "target";
                        newTile.AddAttribute("TargetComponent");
                        newTile.SetAttributeVariable("TargetComponent", "TargetType", TARGETTYPE.pull);
                        currentLevel.targets.Add(newTile);
                        break;
                    case 4:
                        newTile.Remove();
                        currentLevel.levelOrigin = new Point(x, y);
                        break;
                }
            }
        }
    }
}