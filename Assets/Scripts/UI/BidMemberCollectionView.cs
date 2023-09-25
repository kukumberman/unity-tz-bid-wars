using Core;
using System.Collections.Generic;
using UnityEngine;

namespace Project.UI
{
    public sealed class BidMemberCollectionModel : Observable
    {
        public BidMemberItemModel[] ItemModels;
    }

    public sealed class BidMemberCollectionView : BehaviourWithModel<BidMemberCollectionModel>
    {
        [SerializeField]
        private BidMemberItemView _itemPrefab;

        [SerializeField]
        private Transform _contentParent;

        private List<BidMemberItemView> _items;

        protected override void OnApplyModel(BidMemberCollectionModel model)
        {
            if (model == null)
            {
                return;
            }

            _contentParent.DestroyChildrens();

            _items = new List<BidMemberItemView>();

            for (int i = 0; i < model.ItemModels.Length; i++)
            {
                var item = Instantiate(_itemPrefab, _contentParent);
                item.Model = model.ItemModels[i];
                _items.Add(item);
            }
        }

        protected override void OnModelChanged(BidMemberCollectionModel model) { }

        public void ShowMemberPopup(string id, string text)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];

                if (item.Id == id)
                {
                    item.ShowPopupAndAutohide(text);
                }
            }
        }
    }
}
