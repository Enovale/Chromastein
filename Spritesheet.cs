using Chroma.Graphics;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Color = Chroma.Graphics.Color;

namespace Chromastein
{
    public class Spritesheet
    {
        private int _cellWidth;
        private int _cellHeight;

        private List<Rectangle> Cells { get; }

        internal Texture Texture { get; }

        public int CellWidth
        {
            get => _cellWidth;
            set
            {
                _cellWidth = value;
                GenerateSourceRectangles();
            }
        }

        public int CellHeight
        {
            get => _cellHeight;
            set
            {
                _cellHeight = value;
                GenerateSourceRectangles();
            }
        }

        public int CellCount => Cells.Count;

        public Spritesheet(string filePath)
        {
            Cells = new List<Rectangle>();
            Texture = new Texture(filePath);

            CellWidth = 0;
            CellHeight = 0;
        }

        public Vector2 GetGranularXY(int cellIndex)
            => new Vector2(
                Cells[cellIndex].Left / _cellWidth,
                Cells[cellIndex].Top / _cellHeight
            );

       internal void Draw(RenderContext context, int cellIndex, Vector2 position, Vector2 scale, Vector2 origin, int rotation)
        {
            context.DrawTexture(Texture, position, scale, origin, rotation, Cells[cellIndex]);
        }

        private void GenerateSourceRectangles()
        {
            if (_cellWidth == 0 || _cellHeight == 0)
                return;

            Cells.Clear();

            var totalCellsX = Texture.Width / _cellWidth;
            var totalCellsY = Texture.Height / _cellHeight;

            for (int y = 0; y < totalCellsY; y++)
            {
                for (int x = 0; x < totalCellsX; x++)
                {
                    Cells.Add(
                        new Rectangle(
                            x * CellWidth,
                            y * CellHeight,
                            CellWidth,
                            CellHeight
                        )
                    );
                }
            }
        }
    }
}