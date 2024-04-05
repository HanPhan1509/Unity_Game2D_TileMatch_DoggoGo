using Home;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemsCollection : MonoBehaviour
{
	[SerializeField] private RectTransform rect;
	[SerializeField] private TextMeshProUGUI txtNameMeme;
	[SerializeField] private Image imgMeme;
	public Action OnItemClicked;

	public void ClickedItem()
	{
		OnItemClicked?.Invoke();
	}

	public void SetCollectionInfo(ItemCollection item, RectTransform parent)
	{
		rect.SetParent(parent);
		txtNameMeme.text = item.nameMeme.ToString();
		imgMeme.sprite = item.spriteMeme;
		if(item.isCollect < 0)
		{
			imgMeme.color = Color.black;
		} else
		{
			imgMeme.color = Color.white;
		}
	}	
}
