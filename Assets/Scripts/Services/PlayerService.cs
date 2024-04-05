using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Services
{
	public class PlayerService
	{
		/// <summary>
		/// All keys for save data in PlayerPrefs
		/// </summary>
		private const string MusicVolumeKey    = "mvl";
		private const string SoundVolumeKey    = "svl";
		private const string VibrateKey        = "vbr";
		private const string LastGamePlayedKey = "lgp";

		private const string BestScoreKey  = "htr";
		private const string HistoryKey  = "htr";
		private const string FavoriteKey = "fvr";
		private const string TutorialKey = "ttr";
		private const string GameModeKey = "gm";

		private const string collectionKey     = "clt";
		private const string levelKey          = "lvl";
		private const string currentLevelKey   = "crl";
		private const string scoreKey          = "scr";
		private const string replayKey         = "rpl";
		private const string starKey           = "str";
		private const string boosterRemoveKey  = "brm";
		private const string boosterUndoKey    = "bud";
		private const string boosterShuffleKey = "bsf";
		private const string datetimeKey       = "dtm";
		private const string maxStageKey       = "mst";
		private const string userDateKey       = "udt";
		private const string winStreakKey      = "wst";
		private const string tempWinStreakKey  = "cwn";
		private const string rateUsKey         = "rtu";
		private const string bonusBoosterKey   = "bnb";
		private const string rankScoreKey      = "rks";
		private const string rankRemoveKey     = "rrm";
		private const string rankUndoKey       = "rud";
		private const string rankShuffleKey    = "rsf";
		private const string rankSlotKey       = "rsl";
		private const string countLevelKey     = "clv";

		private const string Break = "~";

		/// <summary>
		/// A variable to save the score for the second player when opening the app
		/// </summary>
		public int BlueScore { get; private set; }
		/// <summary>
		/// A variable to save the score for the first player when opening the app
		/// </summary>
		public int RedScore { get; private set; }
		/// <summary>
		/// A variable for save last game win
		/// </summary>
		public int LastWin { get; private set; } = -1;
		/// <summary>
		/// Action for catch event when music volume change
		/// </summary>
		public Action<float> OnMusicVolumeChange;
		/// <summary>
		/// Action for catch event when sound volume change
		/// </summary>
		public Action<float> OnSoundVolumeChange;
		/// <summary>
		/// Action for catch event when vibrate change
		/// </summary>
		public Action<bool> OnVibrateChange;
		/// <summary>
		/// Add 1 score to second player's score
		/// </summary>
		public void AddBlueScore()
		{
			BlueScore++;
		}
		/// <summary>
		/// Add 1 score to first player's score
		/// </summary>
		public void AddRedScore()
		{
			RedScore++;
		}
		/// <summary>
		/// Set game just win
		/// </summary>
		/// <param name="win">order of games in the list</param>
		public void SetLastWin(int win)
		{
			LastWin = win;
		}
		/// <summary>
		/// Get the music volume of game
		/// </summary>
		/// <returns>Music volume</returns>
		public float GetMusicVolume()
		{
			return PlayerPrefs.GetFloat(MusicVolumeKey, 1.0f);
		}
		/// <summary>
		/// Set the music volume of game
		/// </summary>
		/// <param name="volume"></param>
		public void SetMusicVolume(float volume)
		{
			PlayerPrefs.SetFloat(MusicVolumeKey, volume);
			OnMusicVolumeChange?.Invoke(volume);
		}
		/// <summary>
		/// Get the sound volume of game
		/// </summary>
		/// <returns>Sound volume</returns>
		public float GetSoundVolume()
		{
			return PlayerPrefs.GetFloat(SoundVolumeKey, 1.0f);
		}
		/// <summary>
		/// Set the sound volume of game
		/// </summary>
		/// <param name="volume"></param>
		public void SetSoundVolume(float volume)
		{
			PlayerPrefs.SetFloat(SoundVolumeKey, volume);
			OnSoundVolumeChange?.Invoke(volume);
		}
		/// <summary>
		/// Get vibrate of game
		/// </summary>
		/// <returns>is vibrate or not</returns>
		public bool GetVibrate()
		{
			return PlayerPrefs.GetInt(VibrateKey, 1) == 0 ? false : true;
		}
		/// <summary>
		/// Set vibrate of game
		/// </summary>
		/// <param name="isVibrate"></param>
		public void SetVibrate(bool isVibrate)
		{
			OnVibrateChange?.Invoke(isVibrate);
			if (isVibrate == true)
			{
				PlayerPrefs.SetInt(VibrateKey, 1);
			}
			else
			{
				PlayerPrefs.SetInt(VibrateKey, 0);
			}
		}
		/// <summary>
		/// Get the game that just played
		/// </summary>
		/// <returns>order of game in list game</returns>
		public int GetLastGamePlayed()
		{
			return PlayerPrefs.GetInt(LastGamePlayedKey, 0);
		}
		/// <summary>
		/// Set the game that just played
		/// </summary>
		/// <param name="lastGamePlayed">order of game in list game</param>
		public void SetLastGamePlayed(int lastGamePlayed)
		{
			PlayerPrefs.SetInt(LastGamePlayedKey, lastGamePlayed);
		}
		/// <summary>
		/// Save a list of value to PlayerPrefs
		/// </summary>
		/// <typeparam name="T">type of value</typeparam>
		/// <param name="key">name key of list value</param>
		/// <param name="value">list of value that need to save</param>
		/// <exception cref="Exception"></exception>
		private void SaveList<T>(string key, List<T> value)
		{
			if (value == null)
			{
				Logger.Warning("Input list null");
				value = new List<T>();
			}
			if (value.Count == 0)
			{
				PlayerPrefs.SetString(key, string.Empty);
				return;
			}
			if (typeof(T) == typeof(string))
			{
				foreach (var item in value)
				{
					string tempCompare = item.ToString();
					if (tempCompare.Contains(Break))
					{
						throw new Exception("Invalid input. Input contain '~'.");
					}
				}
			}
			PlayerPrefs.SetString(key, string.Join(Break, value));
		}
		/// <summary>
		/// Get list of value that saved
		/// </summary>
		/// <typeparam name="T">type of value</typeparam>
		/// <param name="key">name key of list value</param>
		/// <param name="defaultValue">default value if playerprefs doesn't have value</param>
		/// <returns></returns>
		private List<T> GetList<T>(string key, List<T> defaultValue)
		{
			if (PlayerPrefs.HasKey(key) == false)
			{
				return defaultValue;
			}
			if (PlayerPrefs.GetString(key) == string.Empty)
			{
				return new List<T>();
			}
			string temp = PlayerPrefs.GetString(key);
			string[] listTemp = temp.Split(Break);
			List<T> list = new List<T>();

			foreach (string s in listTemp)
			{
				list.Add((T)Convert.ChangeType(s, typeof(T)));
			}
			return list;
		}
		/// <summary>
		/// Save list of history game that played
		/// </summary>
		/// <param name="history">list of game played</param>
		public void SaveHistory(List<int> history)
		{
			SaveList(HistoryKey, history);
		}
		/// <summary>
		/// Get list of history game that played
		/// </summary>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public List<int> GetHistory(List<int> defaultValue = null)
		{
			return GetList<int>(HistoryKey, defaultValue);
		}
		/// <summary>
		/// Save list of favorite game
		/// </summary>
		/// <param name="favorite">list of favorite game</param>
		public void SaveFavorite(List<int> favorite)
		{
			SaveList(FavoriteKey, favorite);
		}
		/// <summary>
		/// Get list of favorite game that saved
		/// </summary>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public List<string> GetFavorite(List<string> defaultValue = null)
		{
			return GetList<string>(FavoriteKey, defaultValue);
		}
		/// <summary>
		/// This function use for turn on tutorial. First time call, it return false, next times return true.
		/// </summary>
		/// <returns></returns>
		public bool IsTutorialPlayed()
		{
			int first = PlayerPrefs.GetInt(TutorialKey, 0);
			if (first == 0)
			{
				PlayerPrefs.SetInt(TutorialKey, 1);
				return false;
			}
			else
			{
				return true;
			}
		}
		/// <summary>
		/// This function used to tell if the player has played the previous level
		/// </summary>
		/// <returns></returns>
		public void SetReplay(bool isReplay)
		{
			if(isReplay)
			{
				PlayerPrefs.SetInt(replayKey, 1);
			} else
			{
				PlayerPrefs.SetInt(replayKey, 0);
			}
		}
		public bool IsReplayGame()
		{
			int replay = PlayerPrefs.GetInt(replayKey, 0);
			if (replay == 1)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		/// <summary>
		/// Save score of player
		/// </summary>
		/// <param name="score"></param>
		public void SetScore(int score)
		{
			PlayerPrefs.SetInt(scoreKey, score);
		}
		/// <summary>
		/// Get score that saved
		/// </summary>
		/// <returns></returns>
		public int GetScore()
		{
			return PlayerPrefs.GetInt(scoreKey, 0);
		}
		/// <summary>
		/// Save collection player has got
		/// </summary>
		/// <param name="collection"></param>
		public void SetCollection(int collection)
		{
			PlayerPrefs.SetInt(collectionKey, collection);
		}
		/// <summary>
		/// Get collection that saved
		/// </summary>
		/// <returns></returns>
		public int GetCollection()
		{
			return PlayerPrefs.GetInt(collectionKey, 0);
		}
		/// <summary>
		/// Save game mode of player
		/// </summary>
		/// <param name="mode"></param>
		public void SetGameMode(int mode)
		{
			PlayerPrefs.SetInt(GameModeKey, mode);
		}
		/// <summary>
		/// Get game mode that saved
		/// </summary>
		/// <returns></returns>
		public int GetGameMode()
		{
			return PlayerPrefs.GetInt(GameModeKey, 0);
		}
		/// <summary>
		/// Save level of player
		/// </summary>
		/// <param name="level"></param>
		public void SetHighestLevel(int level)
		{
			PlayerPrefs.SetInt(levelKey, level);
		}
		/// <summary>
		/// Get level that saved
		/// </summary>
		/// <returns></returns>
		public int GetHighestLevel()
		{
			return PlayerPrefs.GetInt(levelKey, 0);
		}
		/// <summary>
		/// Save level of player
		/// </summary>
		/// <param name="level"></param>
		public void SetCurrentLevel(int level)
		{
			PlayerPrefs.SetInt(currentLevelKey, level);
		}
		/// <summary>
		/// Get level that saved
		/// </summary>
		/// <returns></returns>
		public int GetCurrentLevel()
		{
			return PlayerPrefs.GetInt(currentLevelKey, 0);
		}
		/// <summary>
		/// Save all data
		/// </summary>
		public void Save()
		{
			PlayerPrefs.Save();
		}

		public string ConvertBytesToString(List<byte> stars)
		{
			var result = new StringBuilder(512);
			for (var i = 0; i < stars.Count - 1; i += 2)
			{
				result.Append(Convert.ToChar((byte)(stars[i] << 4 | stars[i + 1])));
			}
			if (stars.Count % 2 == 1)
			{
				result.Append(Convert.ToChar((byte)(stars[stars.Count - 1] << 4)));
			}
			return result.ToString();
		}

		public List<byte> ConvertStringToByte(string data)
		{
			var result = new List<byte>(512);
			foreach (var item in data)
			{
				var a = (byte)((Convert.ToByte(item) >> 4) & 0x0F);
				result.Add(a);
				var b = (byte)(item & 0x0F);
				if(b != 0)
				{
					result.Add(b);
				} else
				{
					break;
				}
			}
			return result;
		}

		public List<byte> GetListStars()
		{
			List<byte> result = new();
			string data = PlayerPrefs.GetString(starKey);
			result = ConvertStringToByte(data);
			return result;
		}

		public void SetListStars(List<byte> stars)
		{
			PlayerPrefs.SetString(starKey, ConvertBytesToString(stars));
		}

		public void SetStars(int level, byte stars)
		{
			List<byte> result = GetListStars();
			if(result.Count == 0)
			{
				result.Add(stars);
				SetListStars(result);
				return;
			} else if(level > result.Count - 1)
			{
				result.Add(stars);
			} else
			{
				if(result[level] < stars)
				{
					result[level] = stars;
				}
			}
			SetListStars(result);
		}
		public long GetDate()
		{
			string s = PlayerPrefs.GetString(datetimeKey, 0.ToString());
			return Convert.ToInt64(s);
		}
		/// <summary>
		/// Save date
		/// </summary>
		/// <param name="date"></param>
		public void SetDate(long date)
		{
			PlayerPrefs.SetString(datetimeKey, date.ToString());
		}
		public int GetMaxStage()
		{
			return PlayerPrefs.GetInt(maxStageKey, 0);
		}
		public void SetMaxStage(int maxStage)
		{
			PlayerPrefs.SetInt(maxStageKey, maxStage);
		}

		public long GetUserDay()
		{
			string s = PlayerPrefs.GetString(userDateKey, 0.ToString());
			return Convert.ToInt64(s);
		}
		public void SetUserDay(long userDate)
		{
			PlayerPrefs.SetString(userDateKey, userDate.ToString());
		}

		public void SaveBestScore(List<int> bestScore)
		{
			SaveList(BestScoreKey, bestScore);
		}

		public List<int> GetBestScore(List<int> defaultValue = null)
		{
			return GetList<int>(BestScoreKey, defaultValue);
		}

		public long GetBestScoreByLevel(int level)
		{
			List<int> result = new();
			result = GetBestScore();
			if (result != null && result.Count > level)
			{
				return Convert.ToInt64(result[level]);
			} else { return 0; }
		}

		public void SetBestScoreByLevel(int level, int score)
		{
			List<int> result = new();
			if(GetBestScore() != null)
			{
				result = GetBestScore();
				if (result.Count - 1 < level)
				{
					result.Add(score);
				}
				else
				{
					if (result[level] < score)
					{
						result[level] = score;
					}
				}
			} else
			{
				result.Add(score);
			}
			SaveBestScore(result);
		}

		public int GetMaxWinningStreak()
		{
			return PlayerPrefs.GetInt(winStreakKey, 0);
		}
		public void SetMaxWinningStreak(int streak)
		{
			PlayerPrefs.SetInt(winStreakKey, streak);
		}

		public void SetWinningStreak(int streak)
		{
			PlayerPrefs.SetInt(tempWinStreakKey, streak);
		}
		public int GetWinningStreak()
		{
			return PlayerPrefs.GetInt(tempWinStreakKey, 0);
		}

		public void SetRate()
		{
			PlayerPrefs.SetInt(rateUsKey, 1);
		}
		public bool IsRate()
		{
			int replay = PlayerPrefs.GetInt(rateUsKey, 0);
			if (replay == 1)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public void SaveDataRankMode(int rankScore, int[] quantity)
		{
			PlayerPrefs.SetInt(rankScoreKey, rankScore);
			//Booster
			PlayerPrefs.SetInt(rankRemoveKey,  quantity[(int)Booster.remove]);
			PlayerPrefs.SetInt(rankUndoKey,    quantity[(int)Booster.undo]);
			PlayerPrefs.SetInt(rankShuffleKey, quantity[(int)Booster.shuffle]);
			PlayerPrefs.SetInt(rankSlotKey,    quantity[(int)Booster.slot]);
		}
		public int GetScoreRankMode()
		{
			return PlayerPrefs.GetInt(rankScoreKey, 0);
		}

		public int[] GetDataRankMode()
		{
			int[] quantityRankBoost = new int[4];
			quantityRankBoost[0] = PlayerPrefs.GetInt(rankRemoveKey , 0);
			quantityRankBoost[1] = PlayerPrefs.GetInt(rankUndoKey,    0);
			quantityRankBoost[2] = PlayerPrefs.GetInt(rankShuffleKey, 0);
			quantityRankBoost[3] = PlayerPrefs.GetInt(rankSlotKey,    0);
			return quantityRankBoost;
		}

		public void SaveQuantityBoosterAdventure(int[] quantity)
		{
			PlayerPrefs.SetInt(boosterRemoveKey, quantity[0]);
			PlayerPrefs.SetInt(boosterUndoKey, quantity[1]);
			PlayerPrefs.SetInt(boosterShuffleKey, quantity[2]);
		}

		public int[] GetDataAdventureMode()
		{
			int[] quantityBoosterAdventure = new int[4];
			quantityBoosterAdventure[0] = PlayerPrefs.GetInt(boosterRemoveKey,  0);
			quantityBoosterAdventure[1] = PlayerPrefs.GetInt(boosterUndoKey,    0);
			quantityBoosterAdventure[2] = PlayerPrefs.GetInt(boosterShuffleKey, 0);
			quantityBoosterAdventure[3] = 0;
			return quantityBoosterAdventure;
		}

		public bool IsShowInterstitialAd()
		{
			int count = PlayerPrefs.GetInt(countLevelKey, 0);
			if(count == 0)
			{
				PlayerPrefs.SetInt(countLevelKey, 1);
				return false;
			} else
			{
				PlayerPrefs.SetInt(countLevelKey, 0);
				return true;
			}
		}


	}
}
