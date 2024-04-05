using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Home
{
	public class LeaderBoardsPopup : MonoBehaviour
	{
		[SerializeField] private Transform frame;
		[SerializeField] private GameObject userPrefab;
		[SerializeField] private Sprite[] spriteTop;
		[SerializeField] private List<GameObject> tabGroup;
		[SerializeField] private Sprite[] choosingTab;
		[SerializeField] private TextMeshProUGUI timingGift;
		[SerializeField] private UnityEvent onButtonGift;
		[SerializeField] private UnityEvent onButtonExit;
		private int maxTop = 100;

		private List<ItemUser> lstUser = new List<ItemUser>();

		public void OnInit(List<ItemUser> lstUser)
		{
			this.lstUser = lstUser;
			if(lstUser.Count >= maxTop)
			{
				maxTop = 100;
			} else
			{
				maxTop = lstUser.Count;
			}
		}

		private void SetInfoItemsTopUsers()
		{
			for(int i = 0; i < maxTop; i++) //Top 100 users
			{
				ItemsTopPlayer item = SimplePool.Spawn(userPrefab, Vector2.zero, Quaternion.identity).GetComponent<ItemsTopPlayer>();
				if (i < 3)
				{
					item.SetItemDetailsInfomation(i + 1, lstUser[i].name, lstUser[i].point, spriteTop[i]);
				} else
				{
					item.SetItemDetailsInfomation(i + 1, lstUser[i].name, lstUser[i].point);
				}
				
			}
		}

		public void ButtonExit()
		{
			onButtonExit?.Invoke();
		}
		public void ButtonGift()
		{
			onButtonGift?.Invoke();
		}

		public void ShowPopup()
		{
			frame.localScale = Vector2.zero;
			frame.DOScale(Vector2.one, 0.1f);
		}
	}
}
