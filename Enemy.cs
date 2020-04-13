using Chroma;
using Chroma.Graphics;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using static System.Math;

namespace Chromastein
{
    class Enemy
    {

        public enum EnemyType
        {
            Soldier = 0,
            Boss = 1
        }

        public static string AssetsFolder = "/Sprites/Enemies/";

        public EnemyType ObjType;
        public Spritesheet EnemySheet;
        public int PosX, PosY;
        /// <summary>
        /// Animation state for the spritesheet
        /// </summary>
        public int EnemyState = 0;
        public float ZIndex = 0;

        public Enemy(int positionX, int positionY, EnemyType thisType)
        {
            ObjType = thisType;
            PosY = positionX;
            PosX = positionY;
        }

        public virtual void InitContent(string contentRoot)
        {
            contentRoot += AssetsFolder;
            switch (ObjType)
            {
                case EnemyType.Soldier:
                    EnemySheet = new Spritesheet(contentRoot + "guard.png");
                    break;
                default:
                    EnemySheet = new Spritesheet(contentRoot + "fuckedup.png");
                    break;
            }
            EnemySheet.CellWidth = 64;
            EnemySheet.CellHeight = 64;
        }

        public virtual void Draw(RenderContext context, Vector2 playerPos, Vector2 playerDir, Vector2 playerPlane, Vector2 screenSize)
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
            double scale = size / EnemySheet.Texture.Height;

            int x = -size / 2 + spriteScreenX;

            Vector2 TexPosition = new Vector2(x, -size / 2 + screenSize.Y / 2);
            Vector2 TexScale = new Vector2((float)scale, (float)scale);

            EnemySheet.Draw(context, EnemyState, TexPosition, TexScale, Vector2.Zero, 0);
        }

        public virtual void Update(float deltaTime, Vector2 playerPos)
        {
            float dX = playerPos.X - PosX;
            float dY = playerPos.Y - PosY;

            float dist = (float)Sqrt(dX * dX + dY * dY);
            if(dist > 4)
            {
                Console.WriteLine(dist);
            }
        }

    }
}
