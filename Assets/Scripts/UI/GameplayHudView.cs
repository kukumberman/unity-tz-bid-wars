using Core;
using Game.Core.UI;
using Game.UI.Hud;

namespace Project.UI
{
    public sealed class GameplayHudModel : Observable
    {
        //
    }

    public sealed class GameplayHudView : BaseHudWithModel<GameplayHudModel>
    {
        protected override void OnEnable()
        {
            //
        }

        protected override void OnDisable()
        {
            //
        }

        protected override void OnModelChanged(GameplayHudModel model)
        {
            //
        }
    }

    public sealed class GameplayHudMediator : Mediator<GameplayHudView>
    {
        protected override void Show()
        {
            //
        }

        protected override void Hide()
        {
            //
        }
    }
}
