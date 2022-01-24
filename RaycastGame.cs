using System;
using System.Collections.Generic;
using static System.Math;
using Chroma;
using Chroma.Input;
using Chroma.Graphics;
using Chroma.Graphics.Batching;
using Chroma.Graphics.TextRendering;
using System.IO;
using System.Numerics;
using System.Drawing;
using Color = Chroma.Graphics.Color;
using Chroma.Diagnostics;
using Chroma.Graphics.TextRendering.TrueType;
using Chroma.Input.GameControllers;

namespace Chromastein
{
    class RaycastGame : Game
    {

        #region Debug WorldMap

        public int[,] WorldMap = new int[,]
        {
            {4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,7,7,7,7,7,7,7,7},
            {4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,7,0,0,0,0,0,0,7},
            {4,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,7},
            {4,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,7},
            {4,0,3,0,0,0,0,0,0,0,0,0,0,0,0,0,7,0,0,0,0,0,0,7},
            {4,0,4,0,0,0,0,5,5,5,5,5,5,5,5,5,7,7,0,7,7,7,7,7},
            {4,0,5,0,0,0,0,5,0,5,0,5,0,5,0,5,7,0,0,0,7,7,7,1},
            {4,0,6,0,0,0,0,5,0,0,0,0,0,0,0,5,7,0,0,0,0,0,0,8},
            {4,0,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,7,7,7,1},
            {4,0,8,0,0,0,0,5,0,0,0,0,0,0,0,5,7,0,0,0,0,0,0,8},
            {4,0,0,0,0,0,0,5,0,0,0,0,0,0,0,5,7,0,0,0,7,7,7,1},
            {4,0,0,0,0,0,0,5,5,5,5,0,5,5,5,5,7,7,7,7,7,7,7,1},
            {6,6,6,6,6,6,6,6,6,6,6,0,6,6,6,6,6,6,6,6,6,6,6,6},
            {8,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4},
            {6,6,6,6,6,6,0,6,6,6,6,0,6,6,6,6,6,6,6,6,6,6,6,6},
            {4,4,4,4,4,4,0,4,4,4,6,0,6,2,2,2,2,2,2,2,3,3,3,3},
            {4,0,0,0,0,0,0,0,0,4,6,0,6,2,0,0,0,0,0,2,0,0,0,2},
            {4,0,0,0,0,0,0,0,0,0,0,0,6,2,0,0,5,0,0,2,0,0,0,2},
            {4,0,0,0,0,0,0,0,0,4,6,0,6,2,0,0,0,0,0,2,2,0,2,2},
            {4,0,6,0,6,0,0,0,0,4,6,0,0,0,0,0,5,0,0,0,0,0,0,2},
            {4,0,0,5,0,0,0,0,0,4,6,0,6,2,0,0,0,0,0,2,2,0,2,2},
            {4,0,6,0,6,0,0,0,0,4,6,0,6,2,0,0,5,0,0,2,0,0,0,2},
            {4,0,0,0,0,0,0,0,0,4,6,0,6,2,0,0,0,0,0,2,0,0,0,2},
            {4,4,4,4,4,4,4,4,4,4,1,1,1,2,2,2,2,2,2,3,3,3,3,3}
        };

        public Object[] SpriteList = new Object[]
        {
            new Object(14, 6, true, Object.Type.Armor),
            new Object(12, 6, true, Object.Type.Armor),
            new Object(10, 6, true, Object.Type.Armor),
            new Object(8, 6, true, Object.Type.Armor),
            new Object(11, 20, false, Object.Type.Lamp),
            new Object(11, 17, false, Object.Type.Lamp),
            new Object(11, 14, false, Object.Type.Lamp),
            new Object(11, 11, false, Object.Type.Lamp),
            new Object(14, 22, true, Object.Type.Plant),
            new Object(8, 22, true, Object.Type.Plant),
            new Object(1, 22, true, Object.Type.Table),
        };

        public Enemy[] EnemyList = new Enemy[]
        {
            new Enemy(11, 11, Enemy.EnemyType.Soldier),
            new Enemy(6, 11, Enemy.EnemyType.Soldier),
            new Enemy(3, 4, Enemy.EnemyType.Boss),
        };
        #endregion

        #region Meta Variables

        public static RaycastGame Instance;

