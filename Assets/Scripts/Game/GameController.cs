using UnityEngine;
using Extensions;
using System;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using Services;
using Random = UnityEngine.Random;
using Home;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.UIElements;

namespace Game
{
	public class GameController : MonoBehaviour
	{
		private GameServices GameServices { get; set; }
		private PlayerService playerService;
		private AdsService adsService;
		private IAPService iapService;
		private AudioService audioService;
		private DisplayService displayService;
		private TrackingService trackingService;
		private InputService inputService;

		[SerializeField] private GameView view;
		[SerializeField] private GameModel model;
		[SerializeField] private AudioGame audioGame;

		[Space(1.5f)]
		[Header("REACT-TRANSFORM")]
		[SerializeField] private RectTransform canvas;
		[SerializeField] private RectTransform firstSlot;
		[SerializeField] private RectTransform showMap;
		[SerializeField] private RectTransform cardsRectTransform;
		[SerializeField] private RectTransform bgShowMap;
		[SerializeField] private RectTransform cardRect;
		[SerializeField] private RectTransform destroyCard;

		[Space(1.5f)]
		[Header("VFX / ANIMATION")]
		[SerializeField] private GameObject vfxCard;

		[Space(1.5f)]
		[Header("SCRIPT-OBJECT")]
		[SerializeField] private Cards cards;
		private LevelScriptableObject level;

		[Space(1.5f)]
		[Header("ANIMATOR")]
		[SerializeField] private Animator animatorBooster;
		private string currentBooster;
		private string[] boosterName = { "Idle", "Restore", "Revoke", "Shuffle" };

		//LIST
		private List<float> lstSlot = new(); //Chua cac vi tri tren bar -> bao gom 8 o
		private List<Card> listCardOnTheTray = new(); //Chua cac card tren bar
		private List<Card> tempCard = new();
		private List<Card> cardRemove = new();
		private List<Card> cardMoving = new();
		private List<TypeCard> listTypeCardInLevel = new();

		//ARRAY
		private float[] withinOneStar = new float[2];
		private float[] withinTwoStar = new float[2];
		private float[] withinThreeStar = new float[2];
		private float minScore, maxScore;

		private float countTimeShowInterAds = 0.0f;
		private float startTime = 0.0f;
		private float xLastSlot = 0.0f;
		private float heightCard = 0.0f;
		private float widthCard = 0.0f;
		private float fillScore, fillone, filltwo, fillthree;
		private float heightBanner = 0.0f;
		private float canvasScale = 0.0f;

		private int indexUndo = 0;
		private int indexRemove = 0;
		private int score = 0;
		private int comboPlus = 1;
		private int numberBonusBooster = 0;
		private int currentLevel = 0;

		//tracking
		private int x2 = 0;
		private int x3 = 0;
		private int x4 = 0;
		private int type0 = 0;
		private int type1 = 0;
		private int type2 = 0;
		private int type3 = 0;

		private bool isClicked = true;
		private bool isUndo = false;
		private bool isLoadRewardSuccess = false;
		private bool isGameOver = false;
		private bool isRevive = false;


		private int[] rankBooster = new int[4]; //booster in rank
		private int[] adventureBooster = new int[4]; //booster in adventure



		private string[] stateReason = { "popup_win_lose", "button_booster", "both", "null" };
		private string reason = string.Empty;

		private void Update()
		{
			if (iapService.IsRemoveAds() == false)
			{
				if (countTimeShowInterAds < model.MaxTimeShowInterAds)
				{
					if (inputService.GetTouches().Count > 0)
					{
						countTimeShowInterAds = 0;
					}
					countTimeShowInterAds += Time.deltaTime;
				}
				else
				{
					adsService.ShowLimitInterstitialAd();
					countTimeShowInterAds = 0;
				}
			}

			//if (Input.GetKeyDown(KeyCode.N))
			//{
			//	foreach (Transform child in cards.transform)
			//	{
			//		GameObject.Destroy(child.gameObject);
			//	}
			//	model.Star = StarLevel.none;
			//	score = (int)minScore;
			//	StartCoroutine(ClearedAllCard());
			//}
		}

		private void Awake()
		{
			view.ThrowIfNull();
			model.ThrowIfNull();
			cards.ThrowIfNull();
			canvas.ThrowIfNull();
			firstSlot.ThrowIfNull();
			showMap.ThrowIfNull();
			cards.ThrowIfNull();
			cardRect.ThrowIfNull();
			audioGame.ThrowIfNull();

			//Load Services
			if (GameObject.FindGameObjectWithTag(Constants.ServicesTag) == null)
			{
				SceneManager.LoadScene(Constants.EntryScene);
			}
			else
			{
				GameServices = GameObject.FindGameObjectWithTag(Constants.ServicesTag).GetComponent<GameServices>();
				playerService = GameServices.GetService<PlayerService>();
				adsService = GameServices.GetService<AdsService>();
				iapService = GameServices.GetService<IAPService>();
				audioService = GameServices.GetService<AudioService>();
				displayService = GameServices.GetService<DisplayService>();
				trackingService = GameServices.GetService<TrackingService>();
				inputService = GameServices.GetService<InputService>();
			}

			GetDataPassedFromHomeScene();
			audioGame.Initialized(audioService);
			audioService?.PlayMusic();

			//Check internet
			if (model.Mode == GameMode.Adventure)
			{
				if (Application.internetReachability == NetworkReachability.NotReachable)
				{
					view.OpenPopup(UIPopups.Notice);
					return;
				}
			}

			//Check load ads reward
			CheckHideButtonWatchAds(adsService.IsRewardedReady());
			adsService.OnRewardedAdsLoad = CheckHideButtonWatchAds;
		}

