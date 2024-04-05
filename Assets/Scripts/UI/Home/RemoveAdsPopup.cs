using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class RemoveAdsPopup : MonoBehaviour
{
	[SerializeField] private Transform frame;
	[SerializeField] private TextMeshProUGUI txtPrice;
	[SerializeField] private UnityEvent onButtonNo;
	[SerializeField] private UnityEvent onButtonPay;

	public void ChangePrice(string price)
	{
		txtPrice.text = price;
	}	

	public void ButtonNo()
	{
		onButtonNo?.Invoke();
	}

	public void ButtonPay()
	{
		onButtonPay?.Invoke();
	}

	public void ShowPopup()
	{
		frame.localScale = Vector2.zero;
		frame.DOScale(Vector2.one, 0.1f);
	}
}
