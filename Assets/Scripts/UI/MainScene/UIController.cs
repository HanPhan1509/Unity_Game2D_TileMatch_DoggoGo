using Extensions;
using Home;
using Services;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
	[System.Serializable]
	public class GamePopups
	{
		public Button button;
		public int quantity;
		public GameObject quantityArea;
		public Sprite spriteBooster;
		public string namePopups;
		public string describeBooster;
	}
	public class UIController : MonoBehaviour
	{
		private PlayerService playerService;
		private AudioService audioService;
		private AdsService adsService;
		[SerializeField] private AudioGame audioGame;
		[SerializeField] private GameView view;
		[SerializeField] private GameController controller;
		[SerializeField] private RectTransform header;
		[SerializeField] private RectTransform bottom;
		[SerializeField] private GamePopups[] gamePopups;
		[SerializeField] private Animator animatorBooster;

		private Booster booster;
		private GameMode Mode;

		private int numberRemove = 0;
		private int numberUndo = 0;
		private int numberShuffle = 0;

		

		private void Awake()
		{
			view.ThrowIfNull();
			controller.ThrowIfNull();
			header.ThrowIfNull();
			bottom.ThrowIfNull();
		}

		public void OnInit(PlayerService playerService, AudioService audioService, AdsService adsService, GameMode Mode, int levelTutorial, int countRound)
		{
			this.playerService = playerService;
			this.audioService = audioService;
			this.adsService = adsService;
			this.Mode = Mode;
			UnlockButtonBooster(levelTutorial);
		}

		public void GetQuantityBooster(int numberRemove, int numberUndo, int numberShuffle)
		{
			this.numberRemove = numberRemove;
			this.numberUndo = numberUndo;
			this.numberShuffle = numberShuffle;
		}

		public void UnlockButtonBooster(int levelTutorial)
		{
			foreach (GamePopups popup in gamePopups)
			{
				popup.button.gameObject.SetActive(true);
			}
			if (Mode == GameMode.Tutorial)
			{
				if (levelTutorial == 0)
				{
					gamePopups[2].button.gameObject.SetActive(false);
					gamePopups[3].button.gameObject.SetActive(false);
				}
				else if (levelTutorial == 1)
				{
					gamePopups[3].button.gameObject.SetActive(false);
				}
				else
				{
					return;
				}
			}
		}
	}
}