		#region AWAKE
		private void CheckHideButtonWatchAds(bool isReady)
		{
			isLoadRewardSuccess = isReady;
		}
		private void GetDataPassedFromHomeScene()
		{
			if (GameObject.FindGameObjectWithTag(Constants.ParamsTag) != null)
			{
				var paramsGo = GameObject.FindGameObjectWithTag(Constants.ParamsTag);
				var gameParameters = paramsGo.GetComponent<GameParameter>();

				//Get Mode
				model.Mode = gameParameters.OnGameMode;
				playerService.SetGameMode((int)model.Mode);
				if (model.Mode == GameMode.Rank)
				{
					playerService.SaveDataRankMode(0, rankBooster);
				}
				currentLevel = gameParameters.LoadLevel;
				playerService.SetCurrentLevel(currentLevel);
				Destroy(paramsGo);
			}
		}

		public void SetupSlotOnTheTray()
		{
			xLastSlot = firstSlot.rect.center.x;
			widthCard = cardRect.rect.width;
			heightCard = cardRect.rect.height;
			lstSlot.Add(xLastSlot);

			//Khoi tao vi tri list slot cho card on bar
			for (int i = 0; i < 10; i++)
			{
				xLastSlot += firstSlot.rect.size.x - 3;
				lstSlot.Add(xLastSlot);
			}
		}
		#endregion

		#region PARAMS START GAME
		private void LoadListCard()
		{
			for (int i = 0; i < ChangeListCard(); i++)
			{
				listTypeCardInLevel.Add(model.listAllTypeCard[i]);
			}
		}
		public int ChangeListCard()
		{
			int quantityCard = 0;
			switch (model.Mode)
			{
				case GameMode.Tutorial:
					quantityCard = model.amountImageTutorial[currentLevel];
					break;
				case GameMode.Adventure:
					if (currentLevel < model.amountImageNormal.Count)
						quantityCard = model.amountImageNormal[currentLevel];
					else
						quantityCard = model.amountImageNormal[model.amountImageNormal.Count - 1];
					break;
				case GameMode.Rank:
					quantityCard = 15;
					break;
			}
			return quantityCard;
		}
		#endregion

		private void Start()
		{
			canvasScale = canvas.localScale.x;

			CheckAds();

			startTime = Time.time;
			cards.PassFunc(MoveCardToBar, () => StartCoroutine(ClearedAllCard()));

			SetupSlotOnTheTray();

			currentLevel = playerService.GetCurrentLevel();
			model.Mode = (GameMode)playerService.GetGameMode();
			//CheckBonusBooster();
			StartGame();
		}

		private void CheckAds()
		{
			//Show banner
			if (!iapService.IsRemoveAds())
			{
				adsService.CreateMREC();
				adsService.ShowBannerAds();
				heightBanner = adsService.GetHeightBanner() / canvasScale;
				view.AvoidCutouts(displayService.SafeArea(), heightBanner);
			}
			else
			{
				view.AvoidCutouts(displayService.SafeArea());
			}
		}	

		private void StartGame()
		{
			view.GameScene.Initialize(model.Mode, model.TimeFillScore, model.Combo2, model.Combo3, model.Combo4, currentLevel + 1, score);

			//Anim booster
			currentBooster = boosterName[0];
			StartCoroutine(SetAnimBooster(currentBooster));

			//Get data booster
			if (model.Mode == GameMode.Rank)
			{
				score = playerService.GetScoreRankMode();
				rankBooster = playerService.GetDataRankMode();
				if (rankBooster[(int)Booster.slot] > 0) model.maxCardOnBar = 8;
				view.GameScene.ChangeQuantityBooster(rankBooster);
			}
			else if (model.Mode == GameMode.Adventure)
			{
				//trackingService.TrackingLevelStart((int)model.Mode, playerService.GetMaxStage(), playerService.GetUserDay(), currentLevel);
				adventureBooster = playerService.GetDataAdventureMode();
				view.GameScene.ChangeQuantityBooster(adventureBooster);
			}

			LoadListCard();

			model.dictCardClassification = new Dictionary<Type, Sprite>();
			for (int i = 0; i < model.listAllTypeCard.Count; i++)
			{
				model.dictCardClassification.Add(model.listAllTypeCard[i].typeCard, model.listAllTypeCard[i].spriteIcon);
			}

			LoadLevel(currentLevel);
		}

		#region CARDS ON THE BAR
		private void MoveCardToBar(Card card)
		{
			if (isClicked && !isGameOver)
			{
				audioGame.PlayPop();
				CheckListBar(card);
			}
		}

