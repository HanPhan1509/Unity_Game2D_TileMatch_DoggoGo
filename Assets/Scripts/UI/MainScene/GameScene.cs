using DG.Tweening;
using Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game
{
	public class GameScene : MonoBehaviour
	{
		[SerializeField] private GameObject areaLevel;
		[SerializeField] private Transform[] stars;
		[SerializeField] private RectTransform fillScore;
		[SerializeField] private Image fillTimeline;
		[Space(0.8f)]
		[Header("TEXT BOOSTER")]
		[SerializeField] private TextMeshProUGUI[] textQuantityBooster;

		[Space(0.8f)]
		[Header("BUTTON BOOSTER")]
		[SerializeField] private Transform focusParent;
		[SerializeField] private Transform boosterArea;
		[SerializeField] private Button[] buttonBooster;
		[SerializeField] private Animator[] animatorBooster;
		[SerializeField] private GameObject[] areaQuantityBooster;
		public GameObject[] positionQuantityBooster { get => areaQuantityBooster; }

		[Space(0.8f)]
		[Header("FULL SLOT")]
		[SerializeField] private GameObject full7Slot;
		[SerializeField] private GameObject full8Slot;

		[Space(0.8f)]
		[Header("COMBO - STAR")]
		[SerializeField] private GameObject[] combo;
		[SerializeField] private GameObject starArea;


		[Space(0.8f)]
		[Header("OTHERS")]
		[SerializeField] private TextMeshProUGUI txtScore;
		//[SerializeField] private TextMeshProUGUI comboPlus;
		[SerializeField] private TextMeshProUGUI txtCurrentLevel;

		[Space(1.5f)]
		[Header("ACTION-BUTTON")]
		[SerializeField] private UnityEvent OnButtonSetting;
		[SerializeField] private UnityEvent OnButtonRemove;
		[SerializeField] private UnityEvent OnButtonUndo;
		[SerializeField] private UnityEvent OnButtonShuffle;
		[SerializeField] private UnityEvent OnButtonSlot;


		private int numberStar = -1;
		private float fillMax = 300.0f;
		private float posY;
		private float posX = -150;
		private float currentfill, targetfill, timeFill;
		private int combo2, combo3, combo4;
		private GameMode GameMode;

		private void Awake()
		{
			areaLevel.ThrowIfNull();
			fillScore.ThrowIfNull();
			fillTimeline.ThrowIfNull();
			focusParent.ThrowIfNull();
			boosterArea.ThrowIfNull();
			txtScore.ThrowIfNull();
			txtCurrentLevel.ThrowIfNull();
		}

		public void Initialize(GameMode GameMode, float timeFillScore, int combo2, int combo3, int combo4, int level, int score)
		{
			this.GameMode = GameMode;
			this.timeFill = timeFillScore;
			this.combo2 = combo2;
			this.combo3 = combo3;
			this.combo4 = combo4;
			starArea.SetActive(GameMode == GameMode.Rank ? false : true);
			areaLevel.SetActive(GameMode != GameMode.Tutorial ? true : false);
			if(GameMode == GameMode.Tutorial)
			{
				for(int i = 0; i < buttonBooster.Length - 1;  i++)
				{
					buttonBooster[i].gameObject.SetActive(false);
				}	
			}	
			this.txtCurrentLevel.text = level.ToString();
			this.txtScore.text = score.ToString();
			ResetLevel();
		}

		public void ResetLevel()
		{
			foreach (var s in stars)
			{
				s.gameObject.SetActive(false);
			}
			numberStar = -1;
			currentfill = 0;
			fillScore.sizeDelta = new Vector2(0, fillScore.sizeDelta.y);
			ResetTimeline();
			posY = fillScore.localPosition.y;
			ShowCombo(1);
			full7Slot.SetActive(false);
			full8Slot.SetActive(false);
		}

		private void Update()
		{
			if (currentfill < targetfill)
			{
				currentfill += timeFill;
				if (currentfill > targetfill)
				{
					currentfill = targetfill;
				}
				fillScore.sizeDelta = new Vector2(currentfill * fillMax, 32f);
				fillScore.localPosition = new Vector2(posX + currentfill * fillMax / 2f, posY);
			}
		}

		#region ON SCREEN

		public void ShowScore(int score)
		{
			this.txtScore.text = score.ToString();
		}
		#endregion

		#region STARS
		public void SetPositionStar(float[] fill)
		{
			for (int i = 0; i < this.stars.Length; i++)
			{
				stars[i].transform.localPosition = new Vector2(posX + fill[i] * fillMax, stars[i].localPosition.y);
			}
		}
		public void GetStar()
		{
			numberStar++;
			stars[numberStar].gameObject.SetActive(true);
		}

		public void FillScoreBar(float fill)
		{
			targetfill = fill;
		}
		#endregion

		#region COMBO
		public void ContinueCombo()
		{
			DOTween.Play(fillTimeline);
		}

		public void ResetTimeline()
		{
			fillTimeline.fillAmount = 0;
			DOTween.Kill(fillTimeline);
		}

		public void Pause()
		{
			DOTween.Pause(fillTimeline);
		}

		public void ChangeTimeline(int numberCombo = 1, Action onEnded = null)
		{
			DOTween.Kill(fillTimeline);
			ShowCombo(numberCombo);
			fillTimeline.fillAmount = 1;
			if (numberCombo == 2)
			{
				fillTimeline.DOFillAmount(0, combo2).SetEase(Ease.Linear).OnComplete(() =>
				{
					ShowCombo(1);
					onEnded?.Invoke();
				});
			}
			else if (numberCombo == 3)
			{
				fillTimeline.DOFillAmount(0, combo3).SetEase(Ease.Linear).OnComplete(() =>
				{
					ShowCombo(1);
					onEnded?.Invoke();
				});
			}
			else
			{
				fillTimeline.DOFillAmount(0, combo4).SetEase(Ease.Linear).OnComplete(() =>
				{
					ShowCombo(1);
					onEnded?.Invoke();
				});
			}
		}

		private void ShowCombo(int number)
		{
			foreach (var cb in combo)
			{
				cb.SetActive(false);
			}
			combo[number - 1].SetActive(true);
		}
		#endregion

		#region BOOSTETS

		public void HideFocusParent(int levelTutorial)
		{
			buttonBooster[levelTutorial].transform.SetParent(boosterArea);
			focusParent.gameObject.SetActive(false);
		}

		public void FocusOnButtonBooster(int levelTutorial)
		{
			for(int i = 0; i <= levelTutorial; i++)
			{
				buttonBooster[i].gameObject.SetActive(true);
			}
			focusParent.gameObject.SetActive(true);
			buttonBooster[levelTutorial].transform.SetParent(focusParent);
			animatorBooster[levelTutorial].SetTrigger("Click");
		}

		public void ChangeQuantityBooster(int[] quantityBooster)
		{
			if (GameMode == GameMode.Adventure)
			{
				for (int i = 0; i < textQuantityBooster.Length; i++)
				{
					textQuantityBooster[i].text = quantityBooster[i] == 0 ? "+" : quantityBooster[i].ToString();
				}
			}
			else
			{
				for (int i = 0; i < quantityBooster.Length; i++)
				{
					if(i < quantityBooster.Length - 1)
					{
						string textQuantity = string.Empty;
						if (quantityBooster[i] == 0)
							textQuantity = "+";
						else if (quantityBooster[i] == 1)
							textQuantity = 1.ToString();
						else
							TurnOffInteractableButton((Booster)i);

						textQuantityBooster[i].text = textQuantity;
					} else
					{
						if (quantityBooster[i] > 0)
						{
							UnlockSlot();
						}	
					}
				}
			}
		}

		public void TurnOffInteractableButton(Booster booster)
		{
			areaQuantityBooster[(int)booster].SetActive(false);
			buttonBooster[(int)booster].interactable = false;
		}

		public void UnlockSlot()
		{
			buttonBooster[(int)Booster.slot].gameObject.SetActive(false);
		}
		#endregion

		#region CARDS BAR
		public void ShowVFXFullSlotOnBar(int maxCard)
		{
			full8Slot.SetActive(false);
			full7Slot.SetActive(false);
			if(maxCard > 7) full8Slot.SetActive(true);
			else full7Slot.SetActive(true);
		}
		#endregion

		#region ACTION-BUTTON
		public void ButtonSetting()
		{
			OnButtonSetting?.Invoke();
		}

		public void ButtonRemove()
		{
			OnButtonRemove?.Invoke();
		}

		public void ButtonUndo()
		{
			OnButtonUndo?.Invoke();
		}

		public void ButtonShuffle()
		{
			OnButtonShuffle?.Invoke();
		}

		public void ButtonSlot()
		{
			OnButtonSlot?.Invoke();
			if (GameMode == GameMode.Tutorial)
				UnlockSlot();
		}
		#endregion
	}

}
