using System;
using System.Collections.Generic;
using Game.Utilities;
using Injection;
using Project.SO;
using Random = System.Random;

public sealed class BidManager
{
    private const string kCurrencySymbol = "$";
    private const int kMaxEnemyMembers = 3;
    private const int kItemStartPrice = 100;
    private const int kItemStepPrice = 25;

    public event Action OnNextPriceChanged;

    [Inject]
    private GameStorage _gameStorage;

    private List<BidMemberScriptableObject> _members;
    private BidItemScriptableObject _selectedItem;

    private readonly int _seed;

    private int _currentPrice;

    public List<BidMemberScriptableObject> Members => _members;
    public BidItemScriptableObject SelectedItem => _selectedItem;
    public int CurrentPrice => _currentPrice;

    public BidManager()
    {
        _seed = DateTime.Now.Second;
    }

    public void Create()
    {
        CreateMembers();
        CreateItem();
    }

    public void Start()
    {
        _currentPrice = kItemStartPrice;

        OnNextPriceChanged.SafeInvoke();
    }

    public string FormatPrice(int amount)
    {
        return string.Format("{0} {1}", kCurrencySymbol, amount);
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

        _members.Add(_gameStorage.LocalMember);
    }

    private void CreateItem()
    {
        var index = new Random(_seed).Next(_gameStorage.Items.Count);
        _selectedItem = _gameStorage.Items[index];
    }
}
