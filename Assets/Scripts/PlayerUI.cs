using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Slider heatSlider;
    public Image heatFill;

    private PlayerController _playerCntrl;
    private PlayerCombat _playerCmbt;

    private void Start()
    {
        _playerCntrl = GetComponent<PlayerController>();
        _playerCmbt = GetComponent<PlayerCombat>();
    }

    void Update()
    {
        UpdateHeatUI();
    }

    void UpdateHeatUI()
    {
        if (heatSlider != null)
        {
            heatSlider.value = _playerCmbt.currentHeat;

            float heatPercent = _playerCmbt.currentHeat / _playerCmbt.maxHeat;

            heatSlider.gameObject.SetActive(_playerCntrl.pistolMode.activeSelf);

            if (heatFill != null)
            {
                Color color;

                if (heatPercent < 0.5f)
                {
                    color = Color.Lerp(Color.green, Color.yellow, heatPercent / 0.5f);
                }
                else
                {
                    color = Color.Lerp(Color.yellow, Color.red, (heatPercent - 0.5f) / 0.5f);
                }

                if (_playerCmbt.currentHeat <= 0.01f)
                {
                    color.a = 0f;
                }
                else
                {
                    color.a = 1f;
                }

                heatFill.color = color;
            }
        }
    }
}
