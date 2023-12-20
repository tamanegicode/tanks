#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace Tanks
{
    class DustParticleManager
    {
        private DustParticle[] dustArray;
        private Random random;

        private Tank tank;
        private Map map;

        private Vector3 tankButtPosition;

        private int randomYaw;
        private int randomPitch;
        private float randomSpeed;
        private float randomPitchSpeed;

        public DustParticleManager(GraphicsDevice graphicsDevice, Map map, Tank tank)
        {
            dustArray = new DustParticle[500];
            random = new Random();

            this.tank = tank;
            this.map = map;

            FindTankButt();

            for (int i = 0; i < dustArray.Length; i++)
            {
                dustArray[i] = new DustParticle(graphicsDevice, map.Effect);
                MakeItRain(i);
            }
        }

        public void FindTankButt()
        {
            tankButtPosition = tank.Position + tank.NewDirection * -1 + tank.TankOrientation;
            tankButtPosition.Y = tank.Position.Y;

            if ((tankButtPosition.X > 2f && tankButtPosition.X < map.Width - 2f && tankButtPosition.Z > 2f && tankButtPosition.Z < map.Height - 2f))
            {
                tankButtPosition = map.CalcSurfaceFollow(tankButtPosition, 0);
            }
            else
            {
                tankButtPosition = tank.Position;
            }
        }

        public void MakeItRain(int i)
        {
            randomYaw = random.Next((int)MathHelper.ToRadians(-90), (int)MathHelper.ToRadians(90));
            randomPitch = random.Next(0, (int)MathHelper.ToRadians(-10));
            randomSpeed = random.Next(10, 30) * 0.005f;
            randomPitchSpeed = random.Next(1, 5) * 0.05f;

            dustArray[i].Position = tankButtPosition + Vector3.Transform(Vector3.Right, Matrix.CreateRotationY(tank.Yaw)) * random.Next(-5, 5) * 0.2f;
            dustArray[i].Speed = randomSpeed;
            dustArray[i].Yaw = tank.Yaw + randomYaw;
            dustArray[i].Pitch = randomPitch;
            dustArray[i].PitchSpeed = randomPitchSpeed;
        }

        public void Update()
        {
            FindTankButt();

            for (int i = 0; i < dustArray.Length; i++)
            {
                if ((dustArray[i].Position.X > 2f && dustArray[i].Position.X < map.Width - 2f && dustArray[i].Position.Z > 2f 
                    && dustArray[i].Position.Z < map.Height - 2f))
                {
                    if (dustArray[i].Position.Y > map.CalcSurfaceFollow(dustArray[i].Position, 0).Y - 0.5f)
                        dustArray[i].Update();

                    if (dustArray[i].Position.Y <= map.CalcSurfaceFollow(dustArray[i].Position, 0).Y - 0.5f && tank.Moving)
                    {
                        MakeItRain(i);
                    }
                }
                else
                {
                    if (dustArray[i].Position.Y > 0)
                        dustArray[i].Update();

                    if (dustArray[i].Position.Y <= 0 && tank.Moving)
                    {
                        MakeItRain(i);
                    }
                }

            }
        }

        public void Draw()
        {
            for (int i = 0; i < dustArray.Length; i++)
            {
                if ((dustArray[i].Position.X > 2f && dustArray[i].Position.X < map.Width - 2f && dustArray[i].Position.Z > 2f 
                    && dustArray[i].Position.Z < map.Height - 2f))
                {
                    if (dustArray[i].Position.Y > map.CalcSurfaceFollow(dustArray[i].Position, 0).Y - 0.5f)
                        dustArray[i].Draw();
                }
                else
                {
                    if (dustArray[i].Position.Y > 0)
                        dustArray[i].Draw();
                }
            }
        }
    }
}
