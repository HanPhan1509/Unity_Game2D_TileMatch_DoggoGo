using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DescibeBooster : MonoBehaviour
{
	[SerializeField] private Transform frame;
	[SerializeField] private TextMeshProUGUI textDescibe;
	[SerializeField] private string[] descibe;

	public void ShowPopup()
	{
		frame.localScale = Vector2.zero;
		frame.DOScale(Vector2.one, 0.1f);
	}

	public void ChangeContentDescibeBooster(int levelTutorial)
	{
		textDescibe.text = descibe[levelTutorial];
	}	
}
