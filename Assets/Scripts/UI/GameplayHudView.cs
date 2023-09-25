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
        public event Action OnRestartButtonClicked;

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

        [SerializeField]
        private Button _btnRestart;

        protected override void OnEnable()
        {
            _btnBid.onClick.AddListener(ButtonBidClickHandler);
            _btnPass.onClick.AddListener(ButtonPassClickHandler);
            _btnRestart.onClick.AddListener(ButtonRestartClickHandler);
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

        public void ShowButtons(bool active)
        {
            _btnBid.gameObject.SetActive(active);
            _btnPass.gameObject.SetActive(active);
        }

        public void ShowRestart(bool active)
        {
            _btnRestart.gameObject.SetActive(active);
        }

        public void ShowMemberPopup(string id, string text)
        {
            _memberCollection.ShowMemberPopup(id, text);
        }

        private void ButtonBidClickHandler()
        {
            OnBidButtonClicked.SafeInvoke();
        }

        private void ButtonPassClickHandler()
        {
            OnPassButtonClicked.SafeInvoke();
        }

        private void ButtonRestartClickHandler()
        {
            OnRestartButtonClicked.SafeInvoke();
        }
    }

    public sealed class GameplayHudMediator : Mediator<GameplayHudView>
    {
        public event Action OnRestart;

        private const string kTemplateStartPrice = "Starting at {0}."; // price
        private const string kTemplateNextPrice = "Anyone for {0}?"; // price
        private const string kTemplateWait = "At {0} going {1}..."; // price, sequence
        private const string kTemplateSold = "Sold for {0}!\n({1})"; // price, name

        private static readonly string[] kSequenceArray = new string[] { "once", "twice", };

        [Inject]
        private IBidManager _bidManager;

        private GameplayHudModel _viewModel;

        protected override void Show()
        {
            _view.OnBidButtonClicked += View_OnBidButtonClicked;
            _view.OnRestartButtonClicked += View_OnRestartButtonClicked;

            _bidManager.OnEvent += BidManager_OnEvent;

            _viewModel = new GameplayHudModel
            {
                ItemSprite = _bidManager.SelectedItem.Sprite,
                MemberCollectionModel = new BidMemberCollectionModel
                {
                    ItemModels = new BidMemberItemModel[_bidManager.Members.Count],
                },
            };

            PopulateMembers();

            _view.ShowButtons(false);
            _view.ShowRestart(false);

            _view.Model = _viewModel;
        }

        protected override void Hide()
        {
            _view.OnBidButtonClicked -= View_OnBidButtonClicked;
            _view.OnRestartButtonClicked -= View_OnRestartButtonClicked;

            _bidManager.OnEvent -= BidManager_OnEvent;
        }

        private void PopulateMembers()
        {
            for (int i = 0; i < _viewModel.MemberCollectionModel.ItemModels.Length; i++)
            {
                var member = _bidManager.Members[i];
                var memberModel = new BidMemberItemModel
                {
                    Id = member.Id,
                    DisplayName = member.DisplayName,
                    ProfileSprite = member.ProfileSprite,
                };

                _viewModel.MemberCollectionModel.ItemModels[i] = memberModel;
            }
        }

        private void View_OnBidButtonClicked()
        {
            _bidManager.ProcessLocalPlayerBid();
        }

        private void View_OnRestartButtonClicked()
        {
            OnRestart.SafeInvoke();
        }

        private void BidManager_OnEvent(BidEvent evt)
        {
            if (evt is BidEventCountdown countdownEvent)
            {
                var text = $"{countdownEvent.TimeLeft}";
                _view.ShowPopup(text);
            }
            else if (evt is BidEventGameStart startEvent)
            {
                var price = _bidManager.FormatPrice(startEvent.Price);
                var text = string.Format(kTemplateStartPrice, price);
                _view.ShowPopup(text);
                _view.ShowButtons(true);
            }
            else if (evt is BidEventRoundStart roundEvent)
            {
                var price = _bidManager.FormatPrice(roundEvent.Price);
                var text = string.Format(kTemplateNextPrice, price);
                _view.ShowPopup(text);
                _view.ShowButtons(true);
            }
            else if (evt is BidEventMemberVote voteEvent)
            {
                var price = _bidManager.FormatPrice(voteEvent.Price);
                _view.ShowMemberPopup(voteEvent.MemberId, price);

                if (voteEvent.MemberId == _bidManager.LocalPlayerId)
                {
                    _view.ShowButtons(false);
                }
            }
            else if (evt is BidEventRoundTick waitEvent)
            {
                var price = _bidManager.FormatPrice(waitEvent.Price);
                var text = string.Format(
                    kTemplateWait,
                    price,
                    kSequenceArray[waitEvent.SequenceIndex]
                );
                _view.ShowPopup(text);
            }
            else if (evt is BidEventSold soldEvent)
            {
                var price = _bidManager.FormatPrice(soldEvent.Price);
                var winner = _bidManager.Members.Find(member => member.Id == soldEvent.WinnerId);
                var text = string.Format(kTemplateSold, price, winner.DisplayName);
                _view.ShowPopup(text);
                _view.ShowButtons(false);
                _view.ShowRestart(true);
            }
            else if (evt is BidEventJunkNotSold junkEvent)
            {
                _view.ShowPopup("Haha this is a junk item and nobody wants to buy it!");
                _view.ShowButtons(false);
                _view.ShowRestart(true);
            }
        }
    }
}
