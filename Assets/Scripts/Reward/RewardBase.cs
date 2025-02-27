using UnityEngine;

public class RewardBase : MonoBehaviour
{
    [SerializeField] private RewardConfig rewardConfig;
    [SerializeField] private SpriteRenderer RewardRenderer;

    public void Init(RewardConfig config)
    {
        rewardConfig = config;
        Setup();
        MoveToWallet();
    }

    private void MoveToWallet()
    {
        //start a coroutine and when coroutine is finished call onreachtowallet
        OnReachToWallet();
    }

    public void Setup()
    {
        RewardRenderer.sprite = rewardConfig.rewardSprite;
    }

    public void OnReachToWallet()
    {
        Debug.Log("Collecting reward: " + rewardConfig.name);
        Destroy(gameObject);
    }

}