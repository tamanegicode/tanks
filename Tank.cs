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
    class Tank
    {
        #region Variables
        private Model tankModel;

        private Matrix world = Matrix.Identity;
        private BasicEffect effect;

        private GraphicsDevice graphicsDevice;


        ModelBone leftBackWheelBone;
        ModelBone rightBackWheelBone;

        ModelBone leftFrontWheelBone;
        ModelBone rightFrontWheelBone;

        ModelBone leftSteerBone;
        ModelBone rightSteerBone;

        ModelBone turretBone;
        ModelBone cannonBone;
        ModelBone hatchBone;


        Matrix leftBackWheelTransform;
        Matrix rightBackWheelTransform;

        Matrix leftFrontWheelTransform;
        Matrix rightFrontWheelTransform;

        Matrix leftSteerTransform;
        Matrix rightSteerTransform;

        Matrix turretTransform;
        Matrix cannonTransform;
        Matrix hatchTransform;


        Matrix[] boneTransforms;


        float wheelRotationValue;
        float steerRotationValue;
        float turretRotationValue;
        float cannonRotationValue;
        float hatchRotationValue;


        private BoundingSphere boundingSphere;
        private Vector3 position;
        private Vector3 previousPosition;
        private Vector3 direction;
        private Vector3 newDirection;
        private Vector3 tankOrientation;
        private Vector3 cannonOrientation;
        private Vector3 normalNova;
        private float yaw = 0.0f;

        private bool lastLeft, lastRight, moveLeft, moveRight;

        private int width, height;
        private bool collided;
        private bool nextCollided;
        private bool moving = false;
        private BoundingSphere nextBoundingSphere;
        private bool player2Control = true;
        private bool canChangePlayer2Control = true;
        private bool turningAround = false;

        /*Music / sound related
        private SoundEffect hitSound;
        */
        #endregion

        #region Properties

        public bool Collided
        {
            get { return collided; }
            set { collided = value; }
        }

        public bool Moving
        {
            get { return moving; }
            set { moving = value; }
        }

        public Vector3 NewDirection
        {
            get { return newDirection; }
            set { newDirection = value; }
        }

        public Vector3 NormalNova
        {
            get { return normalNova; }
            set { normalNova = value; }
        }

        public float Yaw
        {
            get { return yaw; }
            set { yaw = value; }
        }

        public BoundingSphere BoundingSphere
        {
            get { return boundingSphere; }
            set { boundingSphere = value; }
        }

        public Vector3 Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        public Vector3 CannonOrientation
        {
            get { return cannonOrientation; }
            set { cannonOrientation = value; }
        }

        public Vector3 TankOrientation
        {
            get { return tankOrientation; }
            set { tankOrientation = value; }
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public float WheelRotationValue
        {
            get { return wheelRotationValue; }
            set { wheelRotationValue = value; }
        }        
        public float SteerRotationValue
        {
            get { return steerRotationValue; }
            set { steerRotationValue = value; }
        }        
        public float TurretRotationValue
        {
            get { return turretRotationValue; }
            set { turretRotationValue = value; }
        }
        public float CannonRotationValue
        {
            get { return cannonRotationValue; }
            set { cannonRotationValue = value; }
        }
        public float HatchRotationValue
        {
            get { return hatchRotationValue; }
            set { hatchRotationValue = value; }
        }
        #endregion

        public Tank(GraphicsDevice graphicsDevice, ContentManager contentManager, Map mapa, Vector3 startingPosition)
        {
            this.graphicsDevice = graphicsDevice;
            this.effect = mapa.Effect;
            position = startingPosition;

            width = mapa.Width;
            height = mapa.Height;

            boundingSphere = new BoundingSphere();
            boundingSphere.Center = position;
            boundingSphere.Radius = 1.5f;

            tankModel = contentManager.Load<Model>(@"Tank\tank");

            /* Music / sound related
            hitSound = contentManager.Load<SoundEffect>("hitSound");
            */

            // Look up shortcut references to the bones we are going to animate.
            leftBackWheelBone = tankModel.Bones["l_back_wheel_geo"];
            rightBackWheelBone = tankModel.Bones["r_back_wheel_geo"];
            leftFrontWheelBone = tankModel.Bones["l_front_wheel_geo"];
            rightFrontWheelBone = tankModel.Bones["r_front_wheel_geo"];
            leftSteerBone = tankModel.Bones["l_steer_geo"];
            rightSteerBone = tankModel.Bones["r_steer_geo"];
            turretBone = tankModel.Bones["turret_geo"];
            cannonBone = tankModel.Bones["canon_geo"];
            hatchBone = tankModel.Bones["hatch_geo"];

            // Store the original transform matrix for each animating bone.
            leftBackWheelTransform = leftBackWheelBone.Transform;
            rightBackWheelTransform = rightBackWheelBone.Transform;
            leftFrontWheelTransform = leftFrontWheelBone.Transform;
            rightFrontWheelTransform = rightFrontWheelBone.Transform;
            leftSteerTransform = leftSteerBone.Transform;
            rightSteerTransform = rightSteerBone.Transform;
            turretTransform = turretBone.Transform;
            cannonTransform = cannonBone.Transform;
            hatchTransform = hatchBone.Transform;

            // Allocate the transform matrix array.
            boneTransforms = new Matrix[tankModel.Bones.Count];
        }

        public void Update()
        {
            moving = false;

            direction = Vector3.Transform(new Vector3(0, 0, 1), Matrix.CreateRotationY(yaw));

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                Vector3 nextPosition = position + direction * 0.2f;
                nextBoundingSphere = boundingSphere;
                nextBoundingSphere.Center = nextPosition;

                if (!((nextPosition.X < 2f || nextPosition.X > width - 2f || nextPosition.Z < 2f || nextPosition.Z > height - 2f) ||
                    (nextPosition.X < 2f && nextPosition.Z < 2f) || (nextPosition.X > width - 2f && nextPosition.Z < 2f) ||
                    (nextPosition.X > width - 2f && nextPosition.Z > height - 2f) || (nextPosition.X < 2f && nextPosition.Z > height - 2f))
                    && !nextCollided)
                {
                    position += direction * 0.1f;
                }

                wheelRotationValue += 0.1f;

                moving = true;
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                Vector3 nextPosition = position - direction * 0.2f;
                nextBoundingSphere = boundingSphere;
                nextBoundingSphere.Center = nextPosition;

                if (!((nextPosition.X < 2f || nextPosition.X > width - 2f || nextPosition.Z < 2f || nextPosition.Z > height - 2f) ||
                    (nextPosition.X < 2f && nextPosition.Z < 2f) || (nextPosition.X > width - 2f && nextPosition.Z < 2f) ||
                    (nextPosition.X > width - 2f && nextPosition.Z > height - 2f) || (nextPosition.X < 2f && nextPosition.Z > height - 2f))
                    && !nextCollided)
                {
                    position -= direction * 0.1f;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.A) || Keyboard.GetState().IsKeyDown(Keys.D))
                    wheelRotationValue -= 0.2f;
                else
                    wheelRotationValue -= 0.1f;
            }


            if (Keyboard.GetState().IsKeyDown(Keys.A) && lastLeft && Keyboard.GetState().IsKeyDown(Keys.D) && !lastRight)
            {
                moveLeft = false;
                moveRight = true;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.A) && !lastLeft && Keyboard.GetState().IsKeyDown(Keys.D) && lastRight)
            {
                moveLeft = true;
                moveRight = false;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.A) && !lastLeft && !Keyboard.GetState().IsKeyDown(Keys.D))
            {
                moveLeft = true;
                moveRight = false;
            }
            else if (!Keyboard.GetState().IsKeyDown(Keys.A) && Keyboard.GetState().IsKeyDown(Keys.D) && !lastRight)
            {
                moveLeft = false;
                moveRight = true;
            }

            if(Keyboard.GetState().IsKeyDown(Keys.A) && Keyboard.GetState().IsKeyUp(Keys.D))
            {
                moveLeft = true;
                moveRight = false;
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.A) && Keyboard.GetState().IsKeyDown(Keys.D))
            {
                moveLeft = false;
                moveRight = true;
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.A) && Keyboard.GetState().IsKeyUp(Keys.D))
            {
                moveLeft = false;
                moveRight = false;
            }



            if(Keyboard.GetState().IsKeyDown(Keys.I))
            {
                if(cannonRotationValue > -1)
                    cannonRotationValue -= 0.04f;
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.K))
            {
                if (cannonRotationValue < 0)
                    cannonRotationValue += 0.04f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.J))
            {
                turretRotationValue += 0.04f;
            }

            else if (Keyboard.GetState().IsKeyDown(Keys.L))
            {
                turretRotationValue -= 0.04f;
            }

            previousPosition = position;

            if(moveLeft)
            {
                yaw += 0.05f;
                wheelRotationValue += 0.1f;

                if (steerRotationValue < 1f)
                    steerRotationValue += 0.05f;

                moving = true;
            }

            if(moveRight)
            {
                yaw -= 0.05f;
                wheelRotationValue += 0.1f;

                if (steerRotationValue > -1f)
                    steerRotationValue -= 0.05f;

                moving = true;
            }

            if(!moveRight && !moveLeft)
            {
                if (steerRotationValue > 0)
                    steerRotationValue -= 0.05f;
                if (steerRotationValue < 0)
                    steerRotationValue += 0.05f;
            }

            lastLeft = Keyboard.GetState().IsKeyDown(Keys.A);
            lastRight = Keyboard.GetState().IsKeyDown(Keys.D);

            boundingSphere.Center = position;
        }

        public void UpdatePlayer2(Tank tankP1)
        {
            moving = false;

            direction = Vector3.Transform(new Vector3(0, 0, 1), Matrix.CreateRotationY(yaw));

            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && canChangePlayer2Control)
            {
                canChangePlayer2Control = false;

                if (player2Control)
                    player2Control = false;
                else
                    player2Control = true;
            }

            else if (Keyboard.GetState().IsKeyUp(Keys.Enter))
                canChangePlayer2Control = true;

            if (player2Control)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.NumPad8))
                {
                    Vector3 nextPosition = position + direction * 0.2f;
                    nextBoundingSphere = boundingSphere;
                    nextBoundingSphere.Center = nextPosition;

                    if (!((nextPosition.X < 2f || nextPosition.X > width - 2f || nextPosition.Z < 2f || nextPosition.Z > height - 2f) ||
                        (nextPosition.X < 2f && nextPosition.Z < 2f) || (nextPosition.X > width - 2f && nextPosition.Z < 2f) ||
                        (nextPosition.X > width - 2f && nextPosition.Z > height - 2f) || (nextPosition.X < 2f && nextPosition.Z > height - 2f))
                        && !nextCollided)
                    {
                        position += direction * 0.1f;
                    }

                    wheelRotationValue += 0.1f;

                    moving = true;
                }

                else if (Keyboard.GetState().IsKeyDown(Keys.NumPad2))
                {
                    Vector3 nextPosition = position - direction * 0.2f;
                    nextBoundingSphere = boundingSphere;
                    nextBoundingSphere.Center = nextPosition;

                    if (!((nextPosition.X < 2f || nextPosition.X > width - 2f || nextPosition.Z < 2f || nextPosition.Z > height - 2f) ||
                        (nextPosition.X < 2f && nextPosition.Z < 2f) || (nextPosition.X > width - 2f && nextPosition.Z < 2f) ||
                        (nextPosition.X > width - 2f && nextPosition.Z > height - 2f) || (nextPosition.X < 2f && nextPosition.Z > height - 2f))
                        && !nextCollided)
                    {
                        position -= direction * 0.1f;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.NumPad4) || Keyboard.GetState().IsKeyDown(Keys.NumPad6))
                        wheelRotationValue -= 0.2f;
                    else
                        wheelRotationValue -= 0.1f;
                }


                if (Keyboard.GetState().IsKeyDown(Keys.NumPad4) && lastLeft && Keyboard.GetState().IsKeyDown(Keys.NumPad6) && !lastRight)
                {
                    moveLeft = false;
                    moveRight = true;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.NumPad4) && !lastLeft && Keyboard.GetState().IsKeyDown(Keys.NumPad6) && lastRight)
                {
                    moveLeft = true;
                    moveRight = false;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.NumPad4) && !lastLeft && !Keyboard.GetState().IsKeyDown(Keys.NumPad6))
                {
                    moveLeft = true;
                    moveRight = false;
                }
                else if (!Keyboard.GetState().IsKeyDown(Keys.NumPad4) && Keyboard.GetState().IsKeyDown(Keys.NumPad6) && !lastRight)
                {
                    moveLeft = false;
                    moveRight = true;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.NumPad4) && Keyboard.GetState().IsKeyUp(Keys.NumPad6))
                {
                    moveLeft = true;
                    moveRight = false;
                }
                else if (Keyboard.GetState().IsKeyUp(Keys.NumPad4) && Keyboard.GetState().IsKeyDown(Keys.NumPad6))
                {
                    moveLeft = false;
                    moveRight = true;
                }
                else if (Keyboard.GetState().IsKeyUp(Keys.NumPad4) && Keyboard.GetState().IsKeyUp(Keys.NumPad6))
                {
                    moveLeft = false;
                    moveRight = false;
                }



                if (Keyboard.GetState().IsKeyDown(Keys.NumPad7))
                {
                    if (cannonRotationValue > -1)
                        cannonRotationValue -= 0.04f;
                }

                else if (Keyboard.GetState().IsKeyDown(Keys.NumPad9))
                {
                    if (cannonRotationValue < 0)
                        cannonRotationValue += 0.04f;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.NumPad1))
                {
                    turretRotationValue += 0.04f;
                }

                else if (Keyboard.GetState().IsKeyDown(Keys.NumPad3))
                {
                    turretRotationValue -= 0.04f;
                }

                previousPosition = position;

                if (moveLeft)
                {
                    yaw += 0.05f;
                    wheelRotationValue += 0.1f;

                    if (steerRotationValue < 1f)
                        steerRotationValue += 0.05f;

                    moving = true;
                }

                if (moveRight)
                {
                    yaw -= 0.05f;
                    wheelRotationValue += 0.1f;

                    if (steerRotationValue > -1f)
                        steerRotationValue -= 0.05f;

                    moving = true;
                }

                if (!moveRight && !moveLeft)
                {
                    if (steerRotationValue > 0)
                        steerRotationValue -= 0.05f;
                    if (steerRotationValue < 0)
                        steerRotationValue += 0.05f;
                }

                lastLeft = Keyboard.GetState().IsKeyDown(Keys.NumPad4);
                lastRight = Keyboard.GetState().IsKeyDown(Keys.NumPad6);
            }

            else
            {
                if (Vector3.Distance(position, tankP1.position) < 20f)
                {
                    moving = true;

                    Vector3 newDirection = position - tankP1.position;
                    newDirection.Normalize();

                    direction = Vector3.Transform(new Vector3(0, 0, 1), Matrix.CreateRotationY(yaw));
                    direction.Normalize();

                    double newAngle = Math.Atan2(newDirection.Z, newDirection.X) - Math.Atan2(direction.Z, direction.X);

                    if(!turningAround)
                        yaw = MathHelper.Lerp(yaw, yaw - (float)newAngle, 0.05f);

                    Vector3 nextPosition = position + direction * 0.2f;
                    nextBoundingSphere = boundingSphere;
                    nextBoundingSphere.Center = nextPosition;

                    if (!((nextPosition.X < 2f || nextPosition.X > width - 2f || nextPosition.Z < 2f || nextPosition.Z > height - 2f) ||
                        (nextPosition.X < 2f && nextPosition.Z < 2f) || (nextPosition.X > width - 2f && nextPosition.Z < 2f) ||
                        (nextPosition.X > width - 2f && nextPosition.Z > height - 2f) || (nextPosition.X < 2f && nextPosition.Z > height - 2f))
                        && !nextCollided && !turningAround)
                    {
                        wheelRotationValue += 0.1f;
                        position += direction * 0.1f;
                    }
                    else
                    {
                        turningAround = true;
                        float turnYaw = yaw - 180;
                        yaw = MathHelper.Lerp(yaw, turnYaw, 0.05f);

                        if (yaw >= turnYaw)
                            turningAround = false;
                    }
                }
                else
                    moving = false;
            }

            boundingSphere.Center = position;
        }

        public void CheckCollision(BoundingSphere bs)
        {
            if (boundingSphere.Intersects(bs))
            {
                collided = true;
                /* Music / sound related
                hitSound.Play();
                */
            }
            else
                collided = false;
        }

        public void CheckNextCollision(BoundingSphere bs)
        {
            if (nextBoundingSphere.Intersects(bs))
                nextCollided = true;
            else
                nextCollided = false;
        }

        public void Draw()
        {
            // Calculate matrices based on the current animation position.
            Matrix wheelRotation = Matrix.CreateRotationX(wheelRotationValue);
            Matrix steerRotation = Matrix.CreateRotationY(steerRotationValue);
            Matrix turretRotation = Matrix.CreateRotationY(turretRotationValue);
            Matrix cannonRotation = Matrix.CreateRotationX(cannonRotationValue);
            Matrix hatchRotation = Matrix.CreateRotationX(hatchRotationValue);

            // Apply matrices to the relevant bones.
            leftBackWheelBone.Transform = wheelRotation * leftBackWheelTransform;
            rightBackWheelBone.Transform = wheelRotation * rightBackWheelTransform;
            leftFrontWheelBone.Transform = wheelRotation * leftFrontWheelTransform;
            rightFrontWheelBone.Transform = wheelRotation * rightFrontWheelTransform;
            leftSteerBone.Transform = steerRotation * leftSteerTransform;
            rightSteerBone.Transform = steerRotation * rightSteerTransform;
            turretBone.Transform = turretRotation * turretTransform;
            cannonBone.Transform = cannonRotation * cannonTransform;
            hatchBone.Transform = hatchRotation * hatchTransform;

            // Set the world matrix as the root transform of the model.
            tankModel.Root.Transform = world;

            normalNova = Vector3.Transform(tankOrientation, Matrix.CreateRotationY(-yaw));

            Matrix worldMatrix = Matrix.CreateScale(0.005f) * Matrix.CreateFromYawPitchRoll(yaw, normalNova.Z, -normalNova.X) * 
                Matrix.CreateTranslation(position);

            cannonOrientation = Vector3.Transform(Vector3.Forward, Matrix.CreateFromYawPitchRoll(turretRotationValue, cannonRotationValue, 0));

            newDirection = Vector3.Transform(new Vector3(0, 0, 1), Matrix.CreateFromYawPitchRoll(yaw, normalNova.Z, -normalNova.X));

            tankModel.CopyAbsoluteBoneTransformsTo(boneTransforms);

            // Draw the model.
            foreach (ModelMesh mesh in tankModel.Meshes)
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
