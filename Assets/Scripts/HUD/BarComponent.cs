using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarComponent : MonoBehaviour
{
    [SerializeField] private Image Bar;

    public void UpdateBarValue(float value)
    {
        if (Bar == null) return;
        Bar.fillAmount = value;
    }
}
