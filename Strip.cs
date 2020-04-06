using Chroma;
using Chroma.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chromastein
{
    class Strip
    {

        public Texture Texture;
        public Color Color;
        public Vector2 Position;
        public Vector2 Scale;
        public Rectangle SourceRectangle;
        public float ZIndex;

        public Strip(Texture stripTex, Color stripColor, Vector2 pos, Vector2 scale, Rectangle sourceRect, float z)
        {
            Texture = stripTex;
            Color = stripColor;
            Position = pos;
            Scale = scale;
            SourceRectangle = sourceRect;
            ZIndex = z;
        }

    }
}
