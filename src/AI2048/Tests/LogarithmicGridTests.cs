namespace AI2048.Tests
{
    using AI2048.Game;

    using Xunit;

    public class LogarithmicGridTests
    {
        [Fact]
        public static void TestMoves()
        {
            // Arrange
            var grid = new LogarithmicGrid(new[,]
            {
                { 2, 2, 4, 4 },
                { 0, 2, 2, 0 },
                { 0, 2, 2, 2 },
                { 2, 0, 0, 2 }
            });

            var expectedLeft = new LogarithmicGrid(new[,]
            {
                { 4, 8, 0, 0 },
                { 4, 0, 0, 0 },
                { 4, 2, 0, 0 },
                { 4, 0, 0, 0 }
            });

            var expectedRight = new LogarithmicGrid(new[,]
            {
                { 0, 0, 4, 8 },
                { 0, 0, 0, 4 },
                { 0, 0, 2, 4 },
                { 0, 0, 0, 4 }
            });

            var expectedUp = new LogarithmicGrid(new[,]
            {
                { 4, 4, 4, 4 },
                { 0, 2, 4, 4 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            });

            var expectedDown = new LogarithmicGrid(new[,]
            {
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 0, 2, 4, 4 },
                { 4, 4, 4, 4 }
            });

            // Act
            var left = grid.MakeMove(Move.Left);
            var right = grid.MakeMove(Move.Right);
            var up = grid.MakeMove(Move.Up);
            var down = grid.MakeMove(Move.Down);

            // Assert
            Assert.StrictEqual(left, expectedLeft);
            Assert.StrictEqual(right, expectedRight);
            Assert.StrictEqual(up, expectedUp);
            Assert.StrictEqual(down, expectedDown);
        }
    }
}