        public int MapWidth;
        public int MapHeight;
        public int ScreenWidth = 1280;
        public int ScreenHeight = 720;
        public Vector2 ScreenCenter;
        public int TextureSize = 64;
        public string ContentPath;

        public static int ScreenWidthDivision = 1;
        public static int ScreenHeightDivision = 1;

        public Object[,] SpriteMap;

        private TrueTypeFont DebugFont;
        private string DebugText;
        private bool FlatRender = false;
        private bool NoClip = false;
        private bool RenderSprites = true;
        private bool MiniMap = true;
        #endregion

        private Vector2 PlayerPos = new Vector2(22.5f, 11.5f);
        private double DirX = -1, DirY = 0;
        private double PlaneX = 0, PlaneY = 0.66;
        private const double PlayerMoveSpeed = 5.0;
        private const double PlayerRotSpeed = 3.0;
        private const float PlayerSize = 0.2f;

        private Texture[] WallTextures;

        public RaycastGame()
        {
            Instance = this;
            int DisplayWidth = Graphics.FetchDisplay(0).Bounds.Width; // Why the hell are these floats
            int DisplayHeight = Graphics.FetchDisplay(0).Bounds.Height; // Why the hell are these floats
            ScreenCenter = new Vector2(DisplayWidth / 2, DisplayHeight / 2);
            Graphics.VerticalSyncMode = VerticalSyncMode.None;
            Graphics.LimitFramerate = false;

            ContentPath = Path.GetFullPath("./Content") + "/";

            Window.Size = new Size(ScreenWidth, ScreenHeight);
            Window.CenterOnScreen();
            Window.Position = new Vector2(Max(0, Window.Position.X), Max(0, Window.Position.Y));

            MapWidth = WorldMap.GetLength(0);
            MapHeight = WorldMap.GetLength(1);
            SpriteMap = new Object[MapWidth, MapHeight];
            foreach (Object obj in SpriteList)
            {
                SpriteMap[obj.PosX, obj.PosY] = obj;
            }

            DebugFont = new TrueTypeFont(ContentPath + "DooM.ttf", 24);
            LoadTextures();

            // Load sprites
            foreach (Object sprite in SpriteList)
            {
                sprite.InitContent(ContentPath);
            }

            // Load enemies
            foreach (Enemy enemy in EnemyList)
            {
                enemy.InitContent(ContentPath);
            }
        }

        private void LoadTextures()
        {
            Texture allWalls = new Texture(ContentPath + "walls.jpeg");
            int Columns = (int)allWalls.Width / TextureSize;
            int Rows = (int)allWalls.Height / TextureSize;
            // cookie undid his stupid! :D
            WallTextures = new Texture[Columns * Rows];
            for (int cellY = 0; cellY < Rows; cellY++)
            {
                for (int cellX = 0; cellX < Columns; cellX++)
                {
                    WallTextures[(Columns * cellY) + cellX] = new Texture((ushort)TextureSize, (ushort)TextureSize);
                    for (int texX = 0; texX < TextureSize; texX++)
                    {
                        for (int texY = 0; texY < TextureSize; texY++)
                        {
                            Color wallPixel = allWalls.GetPixel(cellX * TextureSize + texX,
                                cellY * TextureSize + texY);
                            // TODO: Workaround for chroma edge case, remove this line when fixed
                            wallPixel = new Color(wallPixel.A, wallPixel.B, wallPixel.G, wallPixel.R);
                            WallTextures[(Columns * cellY) + cellX].SetPixel(
                                texX, texY,
                                wallPixel
                                );
                        }
                    }
                    WallTextures[(Columns * cellY) + cellX].Flush();
                }
            }
        }

        protected override void ControllerConnected(ControllerEventArgs e)
        {
            // 0.2 deadzone
            Controller.SetDeadZoneAllAxes(0, 6553);
        }

