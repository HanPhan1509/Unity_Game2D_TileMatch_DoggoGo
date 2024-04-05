using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class QuitPopup : MonoBehaviour
{
	[SerializeField] private Transform frame;
	[SerializeField] private UnityEvent onButtonQuitGame;
	[SerializeField] private UnityEvent onButtonExit;

	public void ShowPopup()
	{
		frame.localScale = Vector2.zero;
		frame.DOScale(Vector2.one, 0.1f);
	}
	public void ButtonYes()
	{
		onButtonQuitGame?.Invoke();
	}

	public void ButtonNo()
	{
		onButtonExit?.Invoke();
	}	
}
