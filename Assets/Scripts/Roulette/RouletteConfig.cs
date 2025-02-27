using UnityEngine;

[CreateAssetMenu(fileName = "Roulette Config", menuName = "FortuneMineCase/RouletteConfig")]
public class RouletteConfig : ScriptableObject
{
    [Header("Layout")]
    public int slotCount = 12;
    public float wheelRadius = 2.5f;

    [Header("Animation Settings")]
    public float minSpinDuration = 3f;
    public float maxSpinDuration = 5f;
    public int minRotations = 2;
    public int maxRotations = 4;
    public float spinStartSpeed = 0.05f;
    public float spinEndSpeed = 0.5f;

}