		private void CheckListBar(Card cardScript)
		{
			isUndo = true;
			if (listCardOnTheTray.Count == 0)
			{
				indexUndo = 0;
				listCardOnTheTray.Add(cardScript);
				MoveCard(0, cardScript);
			}
			else
			{
				int indexLast = listCardOnTheTray.FindLastIndex(x => x.Type == cardScript.Type);
				if (indexLast == -1) //Khong tim thay
				{
					listCardOnTheTray.Add(cardScript);
					indexUndo = listCardOnTheTray.Count - 1;
					MoveCard(listCardOnTheTray.Count - 1, cardScript);
				}
				else
				{
					if (cardMoving != null)
					{
						foreach (Card card in cardMoving)
						{
							DOTween.Kill(card);
						}
						cardMoving.Clear();
					}

					listCardOnTheTray.Insert(indexLast + 1, cardScript);
					indexUndo = indexLast + 1;
					cardRemove = listCardOnTheTray.FindAll(x => x.Type == cardScript.Type);
					if (cardRemove.Count > 2 && indexLast > 0) //3 card cung type
					{
						isUndo = false;
						for (int i = 0; i < 3; i++)
						{
							tempCard.Add(cardRemove[i]);
							if (i == 1)
							{
								StartCoroutine(ShowVFXDestroyCard(cardRemove[i].transform.position));
							}
						}
						cardRemove.Clear();
						listCardOnTheTray.RemoveRange(indexLast - 1, 3);
						MoveCard(indexLast, cardScript, true, true, () =>
						{
							for (int i = indexLast - 1; i < listCardOnTheTray.Count; i++)
							{
								listCardOnTheTray[i].transform.DOLocalMove(AdditionalCardPositionOnBar(i), model.timeBackCard / listCardOnTheTray.Count);
							}
							for (int i = 0; i < tempCard.Count; i++)
							{
								if (tempCard[i].transform.localScale != Vector3.zero)
								{
									tempCard[i].transform.DOScale(Vector2.zero, model.timeDestroyCard);
								}
							}
							StartCoroutine(ComboInTimeline());
							StartCoroutine(GameOver());
						});
						return;
					}
					else
					{
						cardRemove.Clear();
						if (indexLast + 1 == listCardOnTheTray.Count - 1)
						{
							MoveCard(listCardOnTheTray.Count - 1, cardScript);
						}
						else
						{
							cardMoving.Add(cardScript);
							MoveCard(indexLast + 1, cardScript, true);
						}
					}
				}
			}
		}

		private void MoveCard(int indexList, Card card, bool isBackCard = false, bool isRemove = false, Action onRemove = null)
		{
			card.transform.SetParent(firstSlot);
			card.transform.DOScale(Vector2.one, model.timeMovingCard);
			if (isBackCard)
			{
				if (isRemove)
				{
					//Lui card ve sau
					for (int i = indexList; i <= listCardOnTheTray.Count; i++)
					{
						listCardOnTheTray[i - 1].transform.DOLocalMove(AdditionalCardPositionOnBar(i + 2), model.timeBackCard / listCardOnTheTray.Count);
					}
					card.transform.DOLocalMove(AdditionalCardPositionOnBar(indexList + 1), model.timeMovingCard).OnComplete(() =>
					{
						onRemove?.Invoke();
					});
					return;
				}
				else
				{
					BackCardOnBar(indexList + 1);
					card.transform.DOLocalMove(AdditionalCardPositionOnBar(indexList), model.timeMovingCard);
					cardMoving.Remove(card);
					StartCoroutine(GameOver());
				}
			}
			else
			{
				card.transform.DOLocalMove(AdditionalCardPositionOnBar(indexList), model.timeMovingCard).OnComplete(() => cardMoving.Remove(card));
				StartCoroutine(GameOver());
			}
		}

		private Vector2 AdditionalCardPositionOnBar(int indexAdd)
		{
			return new Vector2(lstSlot[indexAdd], firstSlot.rect.center.y);
		}

		private void BackCardOnBar(int index)
		{
			for (int i = index; i < listCardOnTheTray.Count; i++)
			{
				listCardOnTheTray[i].transform.DOLocalMove(AdditionalCardPositionOnBar(i), model.timeBackCard);
			}
		}

		#endregion

		#region TUTORIAL
		private void FocusBooster()
		{
			if (currentLevel < model.levelsTutorial.Count - 1)
				cards.GetCardsForTutorial(currentLevel);
			FocusBooster(currentLevel);
		}


		#endregion

		#region HANDLE EACH LEVEL
		private void LoadLevel(int levelLoad)
		{
			isClicked = true;
			//Check mode to load level in game
			switch (model.Mode)
			{
				case GameMode.Tutorial:
					level = model.LevelTutorial(levelLoad);
					break;
				case GameMode.Adventure:
					numberBonusBooster = Random.Range(0, 3);
					level = model.LevelNormal(levelLoad);
					break;
				case GameMode.Rank:
					level = model.LevelRank(levelLoad);
					break;
			}
			MilestoneScore(level.Nodes.Count);
			SetupMapOnLevel();
			if (model.Mode == GameMode.Tutorial) FocusBooster();
		}

