using Core;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;

namespace Project.UI
{
    public sealed class BidMemberItemModel : Observable
    {
        public string Id;
        public string DisplayName;
        public Sprite ProfileSprite;
    }

    public sealed class BidMemberItemView : BehaviourWithModel<BidMemberItemModel>
    {
        [SerializeField]
        private Image _imgProfile;

        [SerializeField]
        private Text _txtName;

        [SerializeField]
        private GameObject _contentPopup;

        [SerializeField]
        private Text _txtPopup;

        protected override void OnApplyModel(BidMemberItemModel model)
        {
            _contentPopup.SetActive(false);
        }

        protected override void OnModelChanged(BidMemberItemModel model)
        {
            _imgProfile.sprite = model.ProfileSprite;
            _txtName.text = model.DisplayName;
        }

        public string Id => Model.Id;

        public void ShowPopup(string text)
        {
            _contentPopup.SetActive(true);
            _txtPopup.text = text;
        }

        public void HidePopup()
        {
            _contentPopup.SetActive(false);
        }

        public void ShowPopupAndAutohide(string text)
        {
            ShowPopup(text);

            Invoke(nameof(HidePopup), 1);
        }
    }
}
