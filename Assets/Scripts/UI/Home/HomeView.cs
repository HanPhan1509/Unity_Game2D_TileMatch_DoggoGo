using Game;
using UnityEngine;

namespace Home
{
	public class HomeView : MonoBehaviour
	{
		[Header("MAIN BODY PREFERENCE")]
		[SerializeField] private HomeScene         homeScene;
		[SerializeField] private SettingsPopup     settingsPopup;
		[SerializeField] private RemoveAdsPopup    removeAdsPopup;
		[SerializeField] private LevelSelect       levelSelect;
		[SerializeField] private RateUs            rateUsPopup;
		[SerializeField] private DailyGift         dailyGiftPopup;

		public HomeScene      HomeScene      => homeScene;
		public SettingsPopup  SettingsPopup  => settingsPopup;
		public RemoveAdsPopup RemoveAdsPopup => removeAdsPopup;
		public LevelSelect    LevelSelect    => levelSelect;
		public RateUs         RateUsPopup    => rateUsPopup;
		public DailyGift      DailyGift      => dailyGiftPopup;

		public void OpenPopup(UIPopups popup)
		{
			switch (popup)
			{
				case UIPopups.Main:
					settingsPopup.gameObject.SetActive(false);
					removeAdsPopup.gameObject.SetActive(false);
					levelSelect.gameObject.SetActive(false);
					rateUsPopup.gameObject.SetActive(false);
					dailyGiftPopup.gameObject.SetActive(false);
					break;
				case UIPopups.Settings:
					settingsPopup.gameObject.SetActive(true);
					settingsPopup.EnableButtonQuit();
					settingsPopup.ShowPopup();
					break;
				case UIPopups.RemoveAds:
					removeAdsPopup.gameObject.SetActive(true);
					removeAdsPopup.ShowPopup();
					break;
				case UIPopups.LevelSelect:
					levelSelect.gameObject.SetActive(true);
					levelSelect.ShowPopup();
					break;
				case UIPopups.Rateus:
					rateUsPopup.gameObject.SetActive(true);
					rateUsPopup.ShowPopup();
					break;
				case UIPopups.DailyGift:
					dailyGiftPopup.gameObject.SetActive(true);
					dailyGiftPopup.ShowPopup();
					break;
			}
		}
	}
}
