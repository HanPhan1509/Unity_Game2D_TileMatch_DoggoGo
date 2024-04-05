using Extensions;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Home
{
	public class HomeScene : MonoBehaviour
	{
		[SerializeField] private RectTransform rectTransform;
		[SerializeField] private ScrollRect scrollRect;
		[Header("TABS CONTROL")]
		[SerializeField] private HorizontalScrollSnap horizontalScrollSnap;
		[SerializeField] private List<GameObject> tabGroup;
		[SerializeField] private GameObject toggleRank;

		[Space(1.5f)]
		[Header("ADVENTURE TAB")]
		[SerializeField] private TextMeshProUGUI totalStars;
		[SerializeField] private Button buttonSelectLevel;
		[SerializeField] private Button buttonRemoveAds;

		[Space(1.5f)]
		[Header("ACTION BUTTON")]
		[SerializeField] private UnityEvent onButtonRanked;
		[SerializeField] private UnityEvent onButtonPlay;
		[SerializeField] private UnityEvent onButtonSelectLevel;
		[SerializeField] private UnityEvent onButtonSetting;
		[SerializeField] private UnityEvent onButtonRemoveAds;


		public void Initialize(GameMode gameMode, bool isRemove)
		{
			bool isTutorial = gameMode == GameMode.Tutorial;
			scrollRect.horizontal = !isTutorial;
			toggleRank.SetActive(!isTutorial);
			buttonSelectLevel.gameObject.SetActive(!isTutorial);
			HideRemoveAdsButton(!isRemove);
		}

		private void Awake()
		{
			rectTransform.ThrowIfNull();
			horizontalScrollSnap.ThrowIfNull();
			totalStars.ThrowIfNull();
			buttonSelectLevel.ThrowIfNull();

			foreach (var item in tabGroup)
			{
				item.SetActive(true);
			}

			ChangePage(horizontalScrollSnap.StartingScreen);
		}

		public void AvoidCutouts(Rect safeArea)
		{
			rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -safeArea.y);
		}

		public void ChangePage(int numberPage)
		{
			horizontalScrollSnap.GoToScreen(numberPage);
		}

		public void ChangeTab(int i)
		{
			ChangePage(i);
		}
		public void SetTotalStarsOnMap(int number, int total)
		{
			totalStars.text = $"{number}/{total}";
		}

		public void HideRemoveAdsButton(bool isActive = false)
		{
			buttonRemoveAds.gameObject.SetActive(isActive);
		}	

		#region ACTION BUTTON
		public void ButtonRanked()
		{
			onButtonRanked?.Invoke();
		}

		public void ButtonPlay()
		{
			onButtonPlay?.Invoke();
		}

		public void ButtonSelectLevel()
		{
			onButtonSelectLevel?.Invoke();
		}

		public void ButtonSetting()
		{
			onButtonSetting?.Invoke();
		}
		public void ButtonRemoveAds()
		{
			onButtonRemoveAds?.Invoke();
		}
		#endregion
	}

}
