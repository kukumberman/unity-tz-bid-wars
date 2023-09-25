using System;
using System.Collections.Generic;
using System.Linq;
using Game.Core;
using Game.Utilities;
using Injection;
using Project.SO;
using Random = System.Random;

public sealed class BidRound
{
    public int Price;
    public List<BidMemberScriptableObject> Members;
}

public sealed class BidManager : IBidManager
{
    private const string kCurrencySymbol = "$";
    private const int kMaxEnemyMembers = 3;
    private const int kItemStartPrice = 100;
    private const int kItemStepPrice = 25;

    public event Action<BidEvent> OnEvent;
    public event Action OnRoundMemberCountChanged;

    [Inject]
    private GameStorage _gameStorage;

    [Inject]
    private Context _context;

    private List<BidMemberScriptableObject> _enemyMembers;
    private List<BidMemberScriptableObject> _members;
    private BidItemScriptableObject _selectedItem;

    private readonly int _seed;
    private Random _bidRandom;

    private int _currentPrice;
    private List<BidRound> _bidRounds;
    private StateManager<BidState> _stateManager;

    public int CurrentPrice => _currentPrice;
    public BidRound CurrentRound => _bidRounds[_bidRounds.Count - 1];
    public List<BidRound> Rounds => _bidRounds;

    public List<BidMemberScriptableObject> Members => _members;
    public BidItemScriptableObject SelectedItem => _selectedItem;
    public string LocalPlayerId => _gameStorage.LocalMember.Id;

    public BidManager()
    {
        _seed = DateTime.Now.Second;
    }

    public void Create()
    {
        CreateMembers();
        CreateItem();

        SetupStateMachine();
    }

    public void Start()
    {
        _currentPrice = kItemStartPrice;
        _bidRounds = new List<BidRound>();
        _bidRandom = new Random(_seed);

        SwitchToState(new BidStateCountdown(3));
    }

    public string FormatPrice(int amount)
    {
        return string.Format("{0} {1}", kCurrencySymbol, amount);
    }

    public void ProcessLocalPlayerBid()
    {
        AddMemberToRound(_gameStorage.LocalMember);
    }

    public void SwitchToState(BidState state)
    {
        _stateManager.SwitchToState(state);
    }

    public void DispatchEvent(BidEvent evt)
    {
        OnEvent.SafeInvoke(evt);
    }

    public void IncreasePrice()
    {
        _currentPrice += kItemStepPrice;
    }

    public void BeginNewRound()
    {
        _bidRounds.Add(
            new BidRound { Price = _currentPrice, Members = new List<BidMemberScriptableObject>() }
        );
    }

    public void TryAddRandomEnemyMember()
    {
        if (TrySelectMember(out var member))
        {
            AddMemberToRound(member);
        }
    }

    private void SetupStateMachine()
    {
        var subContext = new Context(_context);
        var injector = new Injector(subContext);
        _stateManager = new StateManager<BidState>();
        _stateManager.IsEnableLog = false;

        subContext.InstallByType(this, GetType());
        subContext.Install(injector);
        injector.Inject(_stateManager);
    }

    private void CreateMembers()
    {
        var enemyMembers = new List<BidMemberScriptableObject>(_gameStorage.Members);
        enemyMembers.Shuffle(new Random(_seed));

        _members = new List<BidMemberScriptableObject>();

        var length = Math.Min(kMaxEnemyMembers, enemyMembers.Count);

        for (int i = 0; i < length; i++)
        {
            _members.Add(enemyMembers[i]);
        }

        _enemyMembers = new List<BidMemberScriptableObject>(_members);

        _members.Add(_gameStorage.LocalMember);
    }

    private void CreateItem()
    {
        var index = new Random(_seed).Next(_gameStorage.Items.Count);
        _selectedItem = _gameStorage.Items[index];
    }

    private bool TrySelectMember(out BidMemberScriptableObject member)
    {
        member = null;

        var uniqueMembers = _enemyMembers.Except(CurrentRound.Members).ToList(); // new List<BidMemberScriptableObject>(_enemyMembers);

        if (uniqueMembers.Count == 0)
        {
            return false;
        }

        uniqueMembers.Shuffle(_bidRandom);
        member = uniqueMembers[0];

        return true;
    }

    private void AddMemberToRound(BidMemberScriptableObject member)
    {
        var round = CurrentRound;

        if (!round.Members.Contains(member))
        {
            round.Members.Add(member);

            OnEvent.SafeInvoke(
                new BidEventMemberVote { MemberId = member.Id, Price = _currentPrice }
            );

            OnRoundMemberCountChanged.SafeInvoke();
        }
    }
}
