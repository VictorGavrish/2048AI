namespace AI2048
{
    using System;
    using System.Drawing.Imaging;

    using AI2048.AI;

    using NUnit.Framework;

    [TestFixture]
    public class Run
    {
        [Test]
        public void RunSimulation()
        {
            var agent = new OptiminiOptimaxAgent(Heuristic.AllRotatiions(Heuristic.CornerVave));

            for (var i = 0; i < 10; i++)
            {
                using (var game = new GamePage())
                {
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