using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Game
{
	public class GameView : MonoBehaviour
	{
		[SerializeField] private RectTransform rectTransform;
		[Space(0.8f)]
		[Header("POPUPS")]
		[SerializeField] private GameScene gameScene;
		[SerializeField] private SettingsPopup settingsPopups;
		[SerializeField] private QuitPopup quitPopups;
		[SerializeField] private BoostersPopups boostersPopups;
		[SerializeField] private Username usernamePopups;
		[SerializeField] private Cleared clearedPopups;
		[SerializeField] private DescibeBooster descibeBoosterPopups;
		[SerializeField] private Revive revivePopup;
		[SerializeField] private NoticePopup noticePopups;
		[SerializeField] private GiftBooster giftBoosterPopups;

		public GameScene GameScene => gameScene;
		public SettingsPopup SettingsPopup => settingsPopups;
		public QuitPopup QuitPopup => quitPopups;
		public BoostersPopups BoostersPopup => boostersPopups;
		public Username UsernamePopup => usernamePopups;
		public Cleared ClearedPopups => clearedPopups;
		public DescibeBooster DescibeBooster => descibeBoosterPopups;
		public Revive Revive => revivePopup;
		public NoticePopup NoticePopup => noticePopups;
		public GiftBooster GiftBooster => giftBoosterPopups;

		[SerializeField] public  GameObject moreSlot;
		[SerializeField] private GameObject showStarArea;

		public BoostersPopups BoostersPopups => boostersPopups;

		public void AvoidCutouts(Rect safeArea, float heightBanner = 0)
		{
			rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -safeArea.y);
			if(heightBanner != 0)
			{
				rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, heightBanner);
			}
		}

		public void OpenPopup(UIPopups popups)
		{
			switch (popups)
			{
				case UIPopups.Game:
					gameScene.ContinueCombo();
					settingsPopups.gameObject.SetActive(false);
					quitPopups.gameObject.SetActive(false);
					boostersPopups.gameObject.SetActive(false);
					usernamePopups.gameObject.SetActive(false);
					clearedPopups.gameObject.SetActive(false);
					descibeBoosterPopups.gameObject.SetActive(false);
					revivePopup.gameObject.SetActive(false);
					giftBoosterPopups.gameObject.SetActive(false);
					noticePopups.gameObject.SetActive(false);
					break;
				case UIPopups.Settings:
					gameScene.Pause();
					settingsPopups.gameObject.SetActive(true);
					settingsPopups.ShowPopup();
					break;
				case UIPopups.Booster:
					gameScene.Pause();
					boostersPopups.gameObject.SetActive(true);
					boostersPopups.ShowPopup();
					break;
				case UIPopups.Username:
					gameScene.ResetTimeline();
					usernamePopups.gameObject.SetActive(true);
					usernamePopups.ShowPopup();
					break;
				case UIPopups.Quit:
					gameScene.Pause();
					settingsPopups.gameObject.SetActive(false);
					quitPopups.gameObject.SetActive(true);
					quitPopups.ShowPopup();
					break;
				case UIPopups.Cleared:
					revivePopup.gameObject.SetActive(false);
					clearedPopups.gameObject.SetActive(true);
					clearedPopups.ShowPopup();
					break;
				case UIPopups.Describe:
					descibeBoosterPopups.gameObject.SetActive(true);
					descibeBoosterPopups.ShowPopup();
					break;
				case UIPopups.Revive:
					gameScene.Pause();
					revivePopup.gameObject.SetActive(true);
					revivePopup.ShowPopup();
					break;
				case UIPopups.GiftBooster:
					giftBoosterPopups.gameObject.SetActive(true);
					giftBoosterPopups.ShowPopup(); ;
					break;
				case UIPopups.Notice:
					noticePopups.gameObject.SetActive(true);
					noticePopups.ShowPopup(); ;
					break;
			}
		}

		#region SETTINGS
		public void LoadSetting(float music, float sound)
		{
			settingsPopups.LoadSetting(music, sound);
		}
		#endregion

		#region LOAD SCENE
		public void GoToHome()
		{
			SceneManager.LoadScene("Home");
		}

		public void ReLoadScene()
		{
			SceneManager.LoadScene("Game");
		}
		#endregion
	}
}
