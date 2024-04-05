using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsPopup : MonoBehaviour
{
	[SerializeField] private Transform  frame;
	[SerializeField] private Slider     musicSlider;
	[SerializeField] private Slider     soundSlider;
	[SerializeField] private GameObject buttonQuit;
	[SerializeField] private UnityEvent<float> onButtonSoundChange;
	[SerializeField] private UnityEvent<float> onButtonMusicChange;
	[SerializeField] private UnityEvent onButtonExit;
	[SerializeField] private UnityEvent onButtonQuit;
	[SerializeField] private UnityEvent onButtonTerms;
	[SerializeField] private UnityEvent onButtonPrivacy;

	public void LoadSetting(float music, float sound)
	{
		musicSlider.value = music;
		soundSlider.value = sound;
	}

	public void ShowPopup()
	{
		frame.localScale = Vector2.zero;
		frame.DOScale(Vector2.one, 0.1f);
	}
	public void EnableButtonQuit()
	{
		buttonQuit.SetActive(false);
	}	

	public void SliderSound(Slider slider)
	{
		onButtonSoundChange?.Invoke(slider.value);
	}

	public void SliderMusic(Slider slider)
	{
		onButtonMusicChange?.Invoke(slider.value);
	}
	public void ButtonExit()
	{
		onButtonExit?.Invoke();
	}

	public void ButtonQuit()
	{
		onButtonQuit?.Invoke();
	}

	public void ButtonTerms()
	{
		onButtonTerms?.Invoke();
	}

	public void ButtonPrivacy()
	{
		onButtonPrivacy?.Invoke();
	}
	
}
