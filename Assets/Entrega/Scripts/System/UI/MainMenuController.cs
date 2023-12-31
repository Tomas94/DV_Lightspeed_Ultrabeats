using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MainMenuController : MonoBehaviour
{
    StaminaManager _staminaManager;
    CurrencyManager _currencyManager;
    UpgradePointsManager _upgradePointsManager;
    SceneManagerr _sceneManager;
    public TextMeshProUGUI contadorTiempo;

    public List<Toggle> levels;
    public TextMeshProUGUI crediBeatsAmount, staminaAmount, upgradePointsAmount;
    public Image staminaBar;

    public bool itemPurchased = false;

    private void Awake()
    {
        _staminaManager = StaminaManager.instance;
        _currencyManager = CurrencyManager.instance;
        _upgradePointsManager = UpgradePointsManager.instance;
        _sceneManager = SceneManagerr.instance;

    }

    private void Start()
    {
        StaminaBarUpdate();
        staminaAmount.text += ": " + _staminaManager.Stamina.ToString();
        crediBeatsAmount.text = _currencyManager.Currency.ToString();
        UpdateAvailableLevels();
        SceneManagerr.Resume();
    }

    private void Update()
    {
        UpdateAvailableLevels();
        crediBeatsAmount.text = _currencyManager.Currency.ToString();
        upgradePointsAmount.text = _upgradePointsManager.UpgradePoints.ToString();
        StaminaBarUpdate();
        
        if (_staminaManager.Stamina == _staminaManager.MaxStamina)
        {
            contadorTiempo.text = "";
            return;
        }
        contadorTiempo.text = Timer.instance.contador;
    }

    public void UpdateAvailableLevels()
    {
        for (int i = 0; i < GameManager.Instance.levelsUnlock; i++)
        {
            Debug.Log("dentro del for con i = " + i);
            levels[i].interactable = true;
        }
    }

    public void PlayLevelSelected()
    {
        if (_staminaManager.Stamina <= 0) return;
        _staminaManager.ConsumeStamina();
        string levelName = "Level_";

       for(int i = 0; i <levels.Count; i++)
        {
            if (levels[i].isOn)
            {
                levelName += (i+1).ToString();
                continue;
            }
        }

        Debug.Log("Consumio estamina");
        _sceneManager.PlayLevel(levelName);
        Debug.Log("Entr� al nivel");
        StaminaBarUpdate();
    }

    public void TryPlayLevel(string levelName)
    {
        if (_staminaManager.Stamina <= 0) return;
        _staminaManager.ConsumeStamina();
        Debug.Log("Consumio estamina");
        _sceneManager.PlayLevel(levelName);
        Debug.Log("Entr� al nivel");
        StaminaBarUpdate();
    }

    #region Funciones Relacionadas a Compra de Objetos

    public void BuyItem(int cost)
    {
        if (CurrencyManager.instance.Currency < cost) return;
        itemPurchased = true;
        _currencyManager.SpentCurrency(cost);
        crediBeatsAmount.text = CurrencyManager.instance.Currency.ToString();
    }

    public void BuyUpgrade(int cost)
    {
        _upgradePointsManager.SpentUP(cost);
    }

    public void DisableItemPurchased(Button boton)
    {
        if (!itemPurchased) return;
        boton.interactable = false;
    }
    #endregion


    public void TryRefillStaminaPaid(int cost)
    {
        if (_currencyManager.Currency < cost) return;
        _staminaManager.PayForRecharge();

    }

    public void BuyPoints(int amount)
    {
        if (_currencyManager.Currency < 450) return;
        _upgradePointsManager.GainUPByAds(20);
    }

    public void GainUPAd()
    {
        _upgradePointsManager.GainUPByAds(5);
    }

    public void StaminaBarUpdate()
    {
        var updatedStaminaAmount = (float)_staminaManager.Stamina / (float)_staminaManager.MaxStamina;
        staminaBar.fillAmount = updatedStaminaAmount;
        staminaAmount.text = _staminaManager.Stamina.ToString();
    }

    public void QuitGame()
    {
        GameManager.Instance.SavePlayerPrefs();
        Application.Quit();
    }

    public void ResetData()
    {
        GameManager.Instance.ResetProgress();
    }
}
