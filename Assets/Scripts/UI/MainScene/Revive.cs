using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
	public class Revive : MonoBehaviour
	{
		[SerializeField] private Transform frame;
		[SerializeField] private UnityEvent onButtonRevive;
		[SerializeField] private UnityEvent onButtonX;

		public void ShowPopup()
		{
			frame.localScale = Vector2.zero;
			frame.DOScale(Vector2.one, 0.1f);
		}
		public void ButtonRevive()
		{
			onButtonRevive?.Invoke();
		}

		public void ButtonX()
		{
			onButtonX?.Invoke();
		}
	}
}
