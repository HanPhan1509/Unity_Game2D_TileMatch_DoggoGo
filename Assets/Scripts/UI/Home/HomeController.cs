using Audio;
using Extensions;
using Game;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;
using UnityEngine.UI.Extensions;

namespace Home
{
	public class HomeController : MonoBehaviour
	{
		//Service
		private GameServices GameServices { get; set; }
		private GameService gameService;
		private PlayerService playerService;
		private DisplayService displayService;
		private AdsService adsService;
		private IAPService iapService;
		private AudioService audioService;
		private TrackingService trackingService;
		private InputService inputService;

		[Header("PANEL PREFERENCE")]
		[SerializeField] private HomeView view;
		[SerializeField] private HomeModel model;
		[SerializeField] private AudioGame audioGame;
		[SerializeField] private RectTransform canvasRectTrasform;

		private float countTimeShowInterAds = 0.0f;

		private void Awake()
		{
			//Load Services
			if (GameObject.FindGameObjectWithTag(Constants.ServicesTag) == null)
			{
				SceneManager.LoadScene(Constants.EntryScene);
			}
			else
			{
				GameServices = GameObject.FindGameObjectWithTag(Constants.ServicesTag).GetComponent<GameServices>();
				playerService = GameServices.GetService<PlayerService>();
				displayService = GameServices.GetService<DisplayService>();
				adsService = GameServices.GetService<AdsService>();
				iapService = GameServices.GetService<IAPService>();
				audioService = GameServices.GetService<AudioService>();
				trackingService = GameServices.GetService<TrackingService>();
				gameService = GameServices.GetService<GameService>();
				inputService = GameServices.GetService<InputService>();
			}

			view.HomeScene.AvoidCutouts(displayService.SafeArea());
			audioGame.Initialized(audioService);
			audioService?.PlayMusic();
			adsService.HideBannerAds();
			adsService.HideMRECAds();

			model.numberLevel = playerService.GetHighestLevel();
			model.lstSaveStar = playerService.GetListStars();
		}

		private void Start()
		{
			ClosePopups();
			view.HomeScene.Initialize((GameMode)playerService.GetGameMode(), iapService.IsRemoveAds());

			TotalStarsOnMap();

			if (playerService.IsRate())
			{
				if (gameService.CanShowRate())
				{
					OpenRateUs();
				}
			}
			SetDate();
		}

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
		}

		private void SetDate()
		{
			long sToday = DateTime.Today.Ticks;
			long sDate = playerService.GetDate();
			if (sDate == 0)
			{
				playerService.SetDate(sToday);
				playerService.SetUserDay(playerService.GetUserDay() + 1); //Output: first day = 1
			}
			else
			{
				if (sDate > sToday)
				{
					var countDay = sDate - sToday;
					playerService.SetDate(sToday);
					playerService.SetUserDay(playerService.GetUserDay() + countDay / 1000 / 60 / 60 / 24 / 10000);
					if (iapService.IsRemoveAds())
					{
						view.OpenPopup(UIPopups.DailyGift);
					}
				}
			}
		}

		private void TotalStarsOnMap()
		{
			int total = 3 * model.totalLevelNormal;
			int number = 0;
			foreach (var n in model.lstSaveStar)
			{
				number += (int)n;
			}
			view.HomeScene.SetTotalStarsOnMap(number, total);
			view.LevelSelect.SetTotalStarsOnMap(number, total);
		}

		#region OPEN POPUPS
		public void OpenSetting()
		{
			audioGame.PlayButton1();
			view.SettingsPopup.LoadSetting(playerService.GetMusicVolume(), playerService.GetSoundVolume());
			view.OpenPopup(UIPopups.Settings);
		}

		public void ClosePopups()
		{
			audioGame.PlayButton2();
			view.OpenPopup(UIPopups.Main);
		}

		public void OpenLeaderBoard()
		{
			audioGame.PlayButton1();
			view.OpenPopup(UIPopups.LeaderBoard);
		}
		public void OpenRemoveAds()
		{
			audioGame.PlayButton1();
			view.RemoveAdsPopup.ChangePrice(iapService.GetPrice());
			view.OpenPopup(UIPopups.RemoveAds);
		}
		public void OpenCollection()
		{
			audioGame.PlayButton1();
			view.OpenPopup(UIPopups.Collection);
		}
		public void OpenRateUs()
		{
			view.OpenPopup(UIPopups.Rateus);
		}
		#endregion

		#region SETTINGS POPUP
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

