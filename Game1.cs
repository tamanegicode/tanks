#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace Tanks
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Map map;
        Texture2D heightMap;
        Texture2D texture;
        Camera camera;
        Tank tank;
        Vector3 startingPoint = new Vector3(40, 500, 50);
        Vector3 startingPointP2 = new Vector3(60, 500, 60);
        Tank tankP2;
        Bullet bullet;
        Bullet bulletP2;
        DustParticleManager dustManager;
        DustParticleManager dustManagerP2;

        SpriteFont font;
        /* Music / sound related
        SoundEffect mainTheme;
        SoundEffectInstance mainThemeInstance;
        */
        bool holdingV = false;

        int scoreP1 = 0, scoreP2 = 0;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("font");
            
            heightMap = Content.Load<Texture2D>("hm.jpg");
            texture = Content.Load<Texture2D>("grass.jpg");

            map = new Map(GraphicsDevice, heightMap, texture);


            tank = new Tank(GraphicsDevice, Content, map, startingPoint);
            tankP2 = new Tank(GraphicsDevice, Content, map, startingPointP2);

            camera = new Camera(GraphicsDevice, map, new Vector3(1.0f, 1.0f, 5.0f), tank, tankP2);

            bullet = new Bullet(GraphicsDevice, Content, tank, tankP2, map);
            bulletP2 = new Bullet(GraphicsDevice, Content, tankP2, tank, map);

            dustManager = new DustParticleManager(GraphicsDevice, map, tank);
            dustManagerP2 = new DustParticleManager(GraphicsDevice, map, tankP2);

            /* Music / sound related
            mainTheme = Content.Load<SoundEffect>("WONDERFUL");

            mainThemeInstance = mainTheme.CreateInstance();
            mainThemeInstance.Volume = 1f;
            mainThemeInstance.IsLooped = true;
            mainThemeInstance.Play();
            */

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            camera.Update();

            tank.Update();
            tank.Position = map.CalcSurfaceFollow(tank.Position, 0.0f);
            tank.TankOrientation = map.CalcNormalsFollow(tank.Position);


            tankP2.UpdatePlayer2(tank);
            tankP2.Position = map.CalcSurfaceFollow(tankP2.Position, 0.0f);
            tankP2.TankOrientation = map.CalcNormalsFollow(tankP2.Position);


            tank.CheckCollision(bulletP2.BoundingSphere);
            tankP2.CheckCollision(bullet.BoundingSphere);


            tank.CheckNextCollision(tankP2.BoundingSphere);
            tankP2.CheckNextCollision(tank.BoundingSphere);


            bullet.Update(gameTime);
            bulletP2.UpdateP2(gameTime);


            dustManager.Update();
            dustManagerP2.Update();

            if (tank.Collided)
                scoreP2++;

            if (tankP2.Collided)
                scoreP1++;

            if(Keyboard.GetState().IsKeyDown(Keys.Back))
            {
                tank.Position = startingPoint;
                tankP2.Position = startingPointP2;
                scoreP1 = 0;
                scoreP2 = 0;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.V))
                holdingV = true;
            else if (Keyboard.GetState().IsKeyUp(Keys.V))
                holdingV = false;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            map.Draw();
            tank.Draw();
            tankP2.Draw();
            bullet.Draw();
            bulletP2.Draw();
            dustManager.Draw();
            dustManagerP2.Draw();

            spriteBatch.Begin();

            if (!holdingV)
            {
                spriteBatch.DrawString(font, "Score P1: " + scoreP1 + "                                                 Camera: " + camera.SwitchCase, new Vector2(10, 10), Color.Black);
                spriteBatch.DrawString(font, "Score P2: " + scoreP2, new Vector2(10, 30), Color.Black);

                spriteBatch.DrawString(font, "Hold V to check controls", new Vector2(10, 450), Color.Black);
            }
            else
            {
                spriteBatch.DrawString(font, "Player 1", new Vector2(50, 20), Color.Black);
                spriteBatch.DrawString(font, "        Move: WASD", new Vector2(50, 40), Color.Black);
                spriteBatch.DrawString(font, "        Turret and cannon: IJKL", new Vector2(50, 60), Color.Black);
                spriteBatch.DrawString(font, "        Shoot: Space", new Vector2(50, 80), Color.Black);
                spriteBatch.DrawString(font, "Player 2", new Vector2(50, 100), Color.Black);
                spriteBatch.DrawString(font, "        Move: Numpad 8, 4, 6, 2", new Vector2(50, 120), Color.Black);
                spriteBatch.DrawString(font, "        Turret and cannon: Numpad 7, 9, 1, 3", new Vector2(50, 140), Color.Black);
                spriteBatch.DrawString(font, "        Shoot: Numpad 5", new Vector2(50, 160), Color.Black);
                spriteBatch.DrawString(font, "        Toggle control: Enter", new Vector2(50, 180), Color.Black);
                spriteBatch.DrawString(font, "Change camera: 1, 2, 3, 4", new Vector2(50, 200), Color.Black);
                spriteBatch.DrawString(font, "Move camera: Arrow keys (Camera 1 and 2 only)", new Vector2(50, 220), Color.Black);
                spriteBatch.DrawString(font, "Zoom in/out: Mouse scroll (Camera 3 and 4 only)", new Vector2(50, 240), Color.Black);
                spriteBatch.DrawString(font, "Reset scores and positions: Backspace", new Vector2(50, 260), Color.Black);
            }

            spriteBatch.End();
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            base.Draw(gameTime);
        }
    }
}