		private void SetupMapOnLevel()
		{
			float scaleMap = AdjustMap().Item1;
			float posYReturn = AdjustMap().Item2;
			float centerX = 0.0f;
			float centerY = 0.0f;
			cards.transform.localPosition = Vector2.zero;
			Vector2 pos = Vector2.zero;
			if (scaleMap != 1)
			{
				cards.transform.localScale = new Vector2(scaleMap, scaleMap);
				pos = level.rect.center * scaleMap;
			}
			if (level.rect.center != Vector2.zero)
			{
				if (level.rect.center.x < 0)
				{
					centerX = cardsRectTransform.localPosition.x + Mathf.Abs(pos.x);
				}
				if (level.rect.center.x > 0)
				{
					centerX = cardsRectTransform.localPosition.x - pos.x;
				}
				if (level.rect.center.y < 0)
				{
					centerY = cardsRectTransform.localPosition.y + Mathf.Abs(pos.y);
				}
				if (level.rect.center.y > 0)
				{
					centerY = cardsRectTransform.localPosition.y - pos.y;
				}
			}
			cardsRectTransform.localPosition = new Vector2(centerX, centerY);
			SetPositionRemoveCard(level.rect.center.x, level.rect.center.y - (level.rect.size.y / 2) - heightCard);
			cards.Initialize(level, listTypeCardInLevel, model.dictCardClassification);
		}	

		private IEnumerator ClearedAllCard()
		{
			isClicked = false;
			//Cleared all the cards
			//Check if you have completed all levels of that mode
			yield return new WaitForSeconds(model.timeMovingCard + model.timeDestroyCard + 0.1f);
			DOTween.KillAll();
			switch (model.Mode)
			{
				case GameMode.Tutorial:
					trackingService.TrackingTutorialStep(currentLevel, type0, type1, type2, type3);
					//CheckCompleteMode
					if (currentLevel == model.levelsTutorial.Count - 1)
					{
						playerService.SetGameMode((int)GameMode.Adventure);
						playerService.SetHighestLevel(0);
						view.GoToHome();
					}
					else
					{
						playerService.SetCurrentLevel(currentLevel + 1);
						playerService.SetHighestLevel(currentLevel);
						view.ReLoadScene();
					}
					break;
				case GameMode.Adventure:
					playerService.SetWinningStreak(playerService.GetWinningStreak() + 1);
					if (currentLevel == model.levelsNormal.Count - 1)
						StartCoroutine(CompleteLevel());
					else
					{
						if (playerService.GetHighestLevel() < currentLevel + 1)
							playerService.SetHighestLevel(currentLevel);
						StartCoroutine(CompleteLevel());
					}
					break;
				case GameMode.Rank:
					if (currentLevel == model.levelsRank.Count - 1)
					{
						view.OpenPopup(UIPopups.Username);
					}
					else
					{
						playerService.SetCurrentLevel(currentLevel + 1);
						playerService.SaveDataRankMode(score, rankBooster);
						if (!playerService.IsRate() && currentLevel > 1)
						{
							playerService.SetRate();
						}
						view.ReLoadScene();
					}
					break;
			}
		}

		private IEnumerator CompleteLevel()
		{
			if ((currentLevel + 1) % 5 == 0)
			{
				int giftBooster = Random.Range(0, adventureBooster.Length - 1);
				adventureBooster[giftBooster]++;
				playerService.SaveQuantityBoosterAdventure(adventureBooster);
				Vector2 posEnd = view.GameScene.positionQuantityBooster[giftBooster].GetComponent<RectTransform>().position;
				view.GiftBooster.ShowGiftBooster((Booster)giftBooster, posEnd, () =>
				{
					view.GameScene.ChangeQuantityBooster(adventureBooster);
				});
				view.OpenPopup(UIPopups.GiftBooster);
				yield return new WaitForSeconds(1.5f);
			}
			playerService.SaveQuantityBoosterAdventure(adventureBooster);
			audioGame.PlayWin();
			playerService.SetBestScoreByLevel(currentLevel, score);
			ShowMRECAds();
			view.ClearedPopups.ChangeDetailCompleteLevelPopup(true, model.Star, currentLevel, score, numberBonusBooster, isLoadRewardSuccess, currentLevel == model.levelsNormal.Count - 1);
			view.OpenPopup(UIPopups.Cleared);
			SaveStarOnLevel(true, (int)model.Star);
		}

		private void ShowMRECAds()
		{
			adsService.HideBannerAds();
			view.AvoidCutouts(displayService.SafeArea(), -heightBanner);
			if (iapService.IsRemoveAds() == false)
			{
				//float heightMREC = adsService.GetMRECLayout().y;
				float heightMREC = 500f;
				//300 x 250
				if (canvas.localScale.x > 1f)
					heightMREC *= canvas.localScale.x;
				else if (heightMREC < 1)
					heightMREC /= canvas.localScale.x;

				view.ClearedPopups.SetPositionPopup(heightMREC, canvas.localScale.x);
				adsService.ShowMRECAd();
			}
		}

