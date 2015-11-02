using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour {

	public float masterVolume;
	public float sfxVolume;
	public float musicVolume;
    public Slider masterSlider;
    public Slider sfxSlider;
    public Slider musicSlider;
    public AudioMixer audioMixer;

	void Awake()
	{
		if (PlayerPrefs.HasKey("masterVolume"))
		{
			masterVolume = PlayerPrefs.GetFloat("masterVolume");
            masterSlider.value = masterVolume;
            audioMixer.SetFloat("masterVol", masterVolume);
        }
		else
		{
            masterVolume = 0f;
            masterSlider.value = masterVolume;
            audioMixer.SetFloat("masterVol", masterVolume);
            PlayerPrefs.SetFloat("masterVolume", masterVolume);
		}

		if (PlayerPrefs.HasKey("sfxVolume"))
		{
			sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
            sfxSlider.value = sfxVolume;
            audioMixer.SetFloat("sfxVol", sfxVolume);
        }
		else
		{
            sfxVolume = 0f;
            sfxSlider.value = sfxVolume;
            audioMixer.SetFloat("sfxVol", sfxVolume);
            PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
		}

		if (PlayerPrefs.HasKey("musicVolume"))
		{
			musicVolume = PlayerPrefs.GetFloat("musicVolume");
            musicSlider.value = musicVolume;
            audioMixer.SetFloat("musicVol", musicVolume);
        }
		else
		{
            musicVolume = 0f;
            musicSlider.value = musicVolume;
            audioMixer.SetFloat("musicVol", musicVolume);
            PlayerPrefs.SetFloat("musicVolume", musicVolume);
		}
	}

	public void SaveOptions()
	{
        masterVolume = masterSlider.value;
        sfxVolume = sfxSlider.value;
        musicVolume = musicSlider.value;
        audioMixer.SetFloat("masterVol", masterVolume);
        audioMixer.SetFloat("sfxVol", sfxVolume);
        audioMixer.SetFloat("musicVol", musicVolume);
        PlayerPrefs.SetFloat("masterVolume", masterVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
    }

}
