using UnityEngine;

namespace Project.SO
{
    [CreateAssetMenu(fileName = "New Bid Member (ScriptableObject)", menuName = "SO/Bid Member")]
    public sealed class BidMemberScriptableObject : ScriptableObject
    {
        public string Id;
        public string DisplayName;
        public Sprite ProfileSprite;
    }
}