		IEnumerator GameOver()
		{
			isClicked = false;
			if (listCardOnTheTray.Count >= model.maxCardOnBar) //full bar
			{
				isGameOver = true;
				cards.GameOver(true);
				audioService.Vibrate();
				view.GameScene.ShowVFXFullSlotOnBar(model.maxCardOnBar);
				foreach (Card card in listCardOnTheTray)
				{
					card.transform.DOScale(Vector2.zero, model.timeDestroyCard);
				}

				audioGame.PlayLose();
				yield return new WaitForSeconds(model.timeShowPopup);
				if (model.Mode == GameMode.Tutorial)
				{
					playerService.SetCurrentLevel(currentLevel);
					view.ReLoadScene();
				}
				else if (model.Mode == GameMode.Rank)
				{
					view.OpenPopup(UIPopups.Username);
				}
				else
				{
					playerService.SetCurrentLevel(currentLevel);
					//playerService.SetGameMode((int)model.Mode);
					yield return new WaitForSeconds(model.timeShowPopup);
					playerService.SetBestScoreByLevel(currentLevel, score);

					//Fail game
					view.ClearedPopups.ChangeDetailCompleteLevelPopup(false, StarLevel.none, currentLevel, score, numberBonusBooster, isLoadRewardSuccess);
					if (isLoadRewardSuccess && !isRevive)
					{
						isRevive = true;
						view.OpenPopup(UIPopups.Revive);
					}
					else
						view.OpenPopup(UIPopups.Cleared);

					SaveStarOnLevel(false, (int)model.Star); //Save level hien tai dang choi

					if (playerService.IsShowInterstitialAd() && iapService.IsRemoveAds() == false)
					{
						adsService.ShowLimitInterstitialAd();
					}
				}
			}
			else
			{
				isClicked = true;
			}
		}

		public void SaveStarOnLevel(bool isClear, int numberStar)
		{
			if (model.Mode == GameMode.Adventure)
			{
				trackingService.TrackingBooster(
					(int)model.Mode,
					(model.Mode == GameMode.Adventure) ? playerService.GetMaxStage() : -1,
					playerService.GetUserDay(),
					currentLevel,
					type0, type1, type2, type3,
					(isClear) ? "win" : "lose",
					(reason == string.Empty) ? stateReason[3] : reason
					);

				trackingService.TrackingScoreCombo(
					playerService.GetMaxStage(),
					playerService.GetUserDay(),
					x2, x3, x4, currentLevel
					);
				if (isClear)
				{
					trackingService.TrackLevelEnd(
						(int)model.Mode,
						playerService.GetMaxStage(),
						playerService.GetUserDay(),
						"win",
						currentLevel,
						playerService.GetMaxWinningStreak(),
						playerService.GetBestScoreByLevel(currentLevel),
						Time.time - startTime,
						0
						);
					if (playerService.GetHighestLevel() <= currentLevel)
					{
						playerService.SetStars(currentLevel, (byte)numberStar);
						if (currentLevel < model.levelsNormal.Count - 1)
						{
							playerService.SetHighestLevel(currentLevel + 1);
						}
						else
						{
							//Het map
							playerService.SetHighestLevel(currentLevel);
						}
						if (playerService.GetMaxStage() < currentLevel) playerService.SetMaxStage(currentLevel);
					}
					else
					{
						playerService.SetStars(currentLevel, (byte)numberStar);
					}
				}
				else
				{
					//Check chuoi thang cua user
					if (playerService.GetMaxWinningStreak() < playerService.GetWinningStreak())
					{
						playerService.SetMaxWinningStreak(playerService.GetWinningStreak());
					}
					playerService.SetWinningStreak(0);

					//Dem so card con lai tren map
					int countCards = 0;
					foreach (Transform child in cards.transform)
					{
						countCards++;
					}

					//trackingService.TrackLevelEnd(
					//(int)model.Mode,
					//playerService.GetMaxStage(),
					//playerService.GetUserDay(),
					//"lose",
					//currentLevel,
					//playerService.GetMaxWinningStreak(),
					//playerService.GetBestScoreByLevel(currentLevel),
					//Time.time - startTime,
					//countCards
					//);
				}
			}
		}

		public void OnButtonQuitToExitGame()
		{
			//SaveStarOnLevel(false, 0);
			if (model.Mode == GameMode.Tutorial)
			{
				//trackingService.TrackingTutorialStep(currentLevel, type0, type1, type2, type3);
			}
			else if (model.Mode == GameMode.Adventure)
			{
				//Dem so card con lai tren map
				int countCards = 0;
				foreach (Transform child in cards.transform)
				{
					countCards++;
				}

				//trackingService.TrackLevelEnd(
				//(int)model.Mode,
				//playerService.GetMaxStage(),
				//playerService.GetUserDay(),
				//"exit",
				//currentLevel,
				//playerService.GetMaxWinningStreak(),
				//playerService.GetBestScoreByLevel(currentLevel),
				//Time.time - startTime,
				//countCards
				//);

				//trackingService.TrackingBooster(
				//	(int)model.Mode,
				//	(model.Mode == GameMode.Adventure) ? playerService.GetMaxStage() : -1,
				//	playerService.GetUserDay(),
				//	currentLevel,
				//	type0, type1, type2, type3,
				//	"exit",
				//	(reason != string.Empty) ? stateReason[1] : stateReason[3]
				//	);
			}

			if (iapService.IsRemoveAds() == false)
				adsService.ShowLimitInterstitialAd();

			view.GoToHome();
		}
		#endregion

		#region COMBO TIMELINE, STAR
		IEnumerator ComboInTimeline()
		{
			yield return new WaitForSeconds(model.timeDestroyCard);
			score += 3 * comboPlus;
			view.GameScene.ShowScore(score);
			audioGame.PlayCombo();
			comboPlus = (comboPlus >= 4) ? 4 : comboPlus + 1;

			if (comboPlus == 2) x2++;
			else if (comboPlus == 3) x3++;
			else if (comboPlus == 4) x4++;

			ShowScoreAndStars(score);
			view.GameScene.ChangeTimeline(comboPlus, () =>
			{
				comboPlus = 1;
			});
		}

