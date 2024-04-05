using System.Collections.Generic;

using UnityEngine;

using Extensions;
using System;
using DG.Tweening;
using System.Linq;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

using NaughtyAttributes;
#endif

namespace Game
{
	public class Cards : MonoBehaviour
	{
		[SerializeField] private Card cardPrefab;
		[SerializeField] private RectTransform rectTransform;

#if UNITY_EDITOR
		[Space(8.0f)]
		[SerializeField] private LevelScriptableObject levelScriptableObject;

		private Dictionary<Card, int> cache = new Dictionary<Card, int>();
#endif

		private Node root;
		private Dictionary<int, Card> map = new Dictionary<int, Card>();

		//DG
		private List<Card> lstCard = new List<Card>();
		private Dictionary<Type, Sprite> dictCardClassification;
		private List<Type> lstType = new List<Type>();
		private List<Node> nodeAddToRoot = new List<Node>();
		private Node saveNode;

		private bool isGameOver = false;
		private Action<Card> onMove;
		private Action onNextLevel;

		private void Awake()
		{
			cardPrefab.ThrowIfNull();
		}

		public void PassFunc(Action<Card> onMove, Action onNextLevel)
		{
			this.onMove = onMove;
			this.onNextLevel = onNextLevel;
		}

		public void Initialize(LevelScriptableObject level, List<TypeCard> lstTypeCard, Dictionary<Type, Sprite> dictCardClassification)
		{
			this.dictCardClassification = dictCardClassification;
			root = level.Root;
			map.Clear();
			CreateCard(level, lstTypeCard);

			lstType = lstType.OrderBy(_ => Guid.NewGuid()).ToList();

			for (var i = 0; i < level.Nodes.Count; ++i)
			{
				var node = level.Nodes[i];
				var card = Instantiate(cardPrefab, transform);
				card.Type = lstType[i];
				card.ChangeImage(dictCardClassification[lstType[i]]);
				card.Initialize(i, node.Position, OnCardClicked);
				lstCard.Add(card);
				map.Add(node.Id, card);
			}

			foreach (var node in root.Childrens)
			{
				map[node.Id].SetInteractable(true);
			}
		}

