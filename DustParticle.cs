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
    class DustParticle
    {
        private GraphicsDevice graphicsDevice;
        private BasicEffect effect;
        private Matrix worldMatrix = Matrix.Identity;

        private VertexPositionNormalTexture[] vertices;
        private short[] indices;
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;
        private Vector3 position, direction;
        private float yaw, pitch, pitchSpeed;

        private float speed;

        public float Yaw
        {
            get { return yaw; }
            set { yaw = value; }
        }


        public float Pitch
        {
            get { return pitch; }
            set { pitch = value; }
        }
        public float PitchSpeed
        {
            get { return pitchSpeed; }
            set { pitchSpeed = value; }
        }

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Vector3 Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        public DustParticle(GraphicsDevice graphicsDevice, BasicEffect effect)
        {
            this.graphicsDevice = graphicsDevice;
            this.effect = effect;        

            CreateLines();
        }

        private void CreateLines()
        {
            float lineLenght = 1.5f;
            int vertexCount = 2;
            vertices = new VertexPositionNormalTexture[vertexCount];

            vertices[0] = new VertexPositionNormalTexture(new Vector3(0.0f, -lineLenght, 0.0f), Vector3.Zero, Vector2.Zero);
            vertices[1] = new VertexPositionNormalTexture(new Vector3(0.0f, 0.0f, 0.0f), Vector3.Zero, Vector2.Zero);

            indices = new short[2];

            indices[0] = 0;
            indices[1] = 1;

            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);

            indexBuffer = new IndexBuffer(graphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData<short>(indices);
        }

        public void Update()
        {
            if (pitch > -2.5f)
                pitch -= pitchSpeed;

            direction = Vector3.Transform(new Vector3(0, 1, 0), Matrix.CreateFromYawPitchRoll(yaw, pitch, 0));

            position += direction * speed;

            worldMatrix = Matrix.CreateScale(0.1f) * Matrix.CreateFromYawPitchRoll(yaw, pitch, 0) * Matrix.CreateTranslation(position);
        }

        public void Draw()
        {
            effect.World = worldMatrix;

            effect.CurrentTechnique.Passes[0].Apply();

            graphicsDevice.SetVertexBuffer(vertexBuffer);
            graphicsDevice.Indices = indexBuffer;

            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, vertices.Length, 0, 1);
        }
    }
}
