namespace AI2048.Game
{
    public static class LogarithmicGridExtensions
    {
        public static LogarithmicGrid RotateCw(this LogarithmicGrid grid)
        {
            var newGrid = new byte[4, 4];

            for (var x = 0; x < 4; x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    newGrid[3 - y, x] = grid[x, y];
                }
            }

            return new LogarithmicGrid(newGrid);
        }
    }
}