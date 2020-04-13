using Chroma;
using Chroma.Graphics;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using static System.Math;

namespace Chromastein
{
    class Object
    {

        public enum Type
        {
            Lamp = 0,
            Armor = 1,
            Table = 2,
            Plant = 3
        }

        public static string AssetsFolder = "/Sprites/";

        public Type ObjType;
        public Texture ObjTexture;
        public int PosX, PosY;
        public float ZIndex = 0;
        public bool Block;

        public Object(int positionX, int positionY, bool block, Type thisType)
        {
            ObjType = thisType;
            PosY = positionX;
            PosX = positionY;
            Block = block;
        }

        public void InitContent(string contentRoot)
        {
            contentRoot += AssetsFolder;
            switch (ObjType)
            {
                case Type.Lamp:
                    ObjTexture = new Texture(contentRoot + "lamp.png");
                    break;
                case Type.Armor:
                    ObjTexture = new Texture(contentRoot + "armor.png");
                    break;
                case Type.Table:
                    ObjTexture = new Texture(contentRoot + "table-chairs.png");
                    break;
                case Type.Plant:
                    ObjTexture = new Texture(contentRoot + "plant-green.png");
                    break;
                default:
                    ObjTexture = new Texture(contentRoot + "fuckedup.png");
                    break;
            }
        }

        public void Draw(RenderContext context, Vector2 playerPos, Vector2 playerDir, Vector2 playerPlane, Vector2 screenSize)
        {

            // Distance from the sprite to the player
            float dX = PosX + 0.5f - playerPos.X;
            float dY = PosY + 0.5f - playerPos.Y;

            // Transform sprite with the inverse camera matrix
            // [ planeX   dirX ] -1                                       [ dirY      -dirX ]
            // [               ]       =  1/(planeX*dirY-dirX*planeY) *   [                 ]
            // [ planeY   dirY ]                                          [ -planeY  planeX ]

            double invDet = 1.0 / (playerPlane.X * playerDir.Y - playerDir.X * playerPlane.Y); // Invert for correct matrix multiplication

            double transformX = invDet * (playerDir.Y * dX - playerDir.X * dY);
            double transformY = invDet * (-playerPlane.Y * dX + playerPlane.X * dY); // Screen depth, Z value

            int spriteScreenX = (int)((screenSize.X / 2) * (1 + transformX / transformY));

            // How big the sprite is in pixels
            int size = Abs((int)(screenSize.Y / (transformY))); // Using 'transformY' instead of the real distance prevents fisheye

            // How big the sprite is relative to its usual height since I have to use scale
            double scale = size / ObjTexture.Height;

            int x = -size / 2 + spriteScreenX;

            Vector2 TexPosition = new Vector2(x, -size / 2 + screenSize.Y / 2);
            Vector2 TexScale = new Vector2((float)scale, (float)scale);

            context.DrawTexture(ObjTexture, TexPosition, TexScale, Vector2.Zero, 0);
        }

        public void Update(float deltaTime)
        {

        }

    }
}