		private void ShowScoreAndStars(int score)
		{
			fillScore = score / maxScore;
			view.GameScene.FillScoreBar(fillScore);
			if (fillScore > fillthree)
			{
				if (model.Star != StarLevel.three)
				{
					model.Star = StarLevel.three;
					view.GameScene.GetStar();
				}

			}
			else if (fillScore > filltwo)
			{
				if (model.Star != StarLevel.two)
				{
					model.Star = StarLevel.two;
					view.GameScene.GetStar();
				}
			}
			else if (fillScore > fillone)
			{
				if (model.Star != StarLevel.one)
				{
					model.Star = StarLevel.one;
					view.GameScene.GetStar();
				}
			}
			else
			{
				model.Star = StarLevel.none;
			}
		}

		private void MilestoneScore(int min)
		{
			minScore = min;
			maxScore = (3 + 6 + 9) + (12 * (3 * (currentLevel + 2)));
			//Reset points star on level
			withinOneStar[0] = minScore;
			withinOneStar[1] = (minScore + maxScore) / 3;
			withinTwoStar[0] = withinOneStar[1] + 1;
			withinTwoStar[1] = Mathf.Floor((minScore + maxScore) / 2);
			withinThreeStar[0] = withinTwoStar[1] + 1;
			withinThreeStar[1] = maxScore;

			fillone = withinOneStar[0] / maxScore;
			filltwo = withinTwoStar[0] / maxScore;
			fillthree = withinThreeStar[0] / maxScore;

			float[] fill = { fillone, filltwo, fillthree };
			view.GameScene.SetPositionStar(fill);
		}
		#endregion

		#region BOOSTERS

		private void UseBooster(Booster booster, Action isUsed = null)
		{
			switch (booster)
			{
				case Booster.slot:
					type3++;
					model.maxCardOnBar++;
					view.GameScene.UnlockSlot();
					break;
				case Booster.remove:
					isUndo = false;
					if (listCardOnTheTray.Count == 0)
						return;
					else
					{
						type0++;
						isUsed?.Invoke();
						int numberCardRemove = 0;
						int i = 0;

						if (model.Mode != GameMode.Rank)
						{
							//Get pos remove
							if (indexRemove == 0)
							{
								i = indexRemove;
								indexRemove = 3;
								while (i < 3 && i < listCardOnTheTray.Count)
								{
									cards.ToolReturnCard(listCardOnTheTray[i], model.returnPosition[i]);
									numberCardRemove++;
									i++;
								}
							}
							else
							{
								i = indexRemove;
								indexRemove = 0;
								while (i < 6 && i < listCardOnTheTray.Count + 3)
								{
									cards.ToolReturnCard(listCardOnTheTray[i - 3], model.returnPosition[i]);
									numberCardRemove++;
									i++;
								}
							}
						}
						else
						{
							while (i < 3 && i < listCardOnTheTray.Count)
							{
								cards.ToolReturnCard(listCardOnTheTray[i], model.returnPosition[i]);
								numberCardRemove++;
								i++;
							}
						}

						listCardOnTheTray.RemoveRange(0, numberCardRemove);
						BackCardOnBar(0);
					}
					break;
				case Booster.undo:
					if (listCardOnTheTray.Count != 0 && isUndo)
					{
						type1++;
						isUsed?.Invoke();
						cards.ToolUndo(listCardOnTheTray[indexUndo]);
						listCardOnTheTray.RemoveAt(indexUndo);
						BackCardOnBar(indexUndo);
						isUndo = false;
					}
					break;
				case Booster.shuffle:
					type2++;
					isUsed?.Invoke();
					cards.Shuffle();
					break;
			}
			view.OpenPopup(UIPopups.Game);
		}

		#endregion

		#region SET POSITION CARDS, MAPS
		private void SetPositionRemoveCard(float posMapX, float posY)
		{
			//Reset the position when the card is removed by map
			if (model.Mode == GameMode.Rank)
			{
				model.returnPosition[0].x = posMapX - widthCard;
				model.returnPosition[1].x = posMapX;
				model.returnPosition[2].x = posMapX + widthCard;
			}
			else
			{
				float posX1 = posMapX - (widthCard + (widthCard / 2));
				model.returnPosition[0].x = posX1 - widthCard;
				model.returnPosition[1].x = posX1;
				model.returnPosition[2].x = posX1 + widthCard;
				float posX2 = posMapX + (widthCard + widthCard / 2);
				model.returnPosition[3].x = posX2 - widthCard;
				model.returnPosition[4].x = posX2;
				model.returnPosition[5].x = posX2 + widthCard;
			}

			for (int i = 0; i < model.returnPosition.Length; i++)
			{
				model.returnPosition[i].y = posY;
			}
		}

