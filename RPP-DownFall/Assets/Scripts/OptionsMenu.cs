using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{
    [Header("Referências de Interface")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Referências de Áudio")]
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Text volumeText;

    [Header("Cenas")]
    [SerializeField] private string firstGameplayScene = "Testes";

    private const string volumeParam = "MasterVol";

    private void Start()
    {
        
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);

        
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);
        UpdateVolumeLabel(savedVolume);

        
        volumeSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        SetVolume(value);
        UpdateVolumeLabel(value);
    }

    public void SetVolume(float value)
    {
        if (value <= 0.0001f)
            mainMixer.SetFloat(volumeParam, -80f);
        else
            mainMixer.SetFloat(volumeParam, Mathf.Log10(value) * 20f);

        PlayerPrefs.SetFloat("MasterVolume", value);
        AudioListener.volume = value; 
    }

    private void UpdateVolumeLabel(float value)
    {
        int percent = Mathf.RoundToInt(value * 100f);
        if (volumeText != null)
            volumeText.text = $"Volume: {percent}%";
    }

    // =============================
    //      FUNÇÕES DE MENU
    // =============================

    public void NovoJogo()
    {
        SceneManager.LoadScene(firstGameplayScene);
    }

    public void CarregarJogo()
    {
        string scene = SaveSystem.GetSavedScene();
        if (!string.IsNullOrEmpty(scene))
            SceneManager.LoadScene(scene);
        else
            Debug.Log("Nenhum jogo salvo encontrado!");
    }

    public void Sair()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // =============================
    //      TROCA DE PAINEL
    // =============================

    public void AbrirOpcoes()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }

    public void VoltarAoMenu()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }
}
