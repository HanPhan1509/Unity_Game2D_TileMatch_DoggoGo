using UnityEngine;

namespace Extensions
{
	public static class RectTransformExt
	{
		public static Rect ToRect(this RectTransform rectTransform, float bias = 0.0f)
		{
			var corners = new Vector3[4];
			rectTransform.GetLocalCorners(corners);
			return new Rect(
				rectTransform.localPosition.x + rectTransform.rect.x + bias,
				rectTransform.localPosition.y + rectTransform.rect.y + bias,
				rectTransform.rect.width - 2.0f * bias,
				rectTransform.rect.height - 2.0f * bias);
		}
	}
}
