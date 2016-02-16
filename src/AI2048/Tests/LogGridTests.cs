namespace AI2048.Tests
{
    using AI2048.Game;

    using NUnit.Framework;

    [TestFixture]
    public class LogGridTests
    {
        [Test]
        public static void Test()
        {
            var grid = new LogGrid(new byte[,]
            {
                { 1, 1, 2, 2 },
                { 0, 1, 1, 0 },
                { 0, 1, 1, 1 },
                { 1, 0, 0, 1 }
            });

            var expectedLeft = new LogGrid(new byte[,]
            {
                { 2, 3, 0, 0 },
                { 2, 0, 0, 0 },
                { 2, 1, 0, 0 },
                { 2, 0, 0, 0 }
            });

            var expectedRight = new LogGrid(new byte[,]
            {
                { 0, 0, 2, 3 },
                { 0, 0, 0, 2 },
                { 0, 0, 1, 2 },
                { 0, 0, 0, 2 }
            });

            var expectedUp = new LogGrid(new byte[,]
            {
                { 2, 2, 2, 2 },
                { 0, 1, 2, 2 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            });

            var expectedDown = new LogGrid(new byte[,]
            {
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 0, 1, 2, 2 },
                { 2, 2, 2, 2 }
            });

            var left = grid.MakeMove(Move.Left);

            var right = grid.MakeMove(Move.Right);

            var up = grid.MakeMove(Move.Up);

            var down = grid.MakeMove(Move.Down);

            Assert.That(left.Equals(expectedLeft));
            Assert.That(right.Equals(expectedRight));
            Assert.That(up.Equals(expectedUp));
            Assert.That(down.Equals(expectedDown));
        }
    }
}
