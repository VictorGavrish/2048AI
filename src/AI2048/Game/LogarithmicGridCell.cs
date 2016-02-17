namespace AI2048.Game
{
    public struct LogarithmicGridCell
    {
        public LogarithmicGridCell(byte value, int x, int y)
        {
            this.X = x;
            this.Y = y;
            this.Value = value;
        }

        public int X { get; }

        public int Y { get; }

        public byte Value { get; }
    }
}