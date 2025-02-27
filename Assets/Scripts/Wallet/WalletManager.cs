using UnityEngine;

public class WalletManager : MonoBehaviour, IProvidable
{
    [SerializeField] private WalletData walletData;

    void Awake()
    {
        ServiceProvider.Register(this);
    }

    private void OnApplicationQuit()
    {
        walletData.Reset();
    }

    public void AddReward(RewardConfig config)
    {

        walletData.Add(config.rewardType, config.amount);
    }

}