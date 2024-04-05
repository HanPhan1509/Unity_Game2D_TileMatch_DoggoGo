using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game
{
	public class BoostersPopups : MonoBehaviour
	{
		[SerializeField] private Transform frame;
		[SerializeField] private Image imgBooser;
		[SerializeField] private GameObject imgAddOne;
		[SerializeField] private TextMeshProUGUI nameBooster;
		[SerializeField] private TextMeshProUGUI describe;

		[SerializeField] private UnityEvent<Booster> OnButtonGetBooster;
		[SerializeField] private UnityEvent OnButtonNo;

		private Booster booster;

		public void ShowPopup()
		{
			frame.localScale = Vector2.zero;
			frame.DOScale(Vector2.one, 0.1f);
		}
		public void Initialize(DetailBoosterPopup detailBoosterPopup)
		{
			this.booster = detailBoosterPopup.booster;
			this.imgBooser.sprite = detailBoosterPopup.spriteBooster;
			this.imgBooser.SetNativeSize();
			this.nameBooster.text = detailBoosterPopup.namePopups;
			this.describe.text = detailBoosterPopup.describeBooster;

			if (detailBoosterPopup.booster == Booster.slot)
				imgAddOne.SetActive(false);
			else
				imgAddOne.SetActive(true);
		}

		public void ButtonGet()
		{
			OnButtonGetBooster?.Invoke(this.booster);
		}

		public void ButtonNo()
		{
			OnButtonNo?.Invoke();
		}
	}

}
