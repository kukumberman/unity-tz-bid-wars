using UnityEngine;

namespace Project.SO
{
    [CreateAssetMenu(fileName = "New Bid Item (ScriptableObject)", menuName = "SO/Bid Item")]
    public sealed class BidItemScriptableObject : ScriptableObject
    {
        public Sprite Sprite;
    }
}
