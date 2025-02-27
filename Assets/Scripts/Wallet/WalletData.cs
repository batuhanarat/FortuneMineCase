using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerWallet", menuName = "Roulette/Wallet")]
public class WalletData : ScriptableObject
{

    [Serializable]
    public struct WalletRewardEntry
    {
        public RewardType RewardType;
        public int Amount;
    }

    public List<WalletRewardEntry> rewardEntries = new();
    private readonly Dictionary<RewardType,int> rewardDictionary = new();

    public void Add(RewardType rewardType, int count)
    {
        if (rewardDictionary.ContainsKey(rewardType))
        {
            rewardDictionary[rewardType] += count;
            int index = rewardEntries.FindIndex(x => x.RewardType == rewardType);
            if (index != -1)
            {
                var entry = rewardEntries[index];
                entry.Amount = rewardDictionary[rewardType];
                rewardEntries[index] = entry;
            }
        }
        else
        {
            rewardDictionary.Add(rewardType, count);
            rewardEntries.Add(new WalletRewardEntry { RewardType =rewardType, Amount = count });

        }
    }
    public bool IsExists(RewardType rewardType)
    {
        return rewardDictionary.ContainsKey(rewardType);
    }

    public int GetCount(RewardType rewardType)
    {
        if (rewardDictionary.ContainsKey(rewardType))
        {
            return rewardDictionary[rewardType];
        }
        return 0;
    }

    public void Reset()
    {
        rewardDictionary.Clear();
        rewardEntries.Clear();
    }


}