using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game
{
	public class GiftBooster : MonoBehaviour
	{
		[SerializeField] private Transform frame;
		[SerializeField] private Image imgBooster;
		[SerializeField] private RectTransform rectTransformNumber;
		[SerializeField] private Sprite[] spriteBooster;
		private Vector2 startPos;
		private Vector2 endPos;
		private Action donePopup;

		public void ShowGiftBooster(Booster booster, Vector2 endPos, Action donePopup)
		{
			this.endPos = endPos;
			this.donePopup = donePopup;
			imgBooster.sprite = spriteBooster[(int)booster];
		}	

		public void ShowPopup()
		{
			startPos = rectTransformNumber.position;
			rectTransformNumber.position = imgBooster.rectTransform.position;
			frame.localScale = Vector2.zero;
			frame.DOScale(Vector2.one, 0.1f).OnComplete(() =>
			{
				rectTransformNumber.DOMove(startPos, 0.3f).OnComplete(() =>
				{
					rectTransformNumber.DORotateQuaternion(Quaternion.identity, 1.2f);
					rectTransformNumber.DOMove(endPos, 1.2f).OnComplete(() =>
					{
						rectTransformNumber.DOScale(Vector2.zero, 0.1f);
						frame.DOScale(Vector2.zero, 0.1f).OnComplete(() =>
						{
							donePopup?.Invoke();
						});
					});
				});
			});
		}
	}
}
