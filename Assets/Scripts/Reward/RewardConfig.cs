using UnityEngine;

[CreateAssetMenu(fileName = "Reward" , menuName = "FortuneMineCase/RewardConfig")]
public class RewardConfig : ScriptableObject
{
    public string rewardName;
    public RewardType rewardType;
    public Sprite rewardSprite;
    public int amount;
}