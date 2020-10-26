using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EnhancedUI.EnhancedScroller;

public class AnimalCellView : EnhancedScrollerCellView 
{
    public Text animalNameText;

    public void SetData(ScrollerData data)
    {
        animalNameText.text = data.animalName;
    }
}
