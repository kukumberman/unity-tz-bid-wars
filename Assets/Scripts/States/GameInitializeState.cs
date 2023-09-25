using Game.States;
using Injection;

namespace Project.States
{
    public sealed class GameInitializeState : GameState
    {
        [Inject]
        private GameStateManager _gameStateManager;

        public override void Initialize()
        {
            var nextState = GetNextState();
            _gameStateManager.SwitchToState(nextState);
        }

        public override void Dispose()
        {
            //
        }

        private GameState GetNextState()
        {
            return new GameplayState();
        }
    }
}
