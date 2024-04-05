using DG.Tweening;
using Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Cleared : MonoBehaviour
{
	[SerializeField] private RectTransform frame;

	[SerializeField] private Animator animBonusButton;
	[SerializeField] private GameObject bonusArea;

	[SerializeField] private GameObject[] stars;
	[SerializeField] private Sprite[] boostersImage;
	[SerializeField] private Image boosterBonus;

	[SerializeField] private TextMeshProUGUI txtLevel;
	[SerializeField] private TextMeshProUGUI txtScore;

	[Header("BUTTON")]
	[SerializeField] private Button btnBonus;
	[SerializeField] private GameObject btnNext;

	[Header("DETAILS INFOMATION POPUP WIN/LOSE")]
	[SerializeField] private Sprite[] bannerWinLose;
	[SerializeField] private Image banner;
	[SerializeField] private TextMeshProUGUI txtTitlePopup;
	private readonly string[] nameTitle = { "Cleared!", "Defeated!" };

	[Header("EVENT BUTTON")]
	[SerializeField] private UnityEvent<Booster> OnButtonBonusBooster;
	[SerializeField] private UnityEvent OnButtonHome;
	[SerializeField] private UnityEvent OnButtonReplay;
	[SerializeField] private UnityEvent OnButtonNext;

	private Booster booster;

	private void Awake()
	{
		frame.ThrowIfNull();
		boosterBonus.ThrowIfNull();
		txtLevel.ThrowIfNull();
		txtScore.ThrowIfNull();
		bonusArea.ThrowIfNull();
		txtTitlePopup.ThrowIfNull();
		banner.ThrowIfNull();
		btnNext.ThrowIfNull();
		btnBonus.ThrowIfNull();
		animBonusButton.ThrowIfNull();
	}

	public void ShowPopup()
	{
		frame.localScale = Vector2.zero;
		frame.DOScale(Vector2.one, 0.1f);
	}

	public void SetPositionPopup(float heightMREC, float canvasScale)
	{
		float heightPopup = ((frame.rect.height / 2) + 125f) / canvasScale;
		float setPosY = Screen.height / canvasScale - heightMREC / canvasScale - heightPopup;
		float centerScreen = (Screen.height / canvasScale) / 2;
		if (centerScreen < setPosY)
			setPosY = centerScreen;
		frame.anchoredPosition = new Vector2(frame.anchoredPosition.x, setPosY);
	}

	public void ChangeDetailCompleteLevelPopup(bool isWin, StarLevel Star, int level, int score, int bonusBooster, bool isLoadRewardSuccess, bool lastLevel = false)
	{
		//Set BG banner + title popup
		banner.sprite = bannerWinLose[isWin ? 0 : 1];
		txtTitlePopup.text = nameTitle[isWin ? 0 : 1];

		//Set stars
		if (isWin)
		{
			for (int i = 0; i < stars.Length; i++)
			{
				if (i < (int)Star)
					stars[i].SetActive(true);
			}
		}

		//Set level + score
		txtLevel.text = (level + 1).ToString();
		txtScore.text = score.ToString();

		//Check to hide button next level
		btnNext.SetActive(isWin);
		if (lastLevel) btnNext.SetActive(false);

		//Set bonus booster
		this.booster = (Booster)bonusBooster;
		boosterBonus.sprite = boostersImage[bonusBooster];
		boosterBonus.SetNativeSize();

		//Check isReadyRewardAds to hide Area button bonus booster
		bonusArea.SetActive(isLoadRewardSuccess);
	}

	#region EVENT BUTTON
	public void ButtonBonusBooster()
	{
		animBonusButton.SetBool("Get", true);
		btnBonus.interactable = false;
		OnButtonBonusBooster?.Invoke(booster);
	}
	public void ButtonHome()
	{
		OnButtonHome?.Invoke();
	}

	public void ButtonReplay()
	{
		OnButtonReplay?.Invoke();
	}

	public void ButtonNext()
	{
		OnButtonNext?.Invoke();
	}
	#endregion
}
