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
    class Map
    {
        #region Variables

        private GraphicsDevice graphicsDevice;
        private Texture2D texture;
        private Texture2D heightMap;
        private Color[] heightMapColors;
        private float[,] heightMapHeightValues;
        private BasicEffect effect;
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;


        private Matrix worldMatrix = Matrix.Identity;

        private VertexPositionNormalTexture[] vertices;
        private short[] indices;

        private int width;       
        private int height;

        #endregion

        #region Properties
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public BasicEffect Effect
        {
            get { return effect; }
            set { effect = value; }
        }
        #endregion

        public Map(GraphicsDevice graphicsDevice, Texture2D heightMap, Texture2D texture)
        {
            this.heightMap = heightMap;
            this.texture = texture;
            this.graphicsDevice = graphicsDevice;

            width = heightMap.Width;
            height = heightMap.Height;

            float aspectRatio = (float)graphicsDevice.Viewport.Width / graphicsDevice.Viewport.Height;
            Vector3 camaraNormal = new Vector3(0.0f, 1.0f, 0.0f);
            camaraNormal.Normalize();

            effect = new BasicEffect(graphicsDevice);
            effect.View = Matrix.CreateLookAt(new Vector3(1.0f, 1.0f, 5.0f), Vector3.Zero, camaraNormal);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 0.1f, 1000.0f);

            effect.EnableDefaultLighting();
            effect.LightingEnabled = true;
            effect.SpecularColor = new Vector3(0.3f, 0.3f, 0.3f);
            effect.VertexColorEnabled = false;
            effect.TextureEnabled = true;
            effect.Texture = this.texture;

            GetHeights();
            CreateVertices();
            CalcNormals();
            CreateIndices();

            //worldMatrix *= Matrix.CreateRotationX((float)Math.PI);
        }

        private void GetHeights()
        {
            heightMapColors = new Color[width * height];
            heightMap.GetData<Color>(heightMapColors);

            heightMapHeightValues = new float[width, height];
            for(int x = 0; x < width; x++)
            {
                for(int z = 0; z < height; z++)
                {
                    heightMapHeightValues[x, z] = heightMapColors[x + z * width].R / 255f * 15f;
                }
            }
        }

        private void CreateVertices()
        {
            vertices = new VertexPositionNormalTexture[width * height];

            for(int x = 0; x < width; x++)
            {
                for(int z = 0; z < height; z++)
                {
                    vertices[x + z * width].Position = new Vector3(x, heightMapHeightValues[x,z], z);
                    vertices[x + z * width].TextureCoordinate = new Vector2(x , z);
                }
            }
        }

        private void CreateIndices()
        {
            indices = new short[(short)(2 * (width - 1) * height)];

            int x = 0;
            int z = width;

            for (int i = 0; i < indices.Length / 2; i++)
            {
                indices[2 * i] = (short)z;
                indices[2 * i + 1] = (short)x;

                x++;
                z++;
            }

            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);

            indexBuffer = new IndexBuffer(graphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData<short>(indices);
        }

        private void CalcNormals()
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    Vector3 pCenter, p1, p2, p3, p4, p5;
                    Vector3 pCenterNormal, p1Normal, p2Normal, p3Normal, p4Normal;

                    if (x == 0 && z == 0) //Top left corner
                    {
                        pCenter = vertices[z * width + x].Position;

                        p1 = vertices[z * width + x + 1].Position;
                        p2 = vertices[(z + 1) * width + (x + 1)].Position;
                        p3 = vertices[(z + 1) * width + x].Position;

                        p1Normal = Vector3.Cross(p2 - pCenter, p1 - pCenter);
                        p1Normal.Normalize();

                        p2Normal = Vector3.Cross(p3 - pCenter, p2 - pCenter);
                        p2Normal.Normalize();

                        pCenterNormal = (p1Normal + p2Normal) / 2;
                        pCenterNormal.Normalize();

                        vertices[z * width + x].Normal = pCenterNormal;
                    }

                    if (x == width - 1 && z == 0) //Top right corner
                    {
                        pCenter = vertices[z * width + x].Position;

                        p1 = vertices[(z + 1) * width + x].Position;
                        p2 = vertices[(z + 1) * width + (x - 1)].Position;
                        p3 = vertices[z * width + (x - 1)].Position;

                        p1Normal = Vector3.Cross(p2 - pCenter, p1 - pCenter);
                        p1Normal.Normalize();

                        p2Normal = Vector3.Cross(p3 - pCenter, p2 - pCenter);
                        p2Normal.Normalize();

                        pCenterNormal = (p1Normal + p2Normal) / 2;
                        pCenterNormal.Normalize();

                        vertices[z * width + x].Normal = pCenterNormal;
                    }

                    if (x == 0 && z == height - 1) //Bottom left corner
                    {
                        pCenter = vertices[z * width + x].Position;

                        p1 = vertices[(z - 1) * width + x].Position;
                        p2 = vertices[(z - 1) * width + (x + 1)].Position;
                        p3 = vertices[z * width + (x + 1)].Position;

                        p1Normal = Vector3.Cross(p2 - pCenter, p1 - pCenter);
                        p1Normal.Normalize();

                        p2Normal = Vector3.Cross(p3 - pCenter, p2 - pCenter);
                        p2Normal.Normalize();

                        pCenterNormal = (p1Normal + p2Normal) / 2;
                        pCenterNormal.Normalize();

                        vertices[z * width + x].Normal = pCenterNormal;
                    }

                    if (x == width - 1 && z == height - 1) //Bottom right corner
                    {
                        pCenter = vertices[z * width + x].Position;

                        p1 = vertices[z * width + (x - 1)].Position;
                        p2 = vertices[(z - 1) * width + (x - 1)].Position;
                        p3 = vertices[(z - 1) * width + x].Position;

                        p1Normal = Vector3.Cross(p2 - pCenter, p1 - pCenter);
                        p1Normal.Normalize();

                        p2Normal = Vector3.Cross(p3 - pCenter, p2 - pCenter);
                        p2Normal.Normalize();

                        pCenterNormal = (p1Normal + p2Normal) / 2;
                        pCenterNormal.Normalize();

                        vertices[z * width + x].Normal = pCenterNormal;
                    }

                    if(x > 0 && x < width - 1 && z == 0) //Top vertices with the exception of the corners
                    {
                        pCenter = vertices[z * width + x].Position;

                        p1 = vertices[z * width + (x + 1)].Position;
                        p2 = vertices[(z + 1) * width + (x + 1)].Position;
                        p3 = vertices[(z + 1) * width + x].Position;
                        p4 = vertices[(z + 1) * width + (x - 1)].Position;
                        p5 = vertices[z * width + (x - 1)].Position;

                        p1Normal = Vector3.Cross(p2 - pCenter, p1 - pCenter);
                        p1Normal.Normalize();

                        p2Normal = Vector3.Cross(p3 - pCenter, p2 - pCenter);
                        p2Normal.Normalize();

                        p3Normal = Vector3.Cross(p4 - pCenter, p3 - pCenter);
                        p3Normal.Normalize();

                        p4Normal = Vector3.Cross(p5 - pCenter, p4 - pCenter);
                        p4Normal.Normalize();

                        pCenterNormal = (p1Normal + p2Normal + p3Normal + p4Normal) / 4;
                        pCenterNormal.Normalize();

                        vertices[z * width + x].Normal = pCenterNormal;
                    }

                    if (x > 0 && x < width - 1 && z == height - 1) //Bottom vertices with the exception of the corners
                    {
                        pCenter = vertices[z * width + x].Position;

                        p1 = vertices[z * width + (x - 1)].Position;
                        p2 = vertices[(z - 1) * width + (x - 1)].Position;
                        p3 = vertices[(z - 1) * width + x].Position;
                        p4 = vertices[(z - 1) * width + (x + 1)].Position;
                        p5 = vertices[z * width + (x + 1)].Position;

                        p1Normal = Vector3.Cross(p2 - pCenter, p1 - pCenter);
                        p1Normal.Normalize();

                        p2Normal = Vector3.Cross(p3 - pCenter, p2 - pCenter);
                        p2Normal.Normalize();

                        p3Normal = Vector3.Cross(p4 - pCenter, p3 - pCenter);
                        p3Normal.Normalize();

                        p4Normal = Vector3.Cross(p5 - pCenter, p4 - pCenter);
                        p4Normal.Normalize();

                        pCenterNormal = (p1Normal + p2Normal + p3Normal + p4Normal) / 4;
                        pCenterNormal.Normalize();

                        vertices[z * width + x].Normal = pCenterNormal;
                    }

                    if (z > 0 && z < height - 1 && x == 0) //Left vertices with the exception of the corners
                    {
                        pCenter = vertices[z * width + x].Position;

                        p1 = vertices[(z - 1) * width + x].Position;
                        p2 = vertices[(z - 1) * width + (x + 1)].Position;
                        p3 = vertices[z * width + (x + 1)].Position;
                        p4 = vertices[(z + 1) * width + (x + 1)].Position;
                        p5 = vertices[(z + 1) * width + x].Position;

                        p1Normal = Vector3.Cross(p2 - pCenter, p1 - pCenter);
                        p1Normal.Normalize();

                        p2Normal = Vector3.Cross(p3 - pCenter, p2 - pCenter);
                        p2Normal.Normalize();

                        p3Normal = Vector3.Cross(p4 - pCenter, p3 - pCenter);
                        p3Normal.Normalize();

                        p4Normal = Vector3.Cross(p5 - pCenter, p4 - pCenter);
                        p4Normal.Normalize();

                        pCenterNormal = (p1Normal + p2Normal + p3Normal + p4Normal) / 4;
                        pCenterNormal.Normalize();

                        vertices[z * width + x].Normal = pCenterNormal;
                    }

                    if (z > 0 && z < height - 1 && x == width - 1) //Right vertices with the exception of the corners
                    {
                        pCenter = vertices[z * width + x].Position;

                        p1 = vertices[(z + 1) * width + x].Position;
                        p2 = vertices[(z + 1) * width + (x - 1)].Position;
                        p3 = vertices[z * width + (x - 1)].Position;
                        p4 = vertices[(z - 1) * width + (x - 1)].Position;
                        p5 = vertices[(z - 1) * width + x].Position;

                        p1Normal = Vector3.Cross(p2 - pCenter, p1 - pCenter);
                        p1Normal.Normalize();

                        p2Normal = Vector3.Cross(p3 - pCenter, p2 - pCenter);
                        p2Normal.Normalize();

                        p3Normal = Vector3.Cross(p4 - pCenter, p3 - pCenter);
                        p3Normal.Normalize();

                        p4Normal = Vector3.Cross(p5 - pCenter, p4 - pCenter);
                        p4Normal.Normalize();

                        pCenterNormal = (p1Normal + p2Normal + p3Normal + p4Normal) / 4;
                        pCenterNormal.Normalize();

                        vertices[z * width + x].Normal = pCenterNormal;
                    }
                }
            }

            for (int x = 1; x <= width - 2; x++) //Middle
			{
                for (int z = 1; z <= height - 2; z++)
			    {
			        Vector3 pCenter, p1, p2, p3, p4, p5, p6, p7, p8;
                    Vector3 pCenterNormal, p1Normal, p2Normal, p3Normal, p4Normal, p5Normal, p6Normal, p7Normal, p8Normal;

                    pCenter = vertices[z * width + x].Position;

                    p1 = vertices[(z - 1) * width + x - 1].Position;
                    p2 = vertices[(z - 1) * width + x].Position;
                    p3 = vertices[(z - 1) * width + x + 1].Position;
                    p4 = vertices[z * width + x + 1].Position;
                    p5 = vertices[(z + 1) * width + x + 1].Position;
                    p6 = vertices[(z + 1) * width + x].Position;
                    p7 = vertices[(z + 1) * width + x - 1].Position;
                    p8 = vertices[z * width + x - 1].Position;

                    p1Normal = Vector3.Cross(p2 - pCenter, p1 - pCenter);
                    p1Normal.Normalize();

                    p2Normal = Vector3.Cross(p3 - pCenter, p2 - pCenter);
                    p2Normal.Normalize();

                    p3Normal = Vector3.Cross(p4 - pCenter, p3 - pCenter);
                    p3Normal.Normalize();

                    p4Normal = Vector3.Cross(p5 - pCenter, p4 - pCenter);
                    p4Normal.Normalize();

                    p5Normal = Vector3.Cross(p6 - pCenter, p5 - pCenter);
                    p5Normal.Normalize();

                    p6Normal = Vector3.Cross(p7 - pCenter, p6 - pCenter);
                    p6Normal.Normalize();

                    p7Normal = Vector3.Cross(p8 - pCenter, p7 - pCenter);
                    p7Normal.Normalize();

                    p8Normal = Vector3.Cross(p1 - pCenter, p8 - pCenter);
                    p8Normal.Normalize();

                    pCenterNormal = (p1Normal + p2Normal + p3Normal + p4Normal + p5Normal + p6Normal + p7Normal + p8Normal) / 8;
                    pCenterNormal.Normalize();

                    vertices[z * width + x].Normal = pCenterNormal;
			    }
            }
        }

        public Vector3 CalcSurfaceFollow(Vector3 objectFollowing, float offset)
        {
            Vector3 objectFollowingPosition = objectFollowing;

            //Determins in which square of the terrain the object is located
            Vector3 posA = new Vector3((int)objectFollowingPosition.X, objectFollowingPosition.Y, (int)objectFollowingPosition.Z);
            Vector3 posB = new Vector3((int)objectFollowingPosition.X + 1, objectFollowingPosition.Y, (int)objectFollowingPosition.Z);
            Vector3 posC = new Vector3((int)objectFollowingPosition.X, objectFollowingPosition.Y, (int)objectFollowingPosition.Z + 1);
            Vector3 posD = new Vector3((int)objectFollowingPosition.X + 1, objectFollowingPosition.Y, (int)objectFollowingPosition.Z + 1);

            float da, db, dc, dd, dab, dcd;

            //Calcs the distance between the X coordinate of the object and the X coordinate of each of the 4 vertices of the square
            da = Math.Abs(posA.X - objectFollowingPosition.X);
            db = Math.Abs(posB.X - objectFollowingPosition.X);
            dc = Math.Abs(posC.X - objectFollowingPosition.X);
            dd = Math.Abs(posD.X - objectFollowingPosition.X);

            //Calcs the distance between the Z coordinate of the object and the Z coordinate of the top and bottom edges of the square
            dab = Math.Abs(posA.Z - objectFollowingPosition.Z);
            dcd = Math.Abs(posC.Z - objectFollowingPosition.Z);

            //Calcs the weighted arithmetic height (Y) of both edges, considering the distance between the X coordinate of their vertices and the X coordinates of the object
            float yab = heightMapHeightValues[(int)posA.X, (int)posA.Z] * db + heightMapHeightValues[(int)posB.X, (int)posB.Z] * da;
            float ycd = heightMapHeightValues[(int)posC.X, (int)posC.Z] * dd + heightMapHeightValues[(int)posD.X, (int)posD.Z] * dc;

            //Calcs the final weighted arithmetic height (Y)
            float y = yab * dcd + ycd * dab;

            return objectFollowing = new Vector3(objectFollowing.X, y + offset, objectFollowing.Z);
        }

        public Vector3 CalcNormalsFollow(Vector3 objectFollowing)
        {
            Vector3 objectFollowingPosition = objectFollowing;

            //Determins in which square of the terrain the object is located
            Vector3 posA = new Vector3((int)objectFollowingPosition.X, objectFollowingPosition.Y, (int)objectFollowingPosition.Z);
            Vector3 posB = new Vector3((int)objectFollowingPosition.X + 1, objectFollowingPosition.Y, (int)objectFollowingPosition.Z);
            Vector3 posC = new Vector3((int)objectFollowingPosition.X, objectFollowingPosition.Y, (int)objectFollowingPosition.Z + 1);
            Vector3 posD = new Vector3((int)objectFollowingPosition.X + 1, objectFollowingPosition.Y, (int)objectFollowingPosition.Z + 1);

            float da, db, dc, dd, dab, dcd;

            //Calcs the distance between the X coordinate of the object and the X coordinate of each of the 4 vertices of the square
            da = Math.Abs(posA.X - objectFollowingPosition.X);
            db = Math.Abs(posB.X - objectFollowingPosition.X);
            dc = Math.Abs(posC.X - objectFollowingPosition.X);
            dd = Math.Abs(posD.X - objectFollowingPosition.X);

            //Calcs the distance between the Z coordinate of the object and the Z coordinate of the top and bottom edges of the square
            dab = Math.Abs(posA.Z - objectFollowingPosition.Z);
            dcd = Math.Abs(posC.Z - objectFollowingPosition.Z);

            Vector3 normal1 = Vector3.Zero;
            Vector3 normal2 = Vector3.Zero;
            Vector3 normal3 = Vector3.Zero;
            Vector3 normal4 = Vector3.Zero;
            Vector3 normalWeighted1 = Vector3.Zero;
            Vector3 normalWeighted2 = Vector3.Zero;
            Vector3 normalWeightedFinal = Vector3.Zero;

            for (int i = 0; i < vertices.Length; i++)
			{
			    if(vertices[i].Position.X == posA.X && vertices[i].Position.Z == posA.Z)
                {
                    normal1 = vertices[i].Normal;
                }

                else if(vertices[i].Position.X == posB.X && vertices[i].Position.Z == posB.Z)
                {
                    normal2 = vertices[i].Normal;
                }

                else if (vertices[i].Position.X == posC.X && vertices[i].Position.Z == posC.Z)
                {
                    normal3 = vertices[i].Normal;
                }

                else if (vertices[i].Position.X == posD.X && vertices[i].Position.Z == posD.Z)
                {
                    normal4 = vertices[i].Normal;
                }
			}

            //Calcs the weighted arithmetic normal vector of both edges, considering the distance between the X coordinate of their vertices and the X coordinates of the object
            normalWeighted1 = normal1 * db + normal2 * da;
            normalWeighted1.Normalize();
            normalWeighted2 = normal3 * dd + normal4 * dc;
            normalWeighted2.Normalize();

            //Calcs the final weighted arithmetic normal vector
            normalWeightedFinal = normalWeighted1 * dcd + normalWeighted2 * dab;
            normalWeightedFinal.Normalize();

            return normalWeightedFinal;
        }

        public void Draw()
        {
            //RasterizerState rs = new RasterizerState();
            //rs.CullMode = CullMode.None;
            //graphicsDevice.RasterizerState = rs;

            effect.World = worldMatrix;

            effect.CurrentTechnique.Passes[0].Apply();

            graphicsDevice.SetVertexBuffer(vertexBuffer);
            graphicsDevice.Indices = indexBuffer;

            int offset = 0;

            for (int i = 0; i < height; i++)
            {
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, vertices.Length, offset, (width - 1) * 2);

                offset += width * 2;
            }        
        }
    }
}
