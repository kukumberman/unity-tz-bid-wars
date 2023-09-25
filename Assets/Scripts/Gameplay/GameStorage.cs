using System.Collections.Generic;
using Project.SO;
using UnityEngine;

public sealed class GameStorage : MonoBehaviour
{
    public BidMemberScriptableObject LocalMember;
    public List<BidMemberScriptableObject> Members;
    public List<BidItemScriptableObject> Items;
}
