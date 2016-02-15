namespace AI2048.AI.Victor
{
    using System.Collections.Generic;
    using System.Linq;

    using AI2048.Game;

    public class VictorAgent : Agent
    {
        private Node rootNode;

        public List<Grid> History { get; } = new List<Grid>();

        public override Move MakeDecision(Grid state)
        {
            this.History.Add(state);

            this.rootNode = this.rootNode?.ChildNodes.FirstOrDefault(n => n.State == state) ?? new Node(state);
            this.rootNode.MakeRoot();

            if (this.rootNode.GameOver)
            {
                throw new GameOverException();
            }

            var strategy = new CombinedStrategy(this.rootNode);
            return strategy.MakeDecision();
        }
    }
}