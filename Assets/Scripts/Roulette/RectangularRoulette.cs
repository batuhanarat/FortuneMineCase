using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RectangularRoulette : RouletteBase
{
    [Header("Layout Settings")]
    [SerializeField] private int topCount = 4;
    [SerializeField] private int bottomCount = 4;
    [SerializeField] private int leftCount = 3;
    [SerializeField] private int rightCount = 3;
    [SerializeField] private float tileSpacing = 1.0f;
    [SerializeField] private Vector3 layoutScale = new(0.5f, 0.5f, 0.5f);

    [Header("Tile Settings")]
    [SerializeField] private GameObject tilePrefab;


    private readonly List<RouletteTile> _tiles = new();
    private readonly HashSet<int> _collectedTiles = new();
    private int _currentHighlightedIndex = -1;
    private enum Side { Top, Bottom, Left, Right }
    private struct SpinParameters
    {
        public int TargetIndex;
        public float Duration;
        public int TotalPositions;
        public float StartTime;
    }

    public override bool AreAllRewardsCollected => _collectedTiles.Count >= _tiles.Count;

    void Start()
    {
        InitializeRewards();
        SetupRoulette();
    }

    private void InitializeRewards()
    {
        foreach(var entry in RewardsEntryArray)
        {
            entry.rewardConfig.amount = entry.amount;
        }
    }

    protected override void SetupRoulette()
    {
        base.SetupRoulette();
        CreateRectangularLayout();

        if (config != null)
        {
            config.slotCount = _tiles.Count;
        }

        transform.localScale = layoutScale;
    }

    private void CreateRectangularLayout()
    {
        Vector2 dimensions = CalculateLayoutDimensions();
        Vector3 center = transform.position;

        float leftX = center.x - dimensions.x / 2;
        float rightX = center.x + dimensions.x / 2;
        float topY = center.y + dimensions.y / 2;
        float bottomY = center.y - dimensions.y / 2;

        CreateSideTiles(Side.Top, topCount, leftX, rightX, topY);
        CreateSideTiles(Side.Bottom, bottomCount, leftX, rightX, bottomY);
        CreateSideTiles(Side.Left, leftCount, bottomY, topY, leftX);
        CreateSideTiles(Side.Right, rightCount, bottomY, topY, rightX);
    }

    private Vector2 CalculateLayoutDimensions()
    {
        float width = (rightCount + leftCount + 2) * tileSpacing;
        float height = (topCount + bottomCount + 2) * tileSpacing;
        return new Vector2(width, height);
    }

    private void CreateSideTiles(Side side, int count, float startPos, float endPos, float fixedPos)
    {
        for (int i = 0; i < count; i++)
        {
            float ratio = (i + 1.0f) / (count + 1);
            Vector3 position;

            if (side == Side.Top || side == Side.Bottom)
            {
                float xPos = Mathf.Lerp(startPos, endPos, ratio);
                position = new Vector3(xPos, fixedPos, 0);
            }
            else
            {
                float yPos = Mathf.Lerp(startPos, endPos, ratio);
                position = new Vector3(fixedPos, yPos, 0);
            }

            CreateTile(position, $"{side}Tile_{i}");
        }
    }

    private void CreateTile(Vector3 position, string name)
    {
        GameObject tileObj = Instantiate(tilePrefab, position, Quaternion.identity, transform);
        tileObj.name = name;

        RouletteTile tile = tileObj.GetComponent<RouletteTile>();
        if (tile != null)
        {
            _tiles.Add(tile);

            int index = _tiles.Count - 1;

            var rewardConfig = RewardsEntryArray[index].rewardConfig;
            if (index < RewardsEntryArray.Length)
            {
                Sprite rewardSprite = rewardConfig.rewardSprite;
                if (rewardSprite != null)
                {
                    tile.Initialize(rewardConfig.amount, rewardConfig.rewardType, rewardSprite);
                }
            }
        }
    }

    private void OnValidate()
    {
        if (config != null)
        {
            if (RewardsEntryArray == null || RewardsEntryArray.Length != config.slotCount)
            {
                RewardEntry[] oldRewards = RewardsEntryArray;
                RewardsEntryArray = new RewardEntry[config.slotCount];

                if (oldRewards != null)
                {
                    int copyLength = Mathf.Min(oldRewards.Length, config.slotCount);
                    for (int i = 0; i < copyLength; i++)
                    {
                        RewardsEntryArray[i] = oldRewards[i];
                    }
                }

                for(int i = 0; i < config.slotCount; i++)
                {
                    if(RewardsEntryArray[i].amount == 0)
                    {
                        RewardsEntryArray[i].amount = 1;
                    }
                }
            }
        }
    }

    public override void HighlightReward(int index, bool highlight)
    {
        if (index < 0 || index >= _tiles.Count)
            return;

        _tiles[index].SetHighlighted(highlight);
    }

    public override void MarkRewardCollected(int index)
    {
        if (index < 0 || index >= _tiles.Count)
            return;

        _tiles[index].SetCollected(true);
        _collectedTiles.Add(index);

        if (_collectedTiles.Count >= _tiles.Count)
        {
            NotifyAllRewardsCollected();
        }
    }

    public override void ResetRoulette()
    {
        foreach (var tile in _tiles)
        {
            if (tile != null)
            {
                tile.ResetState();
            }
        }

        _currentHighlightedIndex = -1;
        isSpinning = false;
    }

    protected override IEnumerator SpinCoroutine()
    {
        int targetIndex = GetRandomUncollectedTileIndex();

        SpinParameters parameters = CalculateSpinParameters(targetIndex);

        yield return AnimateSpin(parameters);

        ProcessRewardSelection(targetIndex);

        isSpinning = false;
        NotifySpinCompleted();
    }


    private SpinParameters CalculateSpinParameters(int targetIndex)
    {
        float minDuration = config != null ? config.minSpinDuration : 3f;
        float maxDuration = config != null ? config.maxSpinDuration : 5f;
        int minRotations = config != null ? config.minRotations : 2;
        int maxRotations = config != null ? config.maxRotations : 4;

        float spinDuration = Random.Range(minDuration, maxDuration);
        int spinCount = Random.Range(minRotations, maxRotations);
        int totalPositions = spinCount * _tiles.Count + targetIndex;

        return new SpinParameters
        {
            TargetIndex = targetIndex,
            Duration = spinDuration,
            TotalPositions = totalPositions,
            StartTime = Time.time
        };
    }
    private IEnumerator AnimateSpin(SpinParameters parameters)
    {
        float startSpeed = config != null ? config.spinStartSpeed : 0;
        float endSpeed = config != null ? config.spinEndSpeed : 0;

        while (Time.time - parameters.StartTime < parameters.Duration)
        {
            float progress = (Time.time - parameters.StartTime) / parameters.Duration;

            float easedProgress = EaseOutCubic(progress);

            int currentPosition = Mathf.FloorToInt(easedProgress * parameters.TotalPositions);
            int currentIndex = currentPosition % _tiles.Count;

            UpdateHighlightedTile(currentIndex);

            float delayTime = Mathf.Lerp(startSpeed, endSpeed, easedProgress);
            yield return new WaitForSeconds(delayTime);
        }

        ClearHighlightedTile();
    }

    private void ProcessRewardSelection(int targetIndex)
        {
            _currentHighlightedIndex = targetIndex;

            _tiles[targetIndex].StartSelectionSequence(() => {

                MarkRewardCollected(targetIndex);
                NotifyRewardSelected(RewardsEntryArray[targetIndex].rewardConfig.rewardType);
                ServiceProvider.WalletManager.AddReward(RewardsEntryArray[targetIndex].rewardConfig);
            });

        }


    private int GetRandomUncollectedTileIndex()
    {
        if (_collectedTiles.Count >= _tiles.Count)
        {
            _collectedTiles.Clear();
            ResetAllTiles();
        }

        List<int> availableTiles = new();
        for (int i = 0; i < _tiles.Count; i++)
        {
            if (!_collectedTiles.Contains(i))
            {
                availableTiles.Add(i);
            }
        }

        return availableTiles[Random.Range(0, availableTiles.Count)];
    }

    private void ResetAllTiles()
    {
        foreach (var tile in _tiles)
        {
            if (tile != null)
            {
                tile.ResetState();
            }
        }
    }

    private void UpdateHighlightedTile(int newIndex)
    {
        if (_currentHighlightedIndex >= 0 && _currentHighlightedIndex < _tiles.Count)
        {
            _tiles[_currentHighlightedIndex].SetHighlighted(false);
        }

        _currentHighlightedIndex = newIndex;
        _tiles[_currentHighlightedIndex].SetHighlighted(true);
    }

    private void ClearHighlightedTile()
    {
        if (_currentHighlightedIndex >= 0 && _currentHighlightedIndex < _tiles.Count)
        {
            _tiles[_currentHighlightedIndex].SetHighlighted(false);
        }
    }

    private float EaseOutCubic(float t)
    {
        return 1 - Mathf.Pow(1 - t, 3);
    }




}