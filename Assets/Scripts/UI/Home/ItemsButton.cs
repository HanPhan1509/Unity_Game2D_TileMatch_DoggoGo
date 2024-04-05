using Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Home
{
	public class ItemsButton : MonoBehaviour
	{
		public int Id { get; set; }
		[SerializeField] private RectTransform   rect;
		[SerializeField] private Image           lockMap;
		[SerializeField] private TextMeshProUGUI txtNumberMap;
		[SerializeField] private Image[]         stars;
		[SerializeField] private Sprite          starLight;
		[SerializeField] private GameObject      starArea;
		[SerializeField] private Button          button;
		[SerializeField] private Sprite          spriteCurrentLevel;
		[SerializeField] private Image           BG;

		public Action OnItemClicked;

		private void Awake()
		{
			rect.ThrowIfNull();
			starLight.ThrowIfNull();
			starArea.ThrowIfNull();
			button.ThrowIfNull();
			lockMap.ThrowIfNull();
			txtNumberMap.ThrowIfNull();
		}

		public void ClickedItem()
		{
			OnItemClicked?.Invoke();
		}

		public void ChangeBGCurrentLevel()
		{
			BG.sprite = spriteCurrentLevel;
		}	

		public void SetItemInfo(ItemMapInfo item, int id, RectTransform parent, float scale)
		{
			this.rect.localScale = this.rect.localScale * scale;
			this.Id = id;
			//this.Id = item.idMap;
			rect.SetParent(parent);
			if (item.numberStar < 0)
			{
				lockMap.gameObject.SetActive(true);
				txtNumberMap.text = string.Empty;
				button.interactable = false;
				starArea.SetActive(false);
			}
			else
			{
				lockMap.gameObject.SetActive(false);
				txtNumberMap.text = (this.Id + 1).ToString();
				button.interactable = true;
				starArea.SetActive(true);

				if (item.numberStar > 0)
				{
					for (int i = 0; i < item.numberStar; i++)
					{
						stars[i].sprite = starLight;
					}
				}
			}
		}
	}
}