		private void CreateCard(LevelScriptableObject level, List<TypeCard> lstTypeCard)
		{
			int numberGroup = level.Nodes.Count / 3; //So nhom se an dc trong 1 map
			int numberLoops = numberGroup / lstTypeCard.Count; //So vong lap cho du card
			int numberLoopsResidual = numberGroup % lstTypeCard.Count; //So vong lap neu bi du card

			lstType.Clear();
			for (int i = 0; i < numberLoops; i++)
			{
				foreach (TypeCard typeCard in lstTypeCard)
				{
					for (int j = 0; j < 3; j++)
					{
						lstType.Add(typeCard.typeCard);
					}
				}
			}

			if (numberLoopsResidual != 0)
			{
				for (int i = 0; i < numberLoopsResidual; i++)
				{
					for (int j = 0; j < 3; j++)
					{
						lstType.Add(lstTypeCard[i].typeCard);
					}
				}
			}
		}

#if UNITY_EDITOR
		[Button("Generate Level")]
		private void GenerateLevel()
		{
			if (levelScriptableObject == null)
			{
				Debug.LogWarning("Level Scriptable Object is null");
				return;
			}

			var cards = gameObject.GetComponentsInChildren<Card>();
			if (cards.Length <= 0 || cards.Length % 3 != 0)
			{
				Debug.LogWarning($"Invalid number of cards {cards.Length} need {3 - cards.Length % 3}");
				return;
			}

			cache.Clear();
			var nodes = new List<Node>();
			levelScriptableObject.Nodes.Clear();
			for (var i = 0; i < cards.Length; ++i)
			{
				var card = cards[i];
				var node = new Node()
				{
					Id = i,
					Position = card.GetPosition(),
					ParentCount = 0
				};
				cache.Add(card, i);
				nodes.Add(node);
				levelScriptableObject.Nodes.Add(node);
			}

			levelScriptableObject.Root = new Node();
			for (var i = 0; i < cards.Length - 1; ++i)
			{
				for (var j = i + 1; j < cards.Length; ++j)
				{
					if (cards[i].Overlaps(cards[j]))
					{
						var parentId = cache[cards[j]];
						var childrenId = cache[cards[i]];
						++nodes[childrenId].ParentCount;
						nodes[parentId].Childrens.Add(nodes[childrenId]);
					}
				}
			}

			foreach (var node1 in nodes)
			{
				var isRoot = true;
				foreach (var node2 in nodes)
				{
					if (node1 != node2)
					{
						if (node2.Childrens.Contains(node1))
						{
							isRoot = false;
							break;
						}
					}
				}
				if (isRoot == true)
				{
					levelScriptableObject.Root.Childrens.Add(node1);
				}
			}

			levelScriptableObject.Root.Denoise();

			//Set Map size
			float minXCard = nodes[0].Position.x;
			float maxXCard = nodes[0].Position.x;
			float minYCard = nodes[0].Position.y;
			float maxYCard = nodes[0].Position.y;
			foreach (var node in nodes)
			{
				minXCard = Mathf.Min(minXCard, node.Position.x);
				maxXCard = Mathf.Max(maxXCard, node.Position.x);
				minYCard = Mathf.Min(minYCard, node.Position.y);
				maxYCard = Mathf.Max(maxYCard, node.Position.y);
			}
			var widthMap = Mathf.Abs(maxXCard) + Mathf.Abs(minXCard);
			var heightMap = Mathf.Abs(maxYCard) + Mathf.Abs(minYCard);
			levelScriptableObject.rect.size = new Vector2(widthMap, heightMap);

			var centerXMap = maxXCard - (widthMap / 2);
			var centerYMap = maxYCard - (heightMap / 2);
			levelScriptableObject.rect.center = new Vector2(centerXMap, centerYMap);

			Logger.Debug("Generate level complete");
			EditorUtility.SetDirty(levelScriptableObject);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		[Button("Load Level")]
		private void LoadLevel()
		{
			if (levelScriptableObject == null)
			{
				Debug.LogWarning("Level Scriptable Object is null");
				return;
			}

			for (var i = transform.childCount - 1; i >= 0; --i)
			{
				DestroyImmediate(transform.GetChild(i).gameObject);
			}

			for (var i = 0; i < levelScriptableObject.Nodes.Count; ++i)
			{
				var node = levelScriptableObject.Nodes[i];
				var card = Instantiate(cardPrefab, transform);
				card.SetupPosition(node.Position);
			}

			Logger.Debug("Load level complete");
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
#endif
		public void GameOver(bool gameOver)
		{
			isGameOver = gameOver;
		}

		private void OnCardClicked(int id, Card card)
		{
			if (isGameOver)
				return;
			onMove?.Invoke(card);
			//card.SetInteractable(false);
			int indexCardRemove = 0;
			for (int i = 0; i < lstCard.Count; i++)
			{
				if (lstCard[i].Id == id)
				{
					indexCardRemove = i;
				}
			}
			lstCard.RemoveAt(indexCardRemove);
			//map[id].gameObject.SetActive(false);

			Node nodeNeedRemove = null;
			foreach (var node in root.Childrens)
			{
				if (node.Id == id)
				{
					nodeNeedRemove = node;
					break;
				}
			}
			saveNode = nodeNeedRemove.Clone();
			root.Childrens.Remove(nodeNeedRemove);

			nodeAddToRoot.Clear();
			foreach (var node in nodeNeedRemove.Childrens)
			{
				if (node.ParentCount <= 1)
				{
					nodeAddToRoot.Add(node);
				}
				--node.ParentCount;
			}
			root.Childrens.AddRange(nodeAddToRoot);

			foreach (var node in root.Childrens)
			{
				map[node.Id].SetInteractable(true);
			}
			CheckCardOnScreen();
		}

		#region GAME SKILLS

		public void ToolReturnCard(Card card, Vector2 position)
		{
			card.transform.SetParent(this.transform);
			card.transform.DOScale(Vector2.one, 0.2f);
			card.transform.DOLocalMove(position, 0.2f);
			lstCard.Add(card);
			var newNode = new Node()
			{
				Id = card.Id,
				Position = position,
				ParentCount = 0,
			};
			root.Childrens.Add(newNode);
		}

		public void Shuffle()
		{
			lstType = new List<Type>();
			for (int i = 0; i < lstCard.Count; i++)
			{
				lstType.Add(lstCard[i].Type);
			}

			//shuffle type
			lstType = lstType.OrderBy(_ => Guid.NewGuid()).ToList();

			//Gan lai card
			for (var i = 0; i < lstCard.Count; i++)
			{
				lstCard[i].Type = lstType[i];
				lstCard[i].ChangeImage(dictCardClassification[lstType[i]]);
			}
		}

		public void ToolUndo(Card card)
		{
			//Set button active click = false
			if (nodeAddToRoot.Count != 0)
			{
				foreach (var node in nodeAddToRoot)
				{
					foreach (Card card1 in lstCard)
					{
						if (card1.Id == node.Id)
						{
							card1.SetInteractable(false);
						}
					}
				}

				foreach (Node node1 in nodeAddToRoot)
				{
					foreach (var node2 in root.Childrens)
					{
						if (node1.Id == node2.Id)
						{
							root.Childrens.Remove(node1);
							break;
						}
					}
				}
			}
			//Add node cu vao root
			root.Childrens.Add(saveNode);
			lstCard.Add(card);
			card.SetInteractable(true);
			card.transform.SetParent(this.transform);
			card.transform.DOScale(Vector2.one, 0.2f);
			card.transform.DOLocalMove(saveNode.Position, 0.2f);
			nodeAddToRoot.Clear();
		}

		#endregion

		private void CheckCardOnScreen()
		{
			if (root.Childrens.Count == 0)
			{
				onNextLevel?.Invoke();
			}
		}

		public void GetCardsForTutorial(int level)
		{
			switch (level)
			{
				case 0:
					int[] idRootNode = { 5, 8, 11 };
					bool isChoose = true;
					List<Card> tempCardsTutorial = new List<Card>();
					do
					{
						isChoose = true;
						tempCardsTutorial.Clear();
						foreach (var card in lstCard)
						{
							foreach (var id in idRootNode)
							{
								if (card.Id == id)
								{
									tempCardsTutorial.Add(card);
								}
							}
						}
						//Check 3 card co cung type hay ko
						if (tempCardsTutorial[0].Type == tempCardsTutorial[1].Type || tempCardsTutorial[1].Type == tempCardsTutorial[2].Type || tempCardsTutorial[0].Type == tempCardsTutorial[2].Type)
						{
							Shuffle();
							isChoose = false;
						}
					} while (!isChoose);
					foreach (var card in tempCardsTutorial)
					{
						StartCoroutine(OnClickCardTutorial(card));
					}
					break;
				case 1:
					int cardUndo = 12;
					foreach (var card in lstCard)
					{
						if (card.Id == cardUndo)
						{
							card.OnClicked();
							return;
						}
					}
					break;
			}
		}

		private IEnumerator OnClickCardTutorial(Card card)
		{
			yield return new WaitForSeconds(0.1f);//time back card
			card.OnClicked();
		}	
	}
}
