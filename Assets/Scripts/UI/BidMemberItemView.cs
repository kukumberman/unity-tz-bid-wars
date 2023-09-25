using Core;
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
        public bool IsPopupActive;
        public string PopupText;
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

        protected override void OnModelChanged(BidMemberItemModel model)
        {
            _imgProfile.sprite = model.ProfileSprite;
            _txtName.text = model.DisplayName;
            _contentPopup.SetActive(model.IsPopupActive);
            _txtPopup.text = model.PopupText;
        }
    }
}
