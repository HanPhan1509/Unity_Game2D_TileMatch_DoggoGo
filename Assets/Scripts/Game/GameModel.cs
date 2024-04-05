using System.Collections.Generic;

using UnityEngine;

namespace Game
{
	[System.Serializable]
	public class DetailBoosterPopup
	{
		public Booster booster;
		public Sprite  spriteBooster;
		public string  namePopups;
		public string  describeBooster;
	}

	[System.Serializable]
	public class TypeCard
	{
		public Type typeCard;
		public Sprite spriteIcon;
	}

	public class GameModel : MonoBehaviour
	{
		[SerializeField] public List<LevelScriptableObject> levelsTutorial;
		[SerializeField] public List<LevelScriptableObject> levelsNormal;
		[SerializeField] public List<LevelScriptableObject> levelsRank;

		public LevelScriptableObject LevelTutorial(int level) => Instantiate(levelsTutorial[level]);
		public LevelScriptableObject LevelNormal(int level)   => Instantiate(levelsNormal[level]);
		public LevelScriptableObject LevelRank(int level)     => Instantiate(levelsRank[level]);

		[SerializeField] public StarLevel Star;
		[SerializeField] public GameMode  Mode;
		public Dictionary<Type, Sprite>   dictCardClassification;

		[Header("LIST")]
		[SerializeField] public List<TypeCard> listAllTypeCard;
		[SerializeField] public List<int> amountImageTutorial;
		[SerializeField] public List<int> amountImageNormal;

		[Space(1f)]
		[Header("STORAGE POSITION")]
		[SerializeField] public Vector2[] returnPosition = new Vector2[6];

		[Space(1f)]
		[Header("OTHER PARAMETERS")]
		[SerializeField] public int       maxCardOnBar    = 7;

		[Space(1f)]
		[Header("TIME")]
		[SerializeField] public float     timeMovingCard  = 0.2f;
		[SerializeField] public float     timeDestroyCard = 0.3f;
		[SerializeField] public float     timeBackCard    = 0.1f;
		[SerializeField] public float     timeLoadLevel   = 1f;
		[SerializeField] public float     timeShowPopup   = 0.65f;

		[Space(1f)]
		[Header("BOOSTER")]
		[SerializeField] public Booster Booster;
		[SerializeField] private int numberRemove = 0;
		[SerializeField] private int numberUndo = 0;
		[SerializeField] private int numberShuffle = 0;
		public int NumberRemove { get => numberRemove; set => numberRemove = value; }
		public int NumberUndo { get => numberUndo; set => numberUndo = value; }
		public int NumberShuffle { get => numberShuffle; set => numberShuffle = value; }

		[Space(1f)]
		[Header("PARAMS UI")]
		[SerializeField] private float maxTimeShowInterAds = 180;
		[SerializeField] private float timeFillScore = 0.005f;
		//[SerializeField] private int   combo1 = 10;
		[SerializeField] private int   combo2 = 8;
		[SerializeField] private int   combo3 = 7;
		[SerializeField] private int   combo4 = 5;

		public float MaxTimeShowInterAds { get => maxTimeShowInterAds; }
		public float TimeFillScore { get => timeFillScore; }
		public int Combo2 { get => combo2; }
		public int Combo3 { get => combo3; }
		public int Combo4 { get => combo4; }


		[SerializeField] public List<DetailBoosterPopup> detailBoosterPopups = new();
	}
}