        protected override void Draw(RenderContext context)
        {
            context.Clear(Color.Gray);

            int leftRayX = 0, leftRayY = 0, rightRayX = 0, rightRayY = 0;

            List<Object> spritesDrawn = new List<Object>();

            for (int x = 0; x <= ScreenWidth; x += ScreenWidthDivision)
            {
                // Calculate the direction needed for the ray, and the cam space
                double cameraX = 2 * x / (double)ScreenWidth - 1;
                double rayDirX = DirX + PlaneX * cameraX;
                double rayDirY = DirY + PlaneY * cameraX;

                // Where is the ray located in the map?
                int mapX = (int)PlayerPos.X;
                int mapY = (int)PlayerPos.Y;

                // These variables tell us how far until the next X side of the matrix 
                // Or the next Y side, relative to the current position
                double sideDistX;
                double sideDistY;

                // Same principle as the sideDistX/Y except its from the *last* X or Y side
                // Also prevent deviding by 0 (does C# even care about this? Should look that up)
                double deltaDistX = (rayDirY == 0) ? 0 : ((rayDirX == 0) ? 1 : Abs(1 / rayDirX));
                double deltaDistY = (rayDirX == 0) ? 0 : ((rayDirY == 0) ? 1 : Abs(1 / rayDirY));
                double perpWallDist;

                // Either -1 or 1 depending on what direction we need to step in next
                int stepX;
                int stepY;

                int hit = 0; // Did we hit a wall?
                int side = 0; // Which side we hit the wall on (North/South or East/West)

                // Actually calculate side and directions
                if (rayDirX < 0)
                {
                    stepX = -1;
                    sideDistX = (PlayerPos.X - mapX) * deltaDistX;
                }
                else
                {
                    stepX = 1;
                    sideDistX = (mapX + 1.0 - PlayerPos.X) * deltaDistX;
                }
                if (rayDirY < 0)
                {
                    stepY = -1;
                    sideDistY = (PlayerPos.Y - mapY) * deltaDistY;
                }
                else
                {
                    stepY = 1;
                    sideDistY = (mapY + 1.0 - PlayerPos.Y) * deltaDistY;
                }

                // Finally, Actually, do the raycasting using the DDA algorithm
                while (hit == 0)
                {
                    // Jump to the next map square/X direction/Y direction
                    if (sideDistX < sideDistY)
                    {
                        sideDistX += deltaDistX;
                        mapX += stepX;
                        side = 0;
                    }
                    else
                    {
                        sideDistY += deltaDistY;
                        mapY += stepY;
                        side = 1;
                    }

                    if (Abs(mapX) > 255 || Abs(mapY) > 255) break;
                    if (mapX >= WorldMap.GetLength(0) || mapY >= WorldMap.GetLength(1)) continue;
                    if (mapX < 0 || mapY < 0) continue;

                    // Check if ray has hit a wall
                    if (WorldMap[mapX, mapY] > 0) hit = 1;
                    if (RenderSprites)
                    {
                        // Check if a sprite is in view
                        if (SpriteMap[mapX, mapY] != null)
                        {
                            if (!spritesDrawn.Contains(SpriteMap[mapX, mapY]))
                            {
                                // Calculate sprite distance from the player so we can render it properly
                                Object sprite = SpriteMap[mapX, mapY];
                                double spriteDist;
                                // Calculate distance projected on camera direction (Euclidean distance will give fisheye effect!)
                                if (side == 0)
                                {
                                    spriteDist = (mapX - PlayerPos.X + (1 - stepX) / 2) / rayDirX;
                                }
                                else
                                {
                                    spriteDist = (mapY - PlayerPos.Y + (1 - stepY) / 2) / rayDirY;
                                }
                                int usableDist = (int)(spriteDist * 1000);
                                sprite.ZIndex = usableDist;
                                spritesDrawn.Add(sprite);
                                context.Batch(() => sprite.Draw(
                                    context,
                                    PlayerPos,
                                    new Vector2((float)DirX, (float)DirY),
                                    new Vector2((float)PlaneX, (float)PlaneY),
                                    new Vector2(ScreenWidth, ScreenHeight)
                                    ), (int)sprite.ZIndex);
                            }
                        }
                    }
                }

                if (hit == 0) continue;

                if (x == 0)
                {
                    leftRayX = mapX;
                    leftRayY = mapY;
                }
                else if (x == ScreenWidth)
                {
                    rightRayX = mapX;
                    rightRayY = mapY;
                }

                // Calculate distance projected on camera direction (Euclidean distance will give fisheye effect!)
                if (side == 0)
                {
                    perpWallDist = (mapX - PlayerPos.X + (1 - stepX) / 2) / rayDirX;
                }
                else
                {
                    perpWallDist = (mapY - PlayerPos.Y + (1 - stepY) / 2) / rayDirY;
                }

                // Calculate height of the strip that we draw to the screen
                int lineHeight = (int)(ScreenHeight / perpWallDist);

                // Calculate lowest and highest pixel to fill in current stripe
                int drawStart = -lineHeight / 2 + ScreenHeight / 2;
                if (drawStart < 0) drawStart = 0;
                int drawEnd = lineHeight / 2 + ScreenHeight / 2;
                if (drawEnd >= ScreenHeight) drawEnd = ScreenHeight - 1;

                // Calculate texture rendering
                int texNum = WorldMap[mapX, mapY] - 1; // Subtract 1 so we are 0 indexed

                // Calculate where the wall was hit
                double wallX;
                if (side == 0) wallX = PlayerPos.Y + perpWallDist * rayDirY;
                else wallX = PlayerPos.X + perpWallDist * rayDirX;
                wallX -= Floor(wallX);

                // Calculate the X coordinate of the txture
                int texX = (int)(wallX * TextureSize);
                if (side == 0 && rayDirX > 0) texX = TextureSize - texX - 1;
                if (side == 1 && rayDirY < 0) texX = TextureSize - texX - 1;

                Color color;

                if (!FlatRender)
                {
                    // Draw textures using strips instead of pixels
                    int texY = Min(drawStart, -(lineHeight - ScreenHeight) / 2);
                    Texture stripTex = WallTextures[texNum];
                    Rectangle SourceRectangle = new Rectangle(texX, 0, ScreenWidthDivision, TextureSize);
                    Vector2 Position = new Vector2(x, texY);
                    Vector2 Scale = new Vector2(1, (float)lineHeight / TextureSize);
                    Color StripColor = side == 1 ? Color.Gray : Color.White;
                    int usableDist = (int)(perpWallDist * 1000);
                    context.Batch(() =>
                    {
                        if (!stripTex.ColorMask.Equals(StripColor)) stripTex.ColorMask = StripColor;
                        context.DrawTexture(stripTex, Position, Scale, Vector2.Zero, 0, SourceRectangle);
                    }, usableDist);
                }
                else
                {
                    // Debugging wall colors for flat renderer
                    color = (WorldMap[mapX, mapY]) switch
                    {
                        1 => Color.Red,
                        2 => Color.Green,
                        3 => Color.Blue,
                        4 => Color.Beige,
                        5 => Color.Aquamarine,
                        6 => Color.HotPink,
                        7 => Color.Purple,
                        8 => Color.Brown,
                        _ => Color.Yellow,
                    };

                    // Darken the color to create perspective
                    if (side == 1)
                    {
                        color.R /= 2;
                        color.G /= 2;
                        color.B /= 2;
                    }
                    int usableDist = (int)(perpWallDist * 1000);

                    // Actually draw the pixels of the stripe as a vertical line
                    DrawColorStrip(context, x, drawStart, drawEnd, usableDist, color);
                }
            }

            if (RenderSprites)
            {
                foreach (Enemy enemy in EnemyList)
                {
                    // Calculate distance projected on camera direction (Euclidean distance will give fisheye effect!)
                    float dX = PlayerPos.X - enemy.PosX;
                    float dY = PlayerPos.Y - enemy.PosY;
                    // Calculate sprite distance from the player so we can render it properly
                    double spriteDist;
                    spriteDist = Sqrt(dX * dX + dY * dY);
                    int usableDist = (int)(spriteDist * 1000);
                    enemy.ZIndex = usableDist;
                    context.Batch(() => enemy.Draw(
                        context,
                        PlayerPos,
                        new Vector2((float)DirX, (float)DirY),
                        new Vector2((float)PlaneX, (float)PlaneY),
                        new Vector2(ScreenWidth, ScreenHeight)
                        ), (int)enemy.ZIndex);
                }
            }

            context.DrawBatch(DrawOrder.FrontToBack);

            if (MiniMap)
            {
                // Move these to meta variables later, im tired
                Vector2 MinimapSize = new Vector2(300, 300);
                Vector2 MinimapPosition = new Vector2(ScreenWidth - MinimapSize.X, 0);
                // Render minimap
                for (int x = 0; x < MapWidth; x++)
                {
                    for (int y = 0; y < MapHeight; y++)
                    {
                        // Debugging wall colors for flat renderer
                        var color = (WorldMap[y, x]) switch
                        {
                            1 => Color.Red,
                            2 => Color.Green,
                            3 => Color.Blue,
                            4 => Color.Beige,
                            5 => Color.Aquamarine,
                            6 => Color.HotPink,
                            7 => Color.Purple,
                            8 => Color.Brown,
                            _ => Color.Yellow,
                        };
                        Vector2 tileSize = new Vector2(MinimapSize.X / MapWidth, MinimapSize.Y / MapHeight);
                        Vector2 tilePos = new Vector2(tileSize.X * x, tileSize.Y * y);
                        context.Rectangle(ShapeMode.Fill, MinimapPosition + tilePos, tileSize.X, tileSize.Y, color);
                    }
                }

                Vector2 tileRefSize = new Vector2(MinimapSize.X / MapWidth, MinimapSize.Y / MapHeight);
                Vector2 playerDotSize = tileRefSize / 2;
                Vector2 playerDotPos = new Vector2(tileRefSize.X * PlayerPos.Y, tileRefSize.Y * PlayerPos.X);
                playerDotPos -= playerDotSize / 2;
                context.Rectangle(ShapeMode.Fill, MinimapPosition + playerDotPos, playerDotSize.X, playerDotSize.Y, Color.Red);

                context.Line(MinimapPosition + playerDotPos + (playerDotSize / 2),
                    MinimapPosition + new Vector2(tileRefSize.X * leftRayY, tileRefSize.Y * leftRayX),
                    Color.Red
                    );
                context.Line(MinimapPosition + playerDotPos + (playerDotSize / 2),
                    MinimapPosition + new Vector2(tileRefSize.X * rightRayY, tileRefSize.Y * rightRayX),
                    Color.Red
                    );

                foreach(Enemy enemy in EnemyList)
                {
                    Vector2 enemyDotSize = tileRefSize / 2;
                    Vector2 enemyDotPos = new Vector2(tileRefSize.X * enemy.PosY, tileRefSize.Y * enemy.PosX);
                    enemyDotPos -= enemyDotSize / 2;
                    context.Rectangle(ShapeMode.Fill, MinimapPosition + enemyDotPos, enemyDotSize.X, enemyDotSize.Y, Color.LimeGreen);
                }
            }

            DebugText = $"{PerformanceCounter.FPS} FPS\n" + DebugText;
            context.DrawString(DebugFont, DebugText, Vector2.Zero, (c, i, p, g) => new GlyphTransformData() { Color = Color.White });
        }

