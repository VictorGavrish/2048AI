namespace AI2048.Game
{
    public class GridCell
    {
        public GridCell(int value, int x, int y)
        {
            this.X = x;
            this.Y = y;
            this.Value = value;
        }

        public int X { get; }

        public int Y { get; }

        public int Value { get; }
    }
}