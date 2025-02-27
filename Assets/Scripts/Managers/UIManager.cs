using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, IProvidable
{
    [SerializeField] private RouletteBase roulette;

    [SerializeField] private Button spinButton;
    [SerializeField] private GameObject rewardPopup;
    [SerializeField] private TextMeshProUGUI rewardText;


    void Start()
    {
        roulette.OnSpinCompleted += HandleSpinCompleted;
        roulette.OnRewardSelected += ShowRewardPopup;
    }

    void HandleSpinCompleted()
    {
        spinButton.interactable = true;
    }

    void ShowRewardPopup(RewardType rewardType)
    {
        rewardText.text = rewardType.ToString();
        rewardPopup.SetActive(true);
    }

    public void OnAgainButtonPressed()
    {
        rewardPopup.SetActive(false);


    }


}