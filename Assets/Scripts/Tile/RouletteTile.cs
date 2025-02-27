using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class RouletteTile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer backgroundRenderer;
    [SerializeField] private SpriteRenderer rewardIconRenderer;
    [SerializeField] private GameObject checkmarkObject;
    [SerializeField] private GameObject rewardAmountObject;
    [SerializeField] private TextMeshPro rewardAmountText;


    [Header("States")]
    [SerializeField] private Sprite defaultBackgroundSprite;
    [SerializeField] private Sprite highlightedBackgroundSprite;
    [SerializeField] private Sprite selectedBackgroundSprite;
    [SerializeField] private Sprite collectedBackgroundSprite;

    [Header("Animation")]
    [SerializeField] private float pulseSpeed = 5f;
    [SerializeField] private float pulseScale = 1.2f;
    [SerializeField] private int pulseCount = 3;

    private bool isHighlighted = false;
    private bool isSelected = false;
    private bool isCollected = false;

    public event Action OnAnimationComplete;
    private Coroutine currentAnimation;


    private void Awake()
    {
        if (checkmarkObject != null)
        {
            checkmarkObject.SetActive(false);
        }
    }

    public void Initialize(int rewardAmount, RewardType rewardType, Sprite rewardIcon)
    {

        if (rewardIconRenderer != null)
        {
            rewardIconRenderer.sprite = rewardIcon;
        }
        if(rewardAmount > 1)
        {
            rewardAmountText.text = rewardAmount.ToString();
            rewardAmountObject.SetActive(true);
        }

        ResetState();
    }
    public void SetHighlighted(bool highlighted)
    {
        isHighlighted = highlighted;
        UpdateVisuals();
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisuals();
    }

    public void StartSelectionSequence(Action onComplete = null)
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }

        currentAnimation = StartCoroutine(SelectionAnimationSequence(onComplete));
    }
    private IEnumerator SelectionAnimationSequence(Action onComplete)
    {
        isHighlighted = true;
        UpdateVisuals();

        Vector3 originalScale = transform.localScale;

        for (int i = 0; i < pulseCount; i++)
        {
            float duration = 0.15f;
            float startTime = Time.time;

            while (Time.time < startTime + duration)
            {
                float t = (Time.time - startTime) / duration;
                float scale = Mathf.Lerp(1f, pulseScale, t);
                transform.localScale = originalScale * scale;
                yield return null;
            }

            startTime = Time.time;
            while (Time.time < startTime + duration)
            {
                float t = (Time.time - startTime) / duration;
                float scale = Mathf.Lerp(pulseScale, 1f, t);
                transform.localScale = originalScale * scale;
                yield return null;
            }
        }

        transform.localScale = originalScale;

        isHighlighted = false;
        isSelected = true;
        UpdateVisuals();

        yield return new WaitForSeconds(0.5f);
        if (checkmarkObject != null)
        {
            checkmarkObject.SetActive(true);
        }

        yield return new WaitForSeconds(0.5f);
        isSelected = false;
        isCollected = true;
        UpdateVisuals();

        yield return new WaitForSeconds(0.5f);

        OnAnimationComplete?.Invoke();
        onComplete?.Invoke();

        currentAnimation = null;
    }

    public void SetCollected(bool collected)
    {
        isCollected = collected;
        UpdateVisuals();


        if (checkmarkObject != null)
        {

            checkmarkObject.SetActive(true);
        }
    }

    public void ResetState()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }

        isHighlighted = false;
        isSelected = false;
        isCollected = false;

        UpdateVisuals();

        if (checkmarkObject != null)
        {
            checkmarkObject.SetActive(false);
        }
    }

    public Sprite GetRewardSprite()
    {
        return rewardIconRenderer != null ? rewardIconRenderer.sprite : null;
    }

    private void UpdateVisuals()
    {
        if (backgroundRenderer == null) return;

        if (isCollected)
        {
            backgroundRenderer.sprite = collectedBackgroundSprite;
        }
        else if (isSelected)
        {
            backgroundRenderer.sprite = selectedBackgroundSprite;
        }
        else if (isHighlighted)
        {
            backgroundRenderer.sprite = highlightedBackgroundSprite;
        }
        else
        {
            backgroundRenderer.sprite = defaultBackgroundSprite;
        }
    }

}