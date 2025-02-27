using UnityEngine;

public class WalletManager : MonoBehaviour, IProvidable
{
    [SerializeField] private WalletData walletData;

    void Awake()
    {
        ServiceProvider.Register(this);
    }

    public void AddReward(RewardConfig config)
    {

        walletData.Add(config.rewardType, config.amount);
    }

}