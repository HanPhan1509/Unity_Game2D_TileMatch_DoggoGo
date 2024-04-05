using System;

using UnityEngine;
using UnityEngine.UI;

using Extensions;

namespace Game
{
	[RequireComponent(typeof(Button))]
	public class Card : MonoBehaviour
	{
#if UNITY_EDITOR
		private const float bias = 1.0f;
		private Rect rect;
#endif

		//private int id;
		public int Id { get; set; }
		private Action<int, Card> onClick;

		// Cache
		private RectTransform rectTransform;
		private Image image;
		private Button button;

		//GD
		public Type Type { get; set; }
		[SerializeField] private Image iconImage;

		private void Awake()
		{
			rectTransform = gameObject.GetComponent<RectTransform>();
			image = gameObject.GetComponent<Image>();
			button = gameObject.GetComponent<Button>();

#if UNITY_EDITOR
			rect = rectTransform.ToRect(bias);
#endif

			button.interactable = false;
		}

		public void Initialize(int id, Vector2 position, Action<int, Card> onClick)
		{
			this.Id = id;
			SetupPosition(position);
			this.onClick = onClick;
		}

		public Vector2 GetPosition() => rectTransform.localPosition;

		public void SetupPosition(Vector2 position)
		{
#if UNITY_EDITOR
			rectTransform = gameObject.GetComponent<RectTransform>();
#endif
			rectTransform.localPosition = position;
#if UNITY_EDITOR
			rect = rectTransform.ToRect(bias);
#endif
		}

		public void SetInteractable(bool interactable)
		{
			button.interactable = interactable;
			if(interactable)
			{
				iconImage.color = Color.white;
			} else
			{
				iconImage.color = button.colors.disabledColor;
			}
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			rectTransform = gameObject.GetComponent<RectTransform>();
			rect = rectTransform.ToRect(bias);
		}

		public bool Overlaps(Card other) => rect.Overlaps(other.rect);
#endif

		public void OnClicked()
		{
			onClick?.Invoke(Id, this);
		}

		public void ChangeImage(Sprite iconSprite)
		{
			iconImage.sprite = iconSprite;
		}
	}
}