		private (float, float) AdjustMap(float posX = 0)
		{
			float areaMapWidth = showMap.rect.width;
			float areaMapHeight = showMap.rect.height;
			float sizeX = level.rect.size.x + widthCard;
			float sizeY = level.rect.size.y + heightCard;
			float distance = Vector2.Distance(new Vector2(0, bgShowMap.rect.y), new Vector2(0, showMap.rect.y));
			float scale = 1;
			if (sizeX < areaMapWidth && sizeY < areaMapHeight)
			{
				float posY = (bgShowMap.rect.yMin - distance) / scale + heightCard / 2;
				return (1.0f, posY);
			}
			else if (sizeX > (float)Screen.width && sizeY < areaMapHeight)
			{
				scale = areaMapWidth / sizeX;
				float posY = (bgShowMap.rect.yMin - distance) / scale + heightCard / 2;
				return (scale, posY);
			}
			else if (sizeX < (float)Screen.width && sizeY > areaMapHeight)
			{
				scale = areaMapHeight / sizeY;
				float posY = (bgShowMap.rect.yMin - distance) / scale + heightCard / 2;
				return (scale, posY);
			}
			else
			{
				float ratioMap = sizeX / sizeY;
				if (ratioMap < 1)
				{
					scale = areaMapHeight / sizeY;
					float posY = (bgShowMap.rect.yMin - distance) / scale + heightCard / 2;
					if (sizeX * scale > areaMapWidth)
					{
						return (areaMapWidth / sizeX, posY);
					}
					return (scale, posY);
				}
				else
				{
					scale = areaMapWidth / sizeX;
					float posY = ((bgShowMap.rect.yMin - distance) / scale) + (heightCard / 2);
					if (sizeY * scale > areaMapHeight)
					{
						scale = areaMapHeight / sizeY;
						posY = ((bgShowMap.rect.yMin - distance) / scale) + (heightCard / 2);
						return (scale, posY);
					}
					return (scale, posY);
				}
			}
		}
		#endregion

		#region VFX
		private IEnumerator ShowVFXDestroyCard(Vector2 pos)
		{
			yield return new WaitForSeconds(model.timeMovingCard);
			GameObject vfxDestroy = SimplePool.Spawn(vfxCard, pos, Quaternion.identity);
			vfxDestroy.transform.SetParent(destroyCard);
		}
		#endregion

		#region UI IN-GAME

		#region GAME-SCENE
		public void ButtonSettings()
		{
			audioGame.PlayButton1();
			view.OpenPopup(UIPopups.Settings);
			view.LoadSetting(playerService.GetMusicVolume(), playerService.GetSoundVolume());
		}

		public void ButtonRemove()
		{
			if (model.Mode == GameMode.Rank)
			{
				ControlRankBooster(Booster.remove);
			}
			else if (model.Mode == GameMode.Adventure)
			{
				ControlAdventureBooster(Booster.remove);
			}
			else
			{
				UseBooster(Booster.remove, () => { StartCoroutine(SetAnimBooster(boosterName[(int)Booster.remove])); });
			}
			view.GameScene.HideFocusParent(currentLevel);
		}

		public void ButtonUndo()
		{
			if (model.Mode == GameMode.Rank)
			{
				ControlRankBooster(Booster.undo);
			}
			else if (model.Mode == GameMode.Adventure)
			{
				ControlAdventureBooster(Booster.undo);
			}
			else
			{
				UseBooster(Booster.undo, () =>
				{
					StartCoroutine(SetAnimBooster(boosterName[(int)Booster.undo]));
				});
				view.GameScene.HideFocusParent(currentLevel);
			}
		}

		public void ButtonShuffle()
		{
			if (model.Mode == GameMode.Rank)
			{
				ControlRankBooster(Booster.shuffle);
			}
			else if (model.Mode == GameMode.Adventure)
			{
				ControlAdventureBooster(Booster.shuffle);
			}
			else
			{
				UseBooster(Booster.shuffle, () =>
				{
					StartCoroutine(SetAnimBooster(boosterName[(int)Booster.shuffle]));
				});
				view.GameScene.HideFocusParent(currentLevel);
			}
		}

		public void ButtonSlot()
		{
			if (model.Mode == GameMode.Rank)
			{
				ControlRankBooster(Booster.slot);
			}
			else if (model.Mode == GameMode.Adventure)
			{
				ControlAdventureBooster(Booster.slot);
			}
			else
			{
				UseBooster(Booster.slot, () =>
				{
					StartCoroutine(SetAnimBooster(boosterName[(int)Booster.slot]));
				});
			}
		}

		private void ControlRankBooster(Booster booster)
		{
			if (rankBooster[(int)booster] == 1)
			{
				UseBooster(booster, () =>
				{
					StartCoroutine(SetAnimBooster(boosterName[(int)booster]));
					rankBooster[(int)booster]++;
					view.GameScene.ChangeQuantityBooster(rankBooster);
				});
			}
			else
			{
				view.BoostersPopup.Initialize(model.detailBoosterPopups[(int)booster]);
				view.OpenPopup(UIPopups.Booster);
			}
		}

		private void ControlAdventureBooster(Booster booster)
		{
			if (adventureBooster[(int)booster] > 0)
			{
				UseBooster(booster, () =>
				{
					StartCoroutine(SetAnimBooster(boosterName[(int)booster]));
					adventureBooster[(int)booster]--;
					view.GameScene.ChangeQuantityBooster(adventureBooster);
				});
			}
			else
			{
				view.BoostersPopup.Initialize(model.detailBoosterPopups[(int)booster]);
				view.OpenPopup(UIPopups.Booster);
			}
		}

		private IEnumerator SetAnimBooster(string anim)
		{
			animatorBooster.ResetTrigger(currentBooster);
			currentBooster = anim;
			animatorBooster.SetTrigger(currentBooster);
			view.GameScene.Pause();
			yield return new WaitForSeconds(0.3f);
			view.GameScene.ContinueCombo();
		}
		#endregion

