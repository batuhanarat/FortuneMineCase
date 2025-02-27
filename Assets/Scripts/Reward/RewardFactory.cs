
using UnityEngine;

public class RewardFactory : IProvidable
{
    /*
        I did not used this factory pattern in this project, but if i implement the animation for rewards,
        i will create the prefabs from factory
        and probably i will implement it with object pool
    */

    public RewardFactory()
    {
        ServiceProvider.Register(this);
    }

    public GameObject SpawnReward(RewardConfig config)
    {
        if(config == null) return null;

        RewardBase Reward  =  ServiceProvider.AssetLibrary.GetAsset<RewardBase>(AssetType.Reward);
        Reward.Init(config);
        return Reward.gameObject;
    }

}