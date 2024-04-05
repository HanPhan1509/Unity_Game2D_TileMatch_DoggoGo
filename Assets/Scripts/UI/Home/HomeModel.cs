using Game;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Home
{
	[System.Serializable]
	public class ItemUser
	{
		public int idUser;
		public string name;
		public int point = 0;
	}

	[System.Serializable]
	public class ItemMapInfo
	{
		public int idMap;
		public int numberStar = -1;
	}

	[System.Serializable]
	public class ItemCollection
	{
		public int idMeme;
		public string nameMeme;
		public Sprite spriteMeme;
		public int isCollect = -1;
	}
	public class HomeModel : MonoBehaviour
	{
		[SerializeField] public List<ItemMapInfo> lstMap;
		[SerializeField] public List<byte> lstSaveStar = new();
		[SerializeField] public List<Sprite> lstMemeImage;
		public int numberLevel = 0;

		[Space(0.8f)]
		[Header("LEVELS MAP")]
		[SerializeField] public int totalLevelNormal = 30;

		[SerializeField] private float maxTimeShowInterAds = 180;
		public float MaxTimeShowInterAds { get => maxTimeShowInterAds; }
	}
}
