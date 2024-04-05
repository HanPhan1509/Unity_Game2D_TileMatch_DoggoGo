using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Username : MonoBehaviour
{
	[SerializeField] private Transform frame;
	[SerializeField] private TMP_InputField enterName;
	[SerializeField] private Transform warningText;
	[SerializeField] private UnityEvent<string> onButtonSave;
	//[SerializeField] private UnityEvent onButtonReplay;

	//public void ButtonReplay()
	//{
	//	onButtonReplay?.Invoke();
	//}
	public void ShowPopup()
	{
		frame.localScale = Vector2.zero;
		frame.DOScale(Vector2.one, 0.1f);
	}

	public void ButtonSave()
	{
		if (Regex.IsMatch(enterName.text, @"^[a-zA-Z0-9]+$"))
		{
			onButtonSave?.Invoke(enterName.text);
		}
		else
		{
			warningText.DOScale(new Vector2(1.2f, 1.2f), 0.1f).OnComplete(() => warningText.DOScale(Vector2.one, 0.1f));
		}
	}
}
