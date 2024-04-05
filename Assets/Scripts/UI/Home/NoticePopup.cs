using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
	public class NoticePopup : MonoBehaviour
	{
		[SerializeField] private Transform frame;

		public void ShowPopup()
		{
			frame.localScale = Vector2.zero;
			frame.DOScale(Vector2.one, 0.1f);
		}	
	}
}
