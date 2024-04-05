using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemsTopPlayer : MonoBehaviour
{
	[SerializeField] private Image imgTop;
	[SerializeField] private TextMeshProUGUI numberTop;
	[SerializeField] private TextMeshProUGUI nameUser;
	[SerializeField] private TextMeshProUGUI point;
	public void SetItemDetailsInfomation(int numberTop, string name, int point, Sprite sprite = null)
	{
		if (sprite != null)
		{
			imgTop.enabled = true;
			imgTop.sprite = sprite;
		}
		else
		{
			imgTop.enabled = false;
		}
		this.numberTop.text = numberTop.ToString();
		this.nameUser.text = name;
		this.point.text = point.ToString();
	}
}
