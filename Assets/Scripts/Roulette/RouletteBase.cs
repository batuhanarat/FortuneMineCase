using System;
using System.Collections;
using UnityEngine;

public abstract class RouletteBase : MonoBehaviour
{
    [SerializeField] protected RouletteConfig config;
    public RewardEntry[] RewardsEntryArray;

    public event Action<RewardType> OnRewardSelected;
    public event Action OnSpinStarted;
    public event Action OnSpinCompleted;
    public event Action OnAllRewardsCollected;


    protected bool isSpinning = false;
    protected int selectedIndex = -1;
    protected Coroutine spinCoroutine;

    public bool IsSpinning => isSpinning;
    public int SelectedIndex => selectedIndex;

    public virtual bool AreAllRewardsCollected => false;

    protected abstract IEnumerator SpinCoroutine();
    public abstract void HighlightReward(int index, bool highlight);
    public abstract void MarkRewardCollected(int index);
    public abstract void ResetRoulette();

    public virtual void Initialize(RouletteConfig newConfig = null)
    {
        if (newConfig != null)
        {
            config = newConfig;
        }

        if (config == null)
        {
            Debug.LogError("RouletteConfig is missing! Assign a configuration.");
            return;
        }

        SetupRoulette();
    }

    protected virtual void SetupRoulette()
    {

    }

    public virtual void Spin()
    {
        if (isSpinning) return;

        isSpinning = true;
        OnSpinStarted?.Invoke();

        spinCoroutine = StartCoroutine(SpinCoroutine());
    }

    public virtual void Stop()
    {
        if (!isSpinning) return;

        if (spinCoroutine != null)
        {
            StopCoroutine(spinCoroutine);
            spinCoroutine = null;
        }

        isSpinning = false;
    }

    protected virtual void NotifyRewardSelected(RewardType type)
    {

        OnRewardSelected?.Invoke(type);
    }

    protected virtual void NotifySpinCompleted()
    {
        OnSpinCompleted?.Invoke();
    }

    protected virtual void NotifyAllRewardsCollected()
    {
        OnAllRewardsCollected?.Invoke();
        ServiceProvider.ScenesManager.LoadInitialScene();
    }

}