        private void DrawColorStrip(RenderContext context, int screenX, int drawStart, int drawEnd, int z, Color color)
        {
            if (drawEnd - drawStart <= 0) return;
            context.Batch(() => context.Line(new Vector2(screenX, drawStart), new Vector2(screenX, drawEnd), color), z);
        }

        protected override void Update(float delta)
        {

            foreach (Enemy enemy in EnemyList)
            {
                enemy.Update(delta, PlayerPos);
            }

            if (Keyboard.IsKeyDown(KeyCode.W))
            {
                MovePlayerInDir(delta, new Vector2(0, 1));
            }
            if (Keyboard.IsKeyDown(KeyCode.A))
            {
                MovePlayerInDir(delta, new Vector2(-1, 0));
            }
            if (Keyboard.IsKeyDown(KeyCode.S))
            {
                MovePlayerInDir(delta, new Vector2(0, -1));
            }
            if (Keyboard.IsKeyDown(KeyCode.D))
            {
                MovePlayerInDir(delta, new Vector2(1, 0));
            }
            if (Keyboard.IsKeyDown(KeyCode.Left))
            {
                RotatePlayerInDir(delta, -1);
            }
            if (Keyboard.IsKeyDown(KeyCode.Right))
            {
                RotatePlayerInDir(delta, 1);
            }

            float controllerMoveX = Controller.GetAxisValueNormalized(0, ControllerAxis.LeftStickX);
            float controllerMoveY = -Controller.GetAxisValueNormalized(0, ControllerAxis.LeftStickY);
            float controllerRot = Controller.GetAxisValueNormalized(0, ControllerAxis.RightStickX);
            DebugText = "F: Flat Render\n" +
                "M: Toggle Minimap\n" +
                "N: Toggle Sprites\n" +
                "V: NoClip";
            MovePlayerInDir(delta, new Vector2(controllerMoveX, controllerMoveY));
            RotatePlayerInDir(delta, controllerRot);
        }

