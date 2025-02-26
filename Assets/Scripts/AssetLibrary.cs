using UnityEngine;

public class AssetLibrary : MonoBehaviour, IProvidable
{
    [Header("Prefabs")]
    [SerializeField] private GameObject TilePrefab;
    [SerializeField] private GameObject RewardPrefab;
    [SerializeField] private GameObject RoulettePrefab;
    [SerializeField] private GameObject WalletPrefab;


    private  void Awake()
    {
        ServiceProvider.Register(this);
    }
    public T GetAsset<T>(AssetType assetType) where T : class
    {
        var asset = GetAsset(assetType);
        return asset == null ? null : asset.GetComponent<T>();
    }

    private GameObject GetAsset(AssetType assetType)
    {
        return assetType switch
        {
            AssetType.Tile => Instantiate(TilePrefab),
            AssetType.Reward => Instantiate(RewardPrefab),
            AssetType.Roulette => Instantiate(RoulettePrefab),
            AssetType.Wallet => Instantiate(WalletPrefab),
            _ => null
        };
    }
}

public enum AssetType
{
    Tile,
    Reward,
    Roulette,
    Wallet
}
