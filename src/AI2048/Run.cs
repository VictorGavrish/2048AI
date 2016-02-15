namespace AI2048
{
    using System;
    using System.Drawing.Imaging;

    using AI2048.AI;
    using AI2048.AI.Victor;

    using NUnit.Framework;

    [TestFixture]
    public class Run
    {
        [Test]
        public void RunSimulation()
        {
            for (var i = 0; i < 10; i++)
            {
                using (var game = new GamePage())
                {
                    var agent = new VictorAgent();

                    while (game.CanMove)
                    {
                        var move = agent.MakeDecision(game.GridState);
                        game.Turn(move);
                    }

                    game.TakeScreenshot().SaveAsFile("game_" + game.Score + ".png", ImageFormat.Png);
                    Console.WriteLine(game.Score);
                }
            }
        }
    }
}