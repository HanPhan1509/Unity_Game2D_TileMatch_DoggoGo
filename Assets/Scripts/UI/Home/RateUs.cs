using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class RateUs : MonoBehaviour
{
	[SerializeField] private Transform frame;
	[SerializeField] private Toggle[] starToggles;
	[SerializeField] private UnityEvent onButtonExit;
	[SerializeField] private UnityEvent<int> onButtonYes;
	private int star = 0;
	public void ShowPopup()
	{
		frame.localScale = Vector2.zero;
		frame.DOScale(Vector2.one, 0.1f);
	}

	public void OnClickStar(int index)
	{
		star = index;
		for (int i = 0; i < starToggles.Length; i++)
		{
			starToggles[i].isOn = i < index;
		}
	}

	public void ButtonExit()
	{
		onButtonExit.Invoke();
	}

	public void ButtonYes()
	{
		onButtonYes.Invoke(star);
	}	
}
