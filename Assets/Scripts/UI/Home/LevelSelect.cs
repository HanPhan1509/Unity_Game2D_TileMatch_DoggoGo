using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UI.Extensions;
using Services;
using System;
using DG.Tweening;

namespace Home
{
	public class LevelSelect : MonoBehaviour
	{
		[SerializeField] private HorizontalScrollSnap horizontalScrollSnap;

		[SerializeField] private Transform frame;
		[SerializeField] private TextMeshProUGUI totalStars;
		[SerializeField] private RectTransform        itemMapContent;
		[Header("ITEMS MAP")]
		[SerializeField] private GameObject           itemMapPrefab;

		[SerializeField] private UnityEvent           onButtonExit;
		[Space(0.8f)]
		private PlayerService playerService;
		private List<ItemMapInfo> lstMap;
		private Action<int> OnItemClicked;

		private float canvasScale = 1f;
		private int highestLevel = 0;

		public void Initialized(float canvasScale, int highestLevel, List<ItemMapInfo> lstMap, Action<int> OnItemClicked)
		{
			this.canvasScale = canvasScale;
			this.highestLevel = highestLevel;
			this.lstMap = lstMap;
			this.OnItemClicked = OnItemClicked;

			SetItemsMap();
		}

		public void SetTotalStarsOnMap(int number, int total)
		{
			totalStars.text = $"{number}/{total}";
		}

		public void SetItemsMap()
		{
			for(int i = 0; i < lstMap.Count; i++)
			{
				ItemsButton item = SimplePool.Spawn(itemMapPrefab, Vector2.zero, Quaternion.identity).GetComponent<ItemsButton>();
				item.SetItemInfo(lstMap[i], i, itemMapContent, canvasScale);
				item.OnItemClicked = () => OnItemClicked?.Invoke(item.Id);
				if(i == highestLevel)
				{
					item.ChangeBGCurrentLevel();
				}	
			}
		}

		public void ShowPopup()
		{
			frame.localScale = Vector2.zero;
			frame.DOScale(Vector2.one, 0.1f);
		}

		public void ButtonExit()
		{
			onButtonExit?.Invoke();
		}
	}

}