        protected override void KeyPressed(KeyEventArgs e)
        {
            if (e.KeyCode == KeyCode.F) FlatRender = !FlatRender;
            if (e.KeyCode == KeyCode.M) MiniMap = !MiniMap;
            if (e.KeyCode == KeyCode.N) RenderSprites = !RenderSprites;
            if (e.KeyCode == KeyCode.V) NoClip = !NoClip;
        }

        protected override void ControllerButtonPressed(ControllerButtonEventArgs e)
        {
            if (e.Button == ControllerButton.Menu) FlatRender = !FlatRender;
            if (e.Button == ControllerButton.View) MiniMap = !MiniMap;
            if (e.Button == ControllerButton.A) RenderSprites = !RenderSprites;
            if (e.Button == ControllerButton.B) NoClip = !NoClip;
        }

        /// <summary>
        /// Moves the player based on a vector direction.
        /// E.G (0, 1) is up, (1, 0) is right (I think lmao)
        /// </summary>
        /// <param name="deltaTime">Delta time</param>
        /// <param name="direction">Aforementioned vector direction</param>
        private void MovePlayerInDir(float deltaTime, Vector2 direction)
        {
            // Speed of player's movement
            float moveSpeed = deltaTime * (float)PlayerMoveSpeed; // The constant value is in squares/second
            Vector2 newPlayerPos = new Vector2(PlayerPos.X, PlayerPos.Y);
            Vector2 playerForward = new Vector2((float)DirX, (float)DirY);
            Vector2 playerRight = new Vector2((float)DirY, (float)-DirX);
            Vector2 movementDir = new Vector2(direction.X) * playerRight + new Vector2(direction.Y) * playerForward;
            newPlayerPos += movementDir * moveSpeed;
            Vector2 xDir = new Vector2(
                newPlayerPos.X,
                PlayerPos.Y);
            Vector2 yDir = new Vector2(
                PlayerPos.X,
                newPlayerPos.Y);
            if (!CheckWallCollision(xDir) && !CheckSpriteCollision(xDir)) PlayerPos.X = newPlayerPos.X;
            if (!CheckWallCollision(yDir) && !CheckSpriteCollision(yDir)) PlayerPos.Y = newPlayerPos.Y;
        }

