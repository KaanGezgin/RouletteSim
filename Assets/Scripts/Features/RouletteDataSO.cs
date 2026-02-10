using UnityEngine;
using System.Collections.Generic;

// In wheel numbers color enum
public enum NumberColor { Green, Red, Black }

//Each roulette slots data
[System.Serializable]
public struct RouletteSlot
{
    public int number;
    public NumberColor color;
}

// Wheel Configurations (Europe/American)
[CreateAssetMenu(fileName = "RouletteData", menuName = "RouletteData")]
public class RouletteDataSO : ScriptableObject
{
    [Tooltip("Çark üzerindeki sýraya göre sayýlarý girin (Saat yönü vs.)")]
    public List<RouletteSlot> wheelSlots;

    // Helper method for finding numbers color
    public NumberColor GetColorOfNumber(int number)
    {
        foreach (var slot in wheelSlots)
        {
            if (slot.number == number) return slot.color;
        }
        return NumberColor.Green; // Default
    }
}