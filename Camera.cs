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
    class Camera
    {
        #region Variables

        private Matrix view;
        private Matrix projection;
        private Vector3 position = new Vector3(32.0f, 0.0f, 23.0f);
        private Vector3 previousPosition;
        private Vector3 distanceFromTank;
        private float distanceFromTankZ = -10f;
        private float previousScrollValue;
        private Vector3 direction;
        private Vector3 target;
        private Tank targetTank;
        private Tank targetTankP2;
        private Map map;
        private BasicEffect effect;
        private GraphicsDevice graphicsDevice;

        private MouseState currentMouseState;
        private MouseState firstMouseState;
        private float verticalMouseRotation = -0.3f;
        private float horizontalMouseRotation = 9.8f;

        private float speed = 0.1f;

        private int width, height;

        private int switchCase = 3;

        #endregion

        #region Properties

        public int SwitchCase
        {
            get { return switchCase; }
            set { switchCase = value; }
        }

        public Matrix WorldMatrix
        {
            get { return view; }
            set { view = value; }
        }     

        public Matrix Projection
        {
            get { return projection; }
            set { projection = value; }
        }        

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector3 Target
        {
            get { return target; }
            set { target = value; }
        }

        public BasicEffect Effect
        {
            get { return effect; }
            set { effect = value; }
        }

        #endregion

        public Camera(GraphicsDevice graphicsDevice, Map map, Vector3 targetPosition, Tank targetTank, Tank targetTankP2)
        {
            this.graphicsDevice = graphicsDevice;
            this.effect = map.Effect;
            this.target = targetPosition;
            this.targetTank = targetTank;
            this.targetTankP2 = targetTankP2;
            this.map = map;

            distanceFromTank = new Vector3(0, 5, distanceFromTankZ);

            width = map.Width;
            height = map.Height;

            Mouse.SetPosition(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);
            firstMouseState = Mouse.GetState();

            previousPosition = position;
        }

        public void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.D1))
            {
                position = targetTank.Position;
                position.Z += -10f;
                switchCase = 1;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D2))
            {                
                position = targetTank.Position;
                position.Z += -10f;
                switchCase = 2;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D3))
            {
                distanceFromTankZ = -10f;
                switchCase = 3;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D4))
            {
                distanceFromTankZ = -10f;
                switchCase = 4;
            }

            switch (switchCase)
            {
                case 1: FreeCamera();
                    break;
                case 2: FreeCameraWithSurfaceFollow();
                    break;
                case 3: TankFollow();
                    break;
                case 4: TankFollowP2();
                    break;
                default: TankFollow();
                    break;
            } 

            previousPosition = position;
        }

        private void FreeCamera()
        {
            direction = Vector3.Zero;

            currentMouseState = Mouse.GetState();

            if (currentMouseState != firstMouseState)
            {
                float x = currentMouseState.X - firstMouseState.X;
                float y = currentMouseState.Y - firstMouseState.Y;

                horizontalMouseRotation -= x * speed * 0.1f;
                verticalMouseRotation -= y * speed * 0.1f;

                if (verticalMouseRotation > MathHelper.ToRadians(45))
                {
                    verticalMouseRotation = MathHelper.ToRadians(45);
                }

                else if (verticalMouseRotation < MathHelper.ToRadians(-45))
                {
                    verticalMouseRotation = MathHelper.ToRadians(-45);
                }

                Mouse.SetPosition(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                direction += new Vector3(0.0f, 0.0f, -1.0f);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                direction += new Vector3(0.0f, 0.0f, 1.0f);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                direction += new Vector3(-1.0f, 0.0f, 0.0f);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                direction += new Vector3(1.0f, 0.0f, 0.0f);
            }

            ApplyView();
        }

        private void FreeCameraWithSurfaceFollow()
        {
            direction = Vector3.Zero;

            currentMouseState = Mouse.GetState();

            if (currentMouseState != firstMouseState)
            {
                float x = currentMouseState.X - firstMouseState.X;
                float y = currentMouseState.Y - firstMouseState.Y;

                horizontalMouseRotation -= x * speed * 0.1f;
                verticalMouseRotation -= y * speed * 0.1f;

                if (verticalMouseRotation > MathHelper.ToRadians(45))
                {
                    verticalMouseRotation = MathHelper.ToRadians(45);
                }

                else if (verticalMouseRotation < MathHelper.ToRadians(-45))
                {
                    verticalMouseRotation = MathHelper.ToRadians(-45);
                }

                Mouse.SetPosition(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                direction += new Vector3(0.0f, 0.0f, -1.0f);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                direction += new Vector3(0.0f, 0.0f, 1.0f);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                direction += new Vector3(-1.0f, 0.0f, 0.0f);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                direction += new Vector3(1.0f, 0.0f, 0.0f);
            }

            ApplyViewSurfaceFollow();
        }

        public void TankFollow()
        {
            currentMouseState = Mouse.GetState();

            if (currentMouseState != firstMouseState)
            {
                float x = currentMouseState.X - firstMouseState.X;
                float y = currentMouseState.Y - firstMouseState.Y;

                horizontalMouseRotation -= x * speed * 0.1f;
                verticalMouseRotation -= y * speed * 0.1f;

                if (verticalMouseRotation > MathHelper.ToRadians(30))
                {
                    verticalMouseRotation = MathHelper.ToRadians(30);
                }

                else if (verticalMouseRotation < MathHelper.ToRadians(-10))
                {
                    verticalMouseRotation = MathHelper.ToRadians(-10);
                }

                Mouse.SetPosition(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);
            }

            if(currentMouseState.ScrollWheelValue > previousScrollValue)
            {
                if(distanceFromTankZ < -4)
                    distanceFromTankZ += 2f;
            }
            else if(currentMouseState.ScrollWheelValue < previousScrollValue)
            {
                if (distanceFromTankZ > -80)
                    distanceFromTankZ -= 2f;
            }

            distanceFromTank = new Vector3(0, 5, distanceFromTankZ);

            Matrix cameraRotation = Matrix.CreateFromYawPitchRoll(horizontalMouseRotation, verticalMouseRotation, 0);

            Vector3 rotationVector = Vector3.Transform(targetTank.Direction + distanceFromTank, cameraRotation);

            position = rotationVector + targetTank.Position;

            try
            {
                if (position.Y <= map.CalcSurfaceFollow(position, 0).Y)
                    position.Y = map.CalcSurfaceFollow(position, 0).Y + 5f;
            }
            catch
            { }

            target = targetTank.Position;

            effect.View = Matrix.CreateLookAt(position, target, Vector3.Up);

            previousPosition = position;
            previousScrollValue = currentMouseState.ScrollWheelValue;
        }

        public void TankFollowP2()
        {
            currentMouseState = Mouse.GetState();

            if (currentMouseState != firstMouseState)
            {
                float x = currentMouseState.X - firstMouseState.X;
                float y = currentMouseState.Y - firstMouseState.Y;

                horizontalMouseRotation -= x * speed * 0.1f;
                verticalMouseRotation -= y * speed * 0.1f;

                if (verticalMouseRotation > MathHelper.ToRadians(30))
                {
                    verticalMouseRotation = MathHelper.ToRadians(30);
                }

                else if (verticalMouseRotation < MathHelper.ToRadians(-20))
                {
                    verticalMouseRotation = MathHelper.ToRadians(-20);
                }

                Mouse.SetPosition(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);
            }

            if (currentMouseState.ScrollWheelValue > previousScrollValue)
            {
                if (distanceFromTankZ < -4)
                    distanceFromTankZ += 2f;
            }
            else if (currentMouseState.ScrollWheelValue < previousScrollValue)
            {
                if (distanceFromTankZ > -80)
                    distanceFromTankZ -= 2f;
            }

            distanceFromTank = new Vector3(0, 5, distanceFromTankZ);

            Matrix cameraRotation = Matrix.CreateFromYawPitchRoll(horizontalMouseRotation, verticalMouseRotation, 0);

            Vector3 rotationVector = Vector3.Transform(targetTankP2.Direction + distanceFromTank, cameraRotation);

            position = rotationVector + targetTankP2.Position;

            try
            {
                if (position.Y <= map.CalcSurfaceFollow(position, 0).Y)
                    position.Y = map.CalcSurfaceFollow(position, 0).Y + 5f;
            }
            catch
            { }

            target = targetTankP2.Position;

            effect.View = Matrix.CreateLookAt(position, target, Vector3.Up);

            previousPosition = position;
            previousScrollValue = currentMouseState.ScrollWheelValue;
        }

        private void ApplyView()
        {
            Matrix cameraRotation = Matrix.CreateFromYawPitchRoll(horizontalMouseRotation, verticalMouseRotation, 0);

            Vector3 rotationVector = Vector3.Transform(direction, cameraRotation);

            position += rotationVector * speed * 4;

            target = Vector3.Transform(new Vector3(0.0f, 0.0f, -1.0f), cameraRotation) + position;

            effect.View = Matrix.CreateLookAt(position, target, Vector3.Up);
        }

        private void ApplyViewSurfaceFollow()
        {
            Matrix cameraRotation = Matrix.CreateFromYawPitchRoll(horizontalMouseRotation, verticalMouseRotation, 0);

            Vector3 rotationVector = Vector3.Transform(direction, cameraRotation);

            position += rotationVector * speed * 2.5f;
            position = map.CalcSurfaceFollow(position, 0.5f);

            if (position.X < 2f || position.X > width - 2f || position.Z < 2f || position.Z > height - 2f)
            {
                position = previousPosition;
            }

            target = Vector3.Transform(new Vector3(0.0f, 0.0f, -1.0f), cameraRotation) + position;

            effect.View = Matrix.CreateLookAt(position, target, Vector3.Up);
        }
    }
}