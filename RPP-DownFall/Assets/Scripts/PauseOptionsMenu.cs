using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PauseOptionsMenu : MonoBehaviour
{
    [Header("Referências de Interface")]
    [SerializeField] private GameObject pausePanel;       // PauseScreen
    [SerializeField] private GameObject optionsPanel;     // OptionsPanelJogo

    [Header("Referências de Áudio")]
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Text volumeText;

    private const string volumeParam = "MasterVol";

    private bool initialized = false;

    private void Start()
    {
        // Certifica que o painel de opções começa desativado
        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        // Inicializa o volume salvo
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
            volumeSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        SetVolume(savedVolume);
        UpdateVolumeLabel(savedVolume);

        initialized = true;
    }

    private void OnSliderValueChanged(float value)
    {
        if (!initialized) return;

        SetVolume(value);
        UpdateVolumeLabel(value);
    }

    private void UpdateVolumeLabel(float value)
    {
        if (volumeText != null)
        {
            int percent = Mathf.RoundToInt(value * 100f);
            volumeText.text = $"Volume: {percent}%";
        }
    }

    public void SetVolume(float value)
    {
        if (mainMixer == null) return;

        if (value <= 0.0001f)
            mainMixer.SetFloat(volumeParam, -80f);
        else
            mainMixer.SetFloat(volumeParam, Mathf.Log10(value) * 20f);

        PlayerPrefs.SetFloat("MasterVolume", value);
        AudioListener.volume = value;
    }

    // ================================
    //  FUNÇÕES DE TROCA DE PAINEL
    // ================================

    public void AbrirOpcoes()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        // Usa coroutine pra abrir 1 frame depois e garantir que o clique funciona mesmo com Time.timeScale = 0
        StartCoroutine(AtivarOpcoesComDelay());
    }

    private IEnumerator AtivarOpcoesComDelay()
    {
        yield return null; // espera 1 frame
        if (optionsPanel != null)
            optionsPanel.SetActive(true);
    }

    public void VoltarParaPause()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(true);
    }
}
