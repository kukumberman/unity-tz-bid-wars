using Game.Core;
using Injection;
using System.Collections;
using UnityEngine;

public abstract class BidState : State { }

public sealed class BidStateCountdown : BidState
{
    [Inject]
    private BidManager _bidManager;

    [Inject]
    private EmptyMonoBehaviour _monoBehaviour;

    private readonly int _maxCountdown;

    public BidStateCountdown(int maxCountdown)
    {
        _maxCountdown = maxCountdown;
    }

    public override void Initialize()
    {
        _monoBehaviour.StartCoroutine(MyCoroutine());
    }

    public override void Dispose() { }

    private IEnumerator MyCoroutine()
    {
        for (int i = _maxCountdown; i > 0; i--)
        {
            _bidManager.DispatchEvent(new BidEventCountdown { TimeLeft = i });
            yield return new WaitForSeconds(1);
        }

        _bidManager.SwitchToState(new BidStateStart());
    }
}

public sealed class BidStateStart : BidState
{
    [Inject]
    private BidManager _bidManager;

    public override void Initialize()
    {
        _bidManager.DispatchEvent(new BidEventGameStart { Price = _bidManager.CurrentPrice });
        _bidManager.BeginNewRound();

        _bidManager.SwitchToState(new BidStateRoundWait());
    }

    public override void Dispose() { }
}

public sealed class BidStateRoundStart : BidState
{
    [Inject]
    private BidManager _bidManager;

    public override void Initialize()
    {
        _bidManager.IncreasePrice();
        _bidManager.BeginNewRound();
        _bidManager.DispatchEvent(new BidEventRoundStart { Price = _bidManager.CurrentPrice });

        _bidManager.SwitchToState(new BidStateRoundWait());
    }

    public override void Dispose() { }
}

public sealed class BidStateRoundWait : BidState
{
    private const int kMaxCounter = 2;

    [Inject]
    private BidManager _bidManager;

    [Inject]
    private Timer _timer;

    [Inject]
    private EmptyMonoBehaviour _monoBehaviour;

    private Coroutine _checkCoroutine;
    private Coroutine _randomEnemyCoroutine;

    private int _counter;

    public override void Initialize()
    {
        _bidManager.OnRoundMemberCountChanged += BidManager_OnRoundMemberCountChanged;
        _timer.TICK += Timer_TICK;

        _randomEnemyCoroutine = _monoBehaviour.StartCoroutine(AddRandomEnemyCoroutine());
        _checkCoroutine = _monoBehaviour.StartCoroutine(CounterCoroutine());
    }

    public override void Dispose()
    {
        _bidManager.OnRoundMemberCountChanged -= BidManager_OnRoundMemberCountChanged;
        _timer.TICK -= Timer_TICK;

        if (_randomEnemyCoroutine != null)
        {
            _monoBehaviour.StopCoroutine(_randomEnemyCoroutine);
            _randomEnemyCoroutine = null;
        }

        if (_checkCoroutine != null)
        {
            _monoBehaviour.StopCoroutine(_checkCoroutine);
            _checkCoroutine = null;
        }
    }

    private void Timer_TICK()
    {
        // Dima: just for debugging purposes
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _bidManager.TryAddRandomEnemyMember();
        }
    }

    private void BidManager_OnRoundMemberCountChanged()
    {
        RestartCounterCoroutine();
    }

    private void DispatchTick(int index)
    {
        _bidManager.DispatchEvent(
            new BidEventRoundTick { SequenceIndex = index, Price = _bidManager.CurrentPrice, }
        );
    }

    private void RestartCounterCoroutine()
    {
        if (_checkCoroutine != null)
        {
            _monoBehaviour.StopCoroutine(_checkCoroutine);
            _checkCoroutine = null;
        }

        _checkCoroutine = _monoBehaviour.StartCoroutine(CounterCoroutine());
    }

    private IEnumerator CounterCoroutine()
    {
        _counter = 0;

        yield return new WaitForSeconds(1);

        if (_bidManager.CurrentRound.Members.Count > 1)
        {
            _bidManager.SwitchToState(new BidStateRoundStart());

            yield break;
        }

        while (true)
        {
            if (_bidManager.CurrentRound.Members.Count > 0)
            {
                DispatchTick(_counter);
            }

            yield return new WaitForSeconds(1);

            _counter += 1;

            if (_counter == kMaxCounter)
            {
                _bidManager.SwitchToState(new BidStateEnd());
                break;
            }
        }
    }

    private IEnumerator AddRandomEnemyCoroutine()
    {
        yield return new WaitForSeconds(2);

        if (_bidManager.Rounds.Count < 5)
        {
            _bidManager.TryAddRandomEnemyMember();
        }
    }
}

public sealed class BidStateEnd : BidState
{
    [Inject]
    private BidManager _bidManager;

    public override void Initialize()
    {
        if (_bidManager.Rounds.Count == 1 && _bidManager.Rounds[0].Members.Count == 0)
        {
            _bidManager.DispatchEvent(new BidEventJunkNotSold());
        }
        else
        {
            var round = GetRound();

            // Dima: what if bot and player voted in same round, but in next round both of them ignored to vote?
            if (round.Members.Count > 1)
            {
                Debug.Log("todo?");
            }

            _bidManager.DispatchEvent(
                new BidEventSold { Price = round.Price, WinnerId = round.Members[0].Id }
            );
        }
    }

    public override void Dispose() { }

    private BidRound GetRound()
    {
        var r1 = _bidManager.Rounds[_bidManager.Rounds.Count - 1];

        if (r1.Members.Count > 0)
        {
            return r1;
        }

        // Dima: this case is related to todo mentioned above
        return _bidManager.Rounds[_bidManager.Rounds.Count - 2];
    }
}
