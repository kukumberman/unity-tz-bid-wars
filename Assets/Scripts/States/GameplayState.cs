using Game.Managers;
using Game.States;
using Injection;
using Project.UI;

namespace Project.States
{
    public sealed class GameplayState : GameState
    {
        [Inject]
        private HudManager _hudManager;

        [Inject]
        private Context _context;

        private BidManager _bidManager;

        public override void Initialize()
        {
            _bidManager = new BidManager();

            _context.Install(_bidManager);
            _context.ApplyInstall();

            _bidManager.Create();

            _hudManager.ShowAdditional<GameplayHudMediator>();

            _bidManager.Start();
        }

        public override void Dispose()
        {
            _hudManager.HideAdditional<GameplayHudMediator>();
        }
    }
}
