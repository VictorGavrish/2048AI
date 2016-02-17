namespace AI2048.Tests
{
    using AI2048.Game;

    using NUnit.Framework;

    [TestFixture]
    public class LogarithmicGridTests
    {
        [Test]
        public static void TestMoves()
        {
            // Arrange
            var grid = new LogarithmicGrid(new byte[,]
            {
                { 1, 1, 2, 2 },
                { 0, 1, 1, 0 },
                { 0, 1, 1, 1 },
                { 1, 0, 0, 1 }
            });

            var expectedLeft = new LogarithmicGrid(new byte[,]
            {
                { 2, 3, 0, 0 },
                { 2, 0, 0, 0 },
                { 2, 1, 0, 0 },
                { 2, 0, 0, 0 }
            });

            var expectedRight = new LogarithmicGrid(new byte[,]
            {
                { 0, 0, 2, 3 },
                { 0, 0, 0, 2 },
                { 0, 0, 1, 2 },
                { 0, 0, 0, 2 }
            });

            var expectedUp = new LogarithmicGrid(new byte[,]
            {
                { 2, 2, 2, 2 },
                { 0, 1, 2, 2 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            });

            var expectedDown = new LogarithmicGrid(new byte[,]
            {
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 0, 1, 2, 2 },
                { 2, 2, 2, 2 }
            });

            // Act
            var left = grid.MakeMove(Move.Left);
            var right = grid.MakeMove(Move.Right);
            var up = grid.MakeMove(Move.Up);
            var down = grid.MakeMove(Move.Down);

            // Assert
            Assert.That(left.Equals(expectedLeft));
            Assert.That(right.Equals(expectedRight));
            Assert.That(up.Equals(expectedUp));
            Assert.That(down.Equals(expectedDown));
        }
    }
}
