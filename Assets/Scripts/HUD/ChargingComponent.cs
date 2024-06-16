using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChargingComponent : MonoBehaviour
{
    [SerializeField] private TMP_Text Charges;
    [SerializeField] private Image Progress;
    [SerializeField] private GameObject FullChargeImage;

    // Start is called before the first frame update
    void Start()
    {
        Charges.text = $"{0}";
        Progress.fillAmount = 0;
        FullChargeImage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateObjects(float progress, uint charges)
    {
        Charges.text = $"{charges}";
        Progress.fillAmount = progress;
        FullChargeImage.SetActive(charges > 1);
    }
}