		#region QUIT-GAME
		public void ButtonYesQuit()
		{
			audioGame.PlayButton2();
			if (model.Mode == GameMode.Adventure) playerService.SaveQuantityBoosterAdventure(adventureBooster);
			OnButtonQuitToExitGame();
		}
		#endregion

		#region BOOSTER
		public void ButtonClaim(Booster booster)
		{
			audioGame.PlayButton1();
			switch (booster)
			{
				case Booster.slot:
					adsService.InitRewardedAd(() =>
					{
						UseBooster(booster);
						view.OpenPopup(UIPopups.Game);
					}, null);
					break;
				case Booster.remove:
					adsService.InitRewardedAd(() =>
					{
						if (model.Mode == GameMode.Rank)
						{
							rankBooster[(int)Booster.remove] += 1;
							view.GameScene.ChangeQuantityBooster(rankBooster);
						}
						else
						{
							adventureBooster[(int)Booster.remove] += 1;
							view.GameScene.ChangeQuantityBooster(adventureBooster);
						}
					}, null);
					break;
				case Booster.undo:
					adsService.InitRewardedAd(
						() =>
						{
							if (model.Mode == GameMode.Rank)
							{
								rankBooster[(int)Booster.undo] += 1;
								view.GameScene.ChangeQuantityBooster(rankBooster);
							}
							else
							{
								adventureBooster[(int)Booster.undo] += 1;
								view.GameScene.ChangeQuantityBooster(adventureBooster);
							}
						}, null);
					break;
				case Booster.shuffle:
					adsService.InitRewardedAd(() =>
					{
						if (model.Mode == GameMode.Rank)
						{
							rankBooster[(int)Booster.shuffle] += 1;
							view.GameScene.ChangeQuantityBooster(rankBooster);
						}
						else
						{
							adventureBooster[(int)Booster.shuffle] += 1;
							view.GameScene.ChangeQuantityBooster(adventureBooster);
						}
					}, null);
					break;
			}
			view.OpenPopup(UIPopups.Game);
			adsService.ShowRewardedAd();
		}
		#endregion

		#region USERNAME

		#endregion

		#region SETTINGS
		public void ButtonX()
		{
			audioGame.PlayButton2();
			view.OpenPopup(UIPopups.Game);
		}
		public void ButtonQuit()
		{
			audioGame.PlayButton1();
			view.OpenPopup(UIPopups.Quit);
		}
		public void OpenTerm()
		{
			audioGame.PlayButton1();
			Application.OpenURL("https://abigames.com.vn/ct-terms-of-service/");
		}
		public void OpenPrivacy()
		{
			audioGame.PlayButton1();
			Application.OpenURL("https://abigames.com.vn/policy/");
		}
		public void SaveMusicVolume(float volume)
		{
			audioGame.PlayButton1();
			playerService.SetMusicVolume(volume);
			audioService.SetMusicVolume(volume);
		}
		public void SaveSoundVolume(float volume)
		{
			audioGame.PlayButton1();
			playerService.SetSoundVolume(volume);
			audioService.SetSoundVolume(volume);
		}
		#endregion

		#region COMPLETE POPUP
		public void ButtonGetBoosterAdsRewars(Booster bonusBooster)
		{
			adsService.InitRewardedAd(() =>
			{
				adventureBooster[(int)bonusBooster]++;
				playerService.SaveQuantityBoosterAdventure(adventureBooster);
				view.GameScene.ChangeQuantityBooster(adventureBooster);
				//tracking
				if (reason == string.Empty)
					reason = stateReason[0];
				else
					reason = stateReason[2];
			}, () => { view.GoToHome(); });

			adsService.ShowRewardedAd();
		}
		public void ButtonReplayLevel()
		{
			audioGame.PlayButton1();
			if (playerService.IsShowInterstitialAd() && iapService.IsRemoveAds() == false)
				adsService.ShowLimitInterstitialAd();

			view.ReLoadScene();
		}

		public void ButtonNextLevel()
		{
			audioGame.PlayButton1();
			playerService.SetCurrentLevel(currentLevel + 1);
			if (playerService.IsShowInterstitialAd() && iapService.IsRemoveAds() == false)
				adsService.ShowLimitInterstitialAd();

			view.ReLoadScene();
		}
		#endregion

		#region REVIVE
		public void ButtonXRevive()
		{
			ShowMRECAds();
			view.OpenPopup(UIPopups.Cleared);
		}

		public void ButtonRevive()
		{
			adsService.InitRewardedAd(() =>
			{
				view.GameScene.ContinueCombo();
				for (int i = 0; i < Mathf.CeilToInt((float)model.maxCardOnBar / 3); i++)
				{
					UseBooster(Booster.remove);
				}
				view.OpenPopup(UIPopups.Game);
				isGameOver = false;
				isClicked = true;
				cards.GameOver(isGameOver);
			}, () => { ButtonXRevive(); });
			adsService.ShowRewardedAd();

		}
		#endregion

		#region DESCRIBE-BOOSTER
		public void FocusBooster(int levelTutorial)
		{
			view.GameScene.FocusOnButtonBooster(levelTutorial);
			view.DescibeBooster.ChangeContentDescibeBooster(levelTutorial);
			view.OpenPopup(UIPopups.Describe);
		}
		#endregion

		#region NOTICE

		#endregion

		#endregion
	}
}
