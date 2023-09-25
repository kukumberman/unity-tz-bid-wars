using System;
using Core;
using Game.Core.UI;
using Game.UI.Hud;
using Injection;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;

namespace Project.UI
{
    public sealed class GameplayHudModel : Observable
    {
        public Sprite ItemSprite;
        public BidMemberCollectionModel MemberCollectionModel;
    }

    public sealed class GameplayHudView : BaseHudWithModel<GameplayHudModel>
    {
        public event Action OnBidButtonClicked;
        public event Action OnPassButtonClicked;

        [SerializeField]
        private GameObject _contentPopup;

        [SerializeField]
        private Text _txtPopup;

        [SerializeField]
        private Image _imgItem;

        [SerializeField]
        private BidMemberCollectionView _memberCollection;

        [SerializeField]
        private Button _btnBid;

        [SerializeField]
        private Button _btnPass;

        protected override void OnEnable()
        {
            _btnBid.onClick.AddListener(ButtonBidClickHandler);
            _btnPass.onClick.AddListener(ButtonPassClickHandler);
        }

        protected override void OnDisable()
        {
            _btnBid.onClick.RemoveListener(ButtonBidClickHandler);
            _btnPass.onClick.RemoveListener(ButtonPassClickHandler);
        }

        protected override void OnApplyModel(GameplayHudModel model)
        {
            _memberCollection.Model = model.MemberCollectionModel;
        }

        protected override void OnModelChanged(GameplayHudModel model)
        {
            _imgItem.sprite = model.ItemSprite;
        }

        public void ShowPopup(string text)
        {
            _contentPopup.SetActive(true);
            _txtPopup.text = text;
        }

        public void HidePopup()
        {
            _contentPopup.SetActive(false);
        }

        private void ButtonBidClickHandler()
        {
            OnBidButtonClicked.SafeInvoke();
        }

        private void ButtonPassClickHandler()
        {
            OnPassButtonClicked.SafeInvoke();
        }
    }

    public sealed class GameplayHudMediator : Mediator<GameplayHudView>
    {
        private const string kTemplateNextPrice = "Anyone for {0}?";

        [Inject]
        private BidManager _bidManager;

        private GameplayHudModel _viewModel;

        protected override void Show()
        {
            _bidManager.OnNextPriceChanged += BidManager_OnNextPriceChanged;

            _viewModel = new GameplayHudModel
            {
                ItemSprite = _bidManager.SelectedItem.Sprite,
                MemberCollectionModel = new BidMemberCollectionModel
                {
                    ItemModels = new BidMemberItemModel[_bidManager.Members.Count],
                },
            };

            for (int i = 0; i < _viewModel.MemberCollectionModel.ItemModels.Length; i++)
            {
                var member = _bidManager.Members[i];
                var memberModel = new BidMemberItemModel
                {
                    Id = member.Id,
                    DisplayName = member.DisplayName,
                    ProfileSprite = member.ProfileSprite,
                    IsPopupActive = false,
                    PopupText = "",
                };

                _viewModel.MemberCollectionModel.ItemModels[i] = memberModel;
            }

            _view.Model = _viewModel;
        }

        protected override void Hide()
        {
            _bidManager.OnNextPriceChanged -= BidManager_OnNextPriceChanged;
        }

        private void BidManager_OnNextPriceChanged()
        {
            var price = _bidManager.FormatPrice(_bidManager.CurrentPrice);
            var text = string.Format(kTemplateNextPrice, price);
            _view.ShowPopup(text);
        }
    }
}
