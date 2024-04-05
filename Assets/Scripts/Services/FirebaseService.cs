using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using Game;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseService
{

	private const string Break = "~";

	// Key save data to player prefs
	private const string showAppOpenAdKey = "saoa";
	private const string limitedTimeAdsKey = "lta";

	// Name key from firebase
	private const string nameShowAppOpenAd = "show_app_open_ad";
	private const string nameLimitedTimeAds = "limited_time_ads";

	// Action
	public Action OnFetchSuccess;
	public Action<bool> OnShowAppOpenAdChange;
	public Action<int> OnLimitTimeAdsChanged;

	private FirebaseApp firebaseApp;

	// Cache
	private bool isShowAppOpenAd = false;
	private int lmtTimeAds = 70;
	public FirebaseService(Action onFetchSuccess)
	{
		OnFetchSuccess = onFetchSuccess;

		_ = InitFirebaseAsync();
	}
	private async Task InitFirebaseAsync()
	{
		await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
		{
			var dependencyStatus = task.Result;
			if (dependencyStatus == DependencyStatus.Available)
			{
#if UNITY_EDITOR
				firebaseApp = FirebaseApp.Create();
#else
				firebaseApp = FirebaseApp.DefaultInstance;
#endif
				InitRemoteConfig();
			}
			else
			{
				Logger.Error(
				  "Could not resolve all Firebase dependencies: " + dependencyStatus);
			}
		});

	}
	public void InitRemoteConfig()
	{
		// Set default values
		Dictionary<string, object> defaults = new()
		{
			{ nameShowAppOpenAd, false },
			{ nameLimitedTimeAds, 70 }
		};

		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			GetData();
			OnFetchSuccess?.Invoke();
			Logger.Debug("System is not connected to internet");
			return;
		}

		FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults).ContinueWithOnMainThread(task =>
		{
			_ = FetchDataAsync();
		});

	}
	private void GetData()
	{
		// AOA
		int tempAOA = PlayerPrefs.GetInt(showAppOpenAdKey, 0);
		isShowAppOpenAd = tempAOA != 0;
		OnShowAppOpenAdChange?.Invoke(isShowAppOpenAd);

		// limit time ad
		lmtTimeAds = PlayerPrefs.GetInt(limitedTimeAdsKey, 70);
		OnLimitTimeAdsChanged?.Invoke(lmtTimeAds);
	}
	public Task FetchDataAsync()
	{
		Logger.Debug("Fetching data...");
#if UNITY_DEBUG
		Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
#else
		Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.FromHours(12.0));
