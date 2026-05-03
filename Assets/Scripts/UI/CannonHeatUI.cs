using UnityEngine;
using UnityEngine.UI;

public class CannonHeatUI : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private Slider heatSlider;
    [SerializeField] private Image fillImage;

    [Header("Gradient de calor")]
    [SerializeField] private Gradient heatGradient;

    [Header("Overheat")]
    [SerializeField] private float pulseSpeed = 6f;
    [SerializeField] private float pulseMinAlpha = 0.3f;
    [SerializeField] private Color overheatColor = new Color(1f, 0.1f, 0.1f);

    private bool isOverheated;
    private float currentHeat;

    private void Awake()
    {
        if (heatSlider != null)
        {
            heatSlider.minValue = 0f;
            heatSlider.maxValue = 100f;
            heatSlider.value = 0f;
        }
    }

    private void Update()
    {
        if (fillImage == null) return;

        if (isOverheated)
        {
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
            float alpha = Mathf.Lerp(pulseMinAlpha, 1f, pulse);

            Color c = overheatColor;
            c.a = alpha;
            fillImage.color = c;
        }
        else
        {
            Color c = heatGradient.Evaluate(currentHeat / 100f);
            c.a = 1f;
            fillImage.color = c;
        }
    }

    public void UpdateHeat(float heat, bool overheated)
    {
        currentHeat = heat;
        isOverheated = overheated;

        if (heatSlider != null)
            heatSlider.value = heat;
    }
}
