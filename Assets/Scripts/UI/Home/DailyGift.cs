using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Home
{
	public class DailyGift : MonoBehaviour
	{
		[SerializeField] private Transform frame;
		[SerializeField] private UnityEvent OnButtonClaim;

		public void ShowPopup()
		{
			frame.localScale = Vector2.zero;
			frame.DOScale(Vector2.one, 0.1f);
		}

		public void ButtonClaim()
		{
			OnButtonClaim.Invoke();
		}	
	}
}