		#region MAIN SCREEN
		public void ButtonPlay()
		{
			audioGame.PlayButton1();

			if ((GameMode)playerService.GetGameMode() == GameMode.Tutorial)
			{
				PassModeTutorial();
			}
			else
			{
				//Mode Adventure
				PassModeNormal(playerService.GetHighestLevel());
			}
			SceneManager.LoadScene("Game");
		}

		public void ButtonSelectLevel()
		{
			audioGame.PlayButton1();

			//Load items
			for (int i = 0; i < model.totalLevelNormal; i++)
			{
				ItemMapInfo item = new();
				item.idMap = i;
				if (i < model.lstSaveStar.Count)
				{
					item.numberStar = (int)model.lstSaveStar[i];
				}
				else if (i == model.lstSaveStar.Count) //highest level
				{
					item.numberStar = 0;
				}
				else
				{
					item.numberStar = -1;
				}
				model.lstMap.Add(item);
			}
			view.LevelSelect.Initialized(canvasRectTrasform.localScale.x, playerService.GetHighestLevel(), model.lstMap, ButtonMapItems);
			view.OpenPopup(UIPopups.LevelSelect);
		}

		public void ButtonRanked()
		{
			audioGame.PlayButton1();
			PassModeRanked();
			SceneManager.LoadScene("Game");
		}

		#endregion

		#region POPUP SELECT LEVEL
		public void ButtonMapItems(int numberLevel)
		{
			audioGame.PlayButton1();
			PassModeNormal(numberLevel);
			SceneManager.LoadScene("Game");
		}

		#region PASS DATA
		private void PassModeTutorial()
		{
			if (GameObject.FindGameObjectWithTag(Constants.ParamsTag) == null)
			{
				GameObject paramObject = new GameObject(nameof(GameParameter));
				paramObject.tag = Constants.ParamsTag;
				GameParameter gameParameters = paramObject.AddComponent<GameParameter>();
				gameParameters.OnGameMode = GameMode.Tutorial;
				gameParameters.LoadLevel = playerService.GetCurrentLevel();
			}
		}
		private void PassModeNormal(int numberLevel)
		{
			if (GameObject.FindGameObjectWithTag(Constants.ParamsTag) == null)
			{
				GameObject paramObject = new GameObject(nameof(GameParameter));
				paramObject.tag = Constants.ParamsTag;
				GameParameter gameParameters = paramObject.AddComponent<GameParameter>();
				gameParameters.OnGameMode = GameMode.Adventure;
				gameParameters.LoadLevel = numberLevel;
			}
		}

		private void PassModeRanked()
		{
			if (GameObject.FindGameObjectWithTag(Constants.ParamsTag) == null)
			{
				GameObject paramObject = new GameObject(nameof(GameParameter));
				paramObject.tag = Constants.ParamsTag;
				GameParameter gameParameters = paramObject.AddComponent<GameParameter>();
				gameParameters.OnGameMode = GameMode.Rank;
				gameParameters.LoadLevel = 0;
			}
		}
		#endregion
		#endregion

		#region POPUP REMOVE ADS
		public void ButtonPayRemoveAds()
		{
			audioGame.PlayButton1();
			iapService.PurchaseRemoveAds(OnPurchaseComplete);
		}

		private void OnPurchaseComplete(bool success)
		{
			if (success == true)
			{
				view.HomeScene.HideRemoveAdsButton();
				view.OpenPopup(UIPopups.Main);
				trackingService.TrackTransactionComplete(
					playerService.GetMaxStage(),
					playerService.GetUserDay(),
					"remove_ads",
					iapService.GetPrice(),
					"remove_ads_popup"
					);
				view.OpenPopup(UIPopups.DailyGift);
			}
			else
			{
				Logger.Debug("Purchase failed.");
			}
		}
		#endregion

		#region POPUP RATE US
		public void ButtonYesToRate(int star)
		{
			audioGame.PlayButton1();
			if (star > 3)
			{
				gameService.Rate();
			}
			ClosePopups();
		}
		#endregion

		#region POPUP DAILY GIFT
		public void ButtonClaim()
		{
			audioGame.PlayButton1();
			int[] quantityBooster = playerService.GetDataAdventureMode();
			for (int i = 0; i < quantityBooster.Length - 1; i++)
			{
				quantityBooster[i]++;
			}
			playerService.SaveQuantityBoosterAdventure(quantityBooster);
			view.OpenPopup(UIPopups.Main);
		}
		#endregion
	}

}