        private void RotatePlayerInDir(float deltaTime, float direction)
        {
            double rotSpeed = (deltaTime * PlayerRotSpeed) * -direction; // The constant value is in radians/second

            //both camera direction and camera plane must be rotated
            double oldDirX = DirX;
            DirX = DirX * Cos(rotSpeed) - DirY * Sin(rotSpeed);
            DirY = oldDirX * Sin(rotSpeed) + DirY * Cos(-rotSpeed);
            double oldPlaneX = PlaneX;
            PlaneX = PlaneX * Cos(rotSpeed) - PlaneY * Sin(rotSpeed);
            PlaneY = oldPlaneX * Sin(rotSpeed) + PlaneY * Cos(rotSpeed);
        }

        private bool CheckWallCollision(Vector2 mapPos)
        {
            if (NoClip) return false;
            try
            {
                if (WorldMap[(int)(mapPos.X + PlayerSize), (int)(mapPos.Y + PlayerSize)] > 0 ||
                    WorldMap[(int)(mapPos.X - PlayerSize), (int)(mapPos.Y - PlayerSize)] > 0 ||
                    WorldMap[(int)(mapPos.X + PlayerSize), (int)(mapPos.Y - PlayerSize)] > 0 ||
                    WorldMap[(int)(mapPos.X - PlayerSize), (int)(mapPos.Y + PlayerSize)] > 0)
                {
                    return true;
                }
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
            return false;
        }

        private bool CheckSpriteCollision(Vector2 mapPos)
        {
            if (NoClip) return false;
            foreach (Object sprite in SpriteList)
            {
                if (!sprite.Block) continue;
                if ((sprite.PosX == (int)(mapPos.X + PlayerSize) && sprite.PosY == (int)(mapPos.Y + PlayerSize)) ||
                    (sprite.PosX == (int)(mapPos.X - PlayerSize) && sprite.PosY == (int)(mapPos.Y - PlayerSize)) ||
                    (sprite.PosX == (int)(mapPos.X + PlayerSize) && sprite.PosY == (int)(mapPos.Y - PlayerSize)) ||
                    (sprite.PosX == (int)(mapPos.X - PlayerSize) && sprite.PosY == (int)(mapPos.Y + PlayerSize)))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
