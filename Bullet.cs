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
    class Bullet
    {
        private Model bulletModel;
        private Matrix world = Matrix.Identity;
        private BasicEffect effect;
        private GraphicsDevice graphicsDevice;
        private Matrix[] boneTransforms;

        private Tank tank;
        private Tank targetTank;
        private Map map;
        private Vector3 position, direction, orientation, velocity, gravity;
        private BoundingSphere boundingSphere;
        private float speed, yaw;
        private bool shoot = false;

        /* Music / sound related
        private SoundEffect shootSound;
        private SoundEffectInstance shootSoundInstance;
        */

        public BoundingSphere BoundingSphere
        {
            get { return boundingSphere; }
            set { boundingSphere = value; }
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector3 Orientation
        {
            get { return orientation; }
            set { orientation = value; }
        }

        public Bullet(GraphicsDevice graphicsDevice, ContentManager contentManager, Tank tank, Tank targetTank, Map map)
        {
            /* Music / sound related
            shootSound = contentManager.Load<SoundEffect>("shootSound");
            shootSoundInstance = shootSound.CreateInstance();
            shootSoundInstance.Volume = 0.7f;
            */

            this.graphicsDevice = graphicsDevice;
            bulletModel = contentManager.Load<Model>("esfera");
            this.effect = map.Effect;
            this.tank = tank;
            this.targetTank = targetTank;
            this.map = map;

            this.orientation = tank.TankOrientation;

            boundingSphere = new BoundingSphere();
            boundingSphere.Radius = 0.8f;

            speed = 0.2f;
            gravity = new Vector3(0, -9.8f, 0);

            boneTransforms = new Matrix[bulletModel.Bones.Count];
        }

        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && shoot == false)
            {
                /* Music / sound related
                shootSoundInstance.Play();
                */

                direction = Vector3.Zero;

                Matrix rotationMatrix = Matrix.CreateFromYawPitchRoll(tank.Yaw + tank.TurretRotationValue, tank.CannonRotationValue, 0);

                direction = Vector3.Transform(new Vector3(0, 0, 1), rotationMatrix);
                direction.Normalize();

                position = tank.Position + tank.TankOrientation * 1.7f + direction;

                velocity = direction * 20;

                bulletModel.Root.Transform = Matrix.CreateScale(0.2f) * Matrix.CreateTranslation(position);
                boundingSphere.Center = position;
                shoot = true;
            }

            if (shoot)
            {
                float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
                position += velocity * time;
                velocity += gravity * time;
                boundingSphere.Center = position;
            }
            else
            {
                boundingSphere.Center = Vector3.Zero;
                position = tank.Position;
                bulletModel.Root.Transform = Matrix.CreateScale(0.2f) * Matrix.CreateTranslation(position);
            }

            if ((position.X < 1f || position.X > map.Width - 1f || position.Z < 1f || position.Z > map.Height - 1f) 
                || position.Y < map.CalcSurfaceFollow(position, 0f).Y || targetTank.Collided)
            {
                shoot = false;    
                boundingSphere.Center = Vector3.Zero;   
            }            
        }

        public void UpdateP2(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad5) && shoot == false)
            {
                /* Music / sound related
                shootSound.Play();
                */

                direction = Vector3.Zero;

                Matrix rotationMatrix = Matrix.CreateFromYawPitchRoll(tank.Yaw + tank.TurretRotationValue, tank.CannonRotationValue, 0);

                direction = Vector3.Transform(new Vector3(0, 0, 1), rotationMatrix);
                direction.Normalize();

                position = tank.Position + tank.TankOrientation * 1.7f + direction;

                velocity = direction * 20;

                bulletModel.Root.Transform = Matrix.CreateScale(0.2f) * Matrix.CreateTranslation(position);

                shoot = true;
            }

            if (shoot)
            {
                float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
                position += velocity * time;
                velocity += gravity * time;
                boundingSphere.Center = position;
            }
            else
            {
                boundingSphere.Center = Vector3.Zero;
                position = tank.Position;
                bulletModel.Root.Transform = Matrix.CreateScale(0.2f) * Matrix.CreateTranslation(position);
            }

            if ((position.X < 1f || position.X > map.Width - 1f || position.Z < 1f || position.Z > map.Height - 1f) || position.Y < map.CalcSurfaceFollow(position, 0f).Y || targetTank.Collided)
            {
                shoot = false;
                boundingSphere.Center = Vector3.Zero;
            }
        }

        public void Draw()
        {
            bulletModel.Root.Transform = world;
            bulletModel.CopyAbsoluteBoneTransformsTo(boneTransforms);

            Matrix worldMatrix = Matrix.CreateScale(0.2f) * Matrix.CreateTranslation(position);

            if (shoot)
            {
                foreach (ModelMesh mesh in bulletModel.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.View = this.effect.View;
                        effect.Projection = this.effect.Projection;
                        effect.World = boneTransforms[mesh.ParentBone.Index] * worldMatrix;
                        effect.EnableDefaultLighting();
                        effect.LightingEnabled = true;
                    }

                    mesh.Draw();
                }
            }
        }
    }
}
