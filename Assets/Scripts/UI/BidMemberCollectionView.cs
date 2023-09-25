using Core;
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

        protected override void OnApplyModel(BidMemberCollectionModel model)
        {
            if (model == null)
            {
                return;
            }

            _contentParent.DestroyChildrens();

            for (int i = 0; i < model.ItemModels.Length; i++)
            {
                var item = Instantiate(_itemPrefab, _contentParent);
                item.Model = model.ItemModels[i];
            }
        }

        protected override void OnModelChanged(BidMemberCollectionModel model) { }
    }
}