#endif
		return fetchTask.ContinueWithOnMainThread(FetchComplete);
	}
	private void FetchComplete(Task fetchTask)
	{
		if (fetchTask.IsCanceled)
		{
			GetData();
			OnFetchSuccess?.Invoke();
			Logger.Debug("Fetching Remote Config values was cancelled.");
			return;
		}
		if (fetchTask.IsFaulted)
		{
			GetData();
			OnFetchSuccess?.Invoke();
			Logger.Debug("Fetching Remote Config values encountered an error: " + fetchTask.Exception);
			return;
		}
		if (fetchTask.IsCompletedSuccessfully == false)
		{
			GetData();
			OnFetchSuccess?.Invoke();
			Logger.Debug("Fetching Failed.");
			return;
		}

		var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
		var info = remoteConfig.Info;
		if (info.LastFetchStatus != LastFetchStatus.Success)
		{
			GetData();
			OnFetchSuccess?.Invoke();
			Logger.Error($"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
			return;
		}

		remoteConfig.ActivateAsync()
		  .ContinueWithOnMainThread(
			task =>
			{
				Logger.Debug($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");

				// limit time ad
				lmtTimeAds = (int)remoteConfig.GetValue(nameLimitedTimeAds).LongValue;
				PlayerPrefs.SetInt(limitedTimeAdsKey, lmtTimeAds);
				OnLimitTimeAdsChanged?.Invoke(lmtTimeAds);

				// AOA
				isShowAppOpenAd = remoteConfig.GetValue(nameShowAppOpenAd).BooleanValue;
				PlayerPrefs.SetInt(showAppOpenAdKey, isShowAppOpenAd == true ? 1 : 0);
				OnShowAppOpenAdChange?.Invoke(isShowAppOpenAd);

				OnFetchSuccess?.Invoke();
			});
	}
	private void OnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo impressionData)
	{
		double revenue = impressionData.Revenue;
		var impressionParameters = new[] {
			new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
			new Firebase.Analytics.Parameter("ad_source", impressionData.NetworkName),
			new Firebase.Analytics.Parameter("ad_unit_name", impressionData.AdUnitIdentifier),
			new Firebase.Analytics.Parameter("ad_format", impressionData.AdFormat),
			new Firebase.Analytics.Parameter("value", revenue),
			new Firebase.Analytics.Parameter("currency", "USD"),
		};
		FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);

		var impressionAbiParameters = new[] {
			new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
			new Firebase.Analytics.Parameter("ad_source", impressionData.NetworkName),
			new Firebase.Analytics.Parameter("ad_unit_name", impressionData.AdUnitIdentifier),
			new Firebase.Analytics.Parameter("ad_format", impressionData.AdFormat),
			new Firebase.Analytics.Parameter("value", revenue),
			new Firebase.Analytics.Parameter("currency", "USD"),
		};
		FirebaseAnalytics.LogEvent("ad_impression_abi", impressionAbiParameters);
	}

	/// <summary>
	/// Log event when exit game.
	/// </summary>
	/// <param name="gamePlayed"></param>
	/// <param name="timePlayed"></param>
	public void SelectItem(long gamePlayed, double timePlayed)
	{
		var selectItem = new[]
			{
				new Parameter("button_exit", gamePlayed),
				new Parameter("time_played", Convert.ToInt64(timePlayed)),
			};
		FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventSelectItem, selectItem);
	}
	/// <summary>
	/// Log event when complete game
	/// </summary>
	/// <param name="gamePlayed"></param>
	/// <param name="timePlayed"></param>
	/// <param name="mode"></param>
	public void LevelComplete(long gamePlayed, double timePlayed, GameMode mode)
	{
		var levelComplete = new[]
			{
				new Parameter(FirebaseAnalytics.ParameterLevelName, gamePlayed),
				new Parameter(FirebaseAnalytics.ParameterLevel, (long)mode),
				new Parameter("time_played", Convert.ToInt64(timePlayed)),
			};
		FirebaseAnalytics.LogEvent("level_complete", levelComplete);
	}
	/// <summary>
	/// Log event when fail game.
	/// </summary>
	/// <param name="gamePlayed"></param>
	/// <param name="timePlayed"></param>
	/// <param name="mode"></param>
	public void LevelFail(long gamePlayed, double timePlayed, GameMode mode)
	{
		var levelFail = new[]
			{
				new Parameter(FirebaseAnalytics.ParameterLevelName, gamePlayed),
				new Parameter(FirebaseAnalytics.ParameterLevel, (long)mode),
				new Parameter("time_played", Convert.ToInt64(timePlayed)),
			};
		FirebaseAnalytics.LogEvent("level_fail", levelFail);
	}

	/* DOGGO GO */
	public void TutorialStep(long levelTutorial, long type0, long type1, long type2, long type3)
	{
		var tutorialStep = new[]
			{
				new Parameter("step", levelTutorial),
				new Parameter("type_0", type0),
				new Parameter("type_1", type1),
				new Parameter("type_2", type2),
				new Parameter("type_3", type3),
			};
		FirebaseAnalytics.LogEvent("tutorial_step", tutorialStep);
	}

	public void LevelStart(long gameMode, long maxStage, long userDay, long stage)
	{
		var levelStart = new[]
			{
				new Parameter("game_mode", gameMode),
				new Parameter("max_stage", maxStage),
				new Parameter("user_day", userDay),
				new Parameter("stage", stage),

				//new Parameter(FirebaseAnalytics.ParameterLevelName, gamePlayed),
				//new Parameter(FirebaseAnalytics.ParameterLevel, (long)mode),
			};
		FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart, levelStart);
	}

	public void LevelEnd(long gameMode, long maxStage, long userDay, string result, long stage, long maxStreakLong, long totalScore, double completeTime, long numberRemaining)
	{
		var levelEnd = new[]
		{
			new Parameter("game_mode", gameMode),
			new Parameter("max_stage", maxStage),
			new Parameter("user_day", userDay),
			new Parameter("result", result),
			new Parameter("stage", stage),
			//new Parameter("play_time", Convert.ToInt64(playTime)),
			new Parameter("max_streak_long", maxStreakLong),
			new Parameter("total_score", totalScore),
			new Parameter("complete_time", Convert.ToInt64(completeTime)),
			new Parameter("number_remaining", numberRemaining),

		};
		FirebaseAnalytics.LogEvent("level_end", levelEnd);
	}

	public void Booster(long gameMode, long maxStage, long userDay, long stage, long type0, long type1, long type2, long type3, string result, string reason)
	{
		var booster = new[]
		{
			new Parameter("game_mode", gameMode),
			new Parameter("max_stage", maxStage),
			new Parameter("user_day", userDay),
			new Parameter("stage", stage),
			new Parameter("type_0", type0),
			new Parameter("type_1", type1),
			new Parameter("type_2", type2),
			new Parameter("type_3", type3),
			new Parameter("result", result),
			new Parameter("reason", reason),
		};
		FirebaseAnalytics.LogEvent("booster", booster);
	}

	public void ScoreCombo(long maxStage, long userDay, long x2, long x3, long x4, long stage)
	{
		var scoreCombo = new[]
		{
			new Parameter("max_stage", maxStage),
			new Parameter("user_day", userDay),
			new Parameter("level_x2", x2),
			new Parameter("level_x3", x3),
			new Parameter("level_x4", x4),
			new Parameter("stage", stage),
		};
		FirebaseAnalytics.LogEvent("score_combo", scoreCombo);
	}
	public void TransactionComplete(long maxStage, long userDay, string offerName, string offerPrice, string transactionPlacement)
	{
		var transactionComplete = new[]
		{
			new Parameter("max_stage", maxStage),
			new Parameter("user_day", userDay),
			new Parameter("offer_name", offerName),
			new Parameter("offer_price", offerPrice),
			new Parameter("transaction_placement", transactionPlacement),
		};
		FirebaseAnalytics.LogEvent("transaction_complete", transactionComplete);
	}
}
