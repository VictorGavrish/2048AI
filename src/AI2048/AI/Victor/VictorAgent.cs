namespace AI2048.AI.Victor
{
    using System.Collections.Generic;
    using System.Linq;

    using AI2048.Game;

    public class VictorAgent : Agent
    {
        private PlayerNode rootPlayerNode;

        public List<Grid> History { get; } = new List<Grid>();

        public override Move MakeDecision(Grid state)
        {
            this.History.Add(state);

            this.rootPlayerNode = this.rootPlayerNode?.Children.SelectMany(kvp => kvp.Value.Children).FirstOrDefault(n => n.State == state) 
                ?? new PlayerNode(state);
            this.rootPlayerNode.MakeRoot();

            if (this.rootPlayerNode.GameOver)
            {
                throw new GameOverException();
            }

            var strategy = new CombinedStrategy(this.rootPlayerNode);
            return strategy.MakeDecision();
        }
    }
}