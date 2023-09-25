using System;
using System.Collections.Generic;
using Project.SO;

public interface IBidManager
{
    event Action<BidEvent> OnEvent;
    void Create();
    void Start();
    string FormatPrice(int price);
    void ProcessLocalPlayerBid();
    string LocalPlayerId { get; }
    BidItemScriptableObject SelectedItem { get; }
    List<BidMemberScriptableObject> Members { get; }
}
