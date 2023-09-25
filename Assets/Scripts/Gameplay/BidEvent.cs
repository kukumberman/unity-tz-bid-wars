public abstract class BidEvent { }

public sealed class BidEventCountdown : BidEvent
{
    public int TimeLeft;
}

public sealed class BidEventGameStart : BidEvent
{
    public int Price;
}

public sealed class BidEventRoundStart : BidEvent
{
    public int Price;
}

public sealed class BidEventMemberVote : BidEvent
{
    public string MemberId;
    public int Price;
}

public sealed class BidEventRoundTick : BidEvent
{
    public int SequenceIndex;
    public int Price;
}

public sealed class BidEventSold : BidEvent
{
    public string WinnerId;
    public string ItemId;
    public int Price;
}

public sealed class BidEventJunkNotSold : BidEvent { }
