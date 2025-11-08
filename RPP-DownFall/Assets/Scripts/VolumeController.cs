using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class VolumeController : MonoBehaviour
{
    public AudioMixer mainMixer;
    public Slider volumeSlider;
    public TextMeshProUGUI volumeText;

    private void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);
    }

    public void SetVolume(float volume)
    {
        mainMixer.SetFloat("MasterVol", Mathf.Log10(volume) * 20);
        volumeText.text = "Volume: " + Mathf.RoundToInt(volume * 100) + "%";
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }
}