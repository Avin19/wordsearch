using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using static WordSearch.UniversalConstants;

namespace WordSearch
{
	public class MainMenuManagerUI : MonoBehaviour
	{
		enum UIInteraction
		{
			BUY_COINS_REQ, DAILY_REWARD_REQ,
			PLAY_GAME_REQ, DAILY_PUZZLE_REQ, LEVEL_REQ, LEADERBOARD_REQ,
			SETTINGS_REQ, TOGGLE_SOUND_REQ, RATE_REQ, PRIVACY_REQ,
		}

		enum UISprite
		{
			MUTE_ON, MUTE_OFF
		}

		//			BUTTONS
		[Header("Buttons")]
		[SerializeField] private Button _buyCoinsBt;
		[SerializeField] private Button _dailyRewardBt;
		[SerializeField] private Button _playBt, _dailyPuzzleBt, _levelMapBt, _leaderBoardBt;
		[SerializeField] private Button _settingsBt, _soundBt, _rateUsBt, _privacyBt;

		[SerializeField] private Button _closeBuyCoinsBt, _closeDailyRewardBt;
		[SerializeField] private Button _closeLevelMapBt, _closeLeaderBoardBt;
		[SerializeField] private Button _closeSettingsBt, _closePrivacyBt;

		//			PANELS
		[Header("Panels")]
		[SerializeField] private RectTransform _mainMenuPanel;
		[SerializeField] private RectTransform _buyCoinsPanel, _dailyRewardsPanel;
		[SerializeField] private RectTransform _levelMapPanel, _leaderBoardPanel;
		[SerializeField] private RectTransform _settingsPanel, _privacyPanel;

		//			TRANSITION
		[Header("Transition")]
		[SerializeField] private RectTransform _loadingPanel;
		[SerializeField] private Image _loadingBarFillImg;

		private float _loadingBarSize;

		//			IMAGES
		[Header("Images")]
		[SerializeField] private Sprite[] _uiSprites;
		[SerializeField] private Image _soundBtImg;

		[SerializeField] private GameData _gameData;

		private void Start()
		{
			Vector2 barSize = _loadingBarFillImg.rectTransform.sizeDelta;
			_loadingBarSize = barSize.x;
			barSize.x = 0f;
			_loadingBarFillImg.rectTransform.sizeDelta = barSize;

			//				OPEN
			_buyCoinsBt.onClick.AddListener(() => HandleUIInteraction(UIInteraction.BUY_COINS_REQ, true));
			_dailyRewardBt.onClick.AddListener(() => HandleUIInteraction(UIInteraction.DAILY_REWARD_REQ, true));

			_playBt.onClick.AddListener(() => HandleUIInteraction(UIInteraction.PLAY_GAME_REQ));
			_dailyPuzzleBt.onClick.AddListener(() => HandleUIInteraction(UIInteraction.DAILY_PUZZLE_REQ));
			_levelMapBt.onClick.AddListener(() => HandleUIInteraction(UIInteraction.LEVEL_REQ, true));
			_leaderBoardBt.onClick.AddListener(() => HandleUIInteraction(UIInteraction.LEADERBOARD_REQ, true));

			_settingsBt.onClick.AddListener(() => HandleUIInteraction(UIInteraction.SETTINGS_REQ, true));
			_soundBt.onClick.AddListener(() => HandleUIInteraction(UIInteraction.TOGGLE_SOUND_REQ));
			_rateUsBt.onClick.AddListener(() => HandleUIInteraction(UIInteraction.RATE_REQ));
			_privacyBt.onClick.AddListener(() => HandleUIInteraction(UIInteraction.PRIVACY_REQ, true));
			_buyCoinsBt.onClick.AddListener(() => HandleUIInteraction(UIInteraction.BUY_COINS_REQ, true));

			//				CLOSE
			_closeBuyCoinsBt.onClick.AddListener(() => HandleUIInteraction(UIInteraction.BUY_COINS_REQ, false));
			_closeDailyRewardBt.onClick.AddListener(() => HandleUIInteraction(UIInteraction.DAILY_REWARD_REQ, false));

			_closeLevelMapBt.onClick.AddListener(() => HandleUIInteraction(UIInteraction.LEVEL_REQ, false));
			_closeLeaderBoardBt.onClick.AddListener(() => HandleUIInteraction(UIInteraction.LEADERBOARD_REQ, false));

			_closeSettingsBt.onClick.AddListener(() => HandleUIInteraction(UIInteraction.SETTINGS_REQ, false));
			_closePrivacyBt.onClick.AddListener(() => HandleUIInteraction(UIInteraction.PRIVACY_REQ, false));
		}

		private void HandleUIInteraction(UIInteraction interaction, bool status = false)
		{
			GameEvents.OnButtonClick?.Invoke();
			switch (interaction)
			{
				case UIInteraction.BUY_COINS_REQ:
					_buyCoinsPanel.gameObject.SetActive(status);

					break;

				case UIInteraction.DAILY_REWARD_REQ:
					_dailyRewardsPanel.gameObject.SetActive(status);

					break;

				case UIInteraction.PLAY_GAME_REQ:
					_mainMenuPanel.gameObject.SetActive(false);
					StartCoroutine(LoadMainGameplay());

					return;

				case UIInteraction.DAILY_PUZZLE_REQ:
					break;

				case UIInteraction.LEVEL_REQ:
					_levelMapPanel.gameObject.SetActive(status);

					break;

				case UIInteraction.LEADERBOARD_REQ:
					_leaderBoardPanel.gameObject.SetActive(status);

					break;

				case UIInteraction.SETTINGS_REQ:
					_settingsPanel.gameObject.SetActive(status);

					break;

				case UIInteraction.TOGGLE_SOUND_REQ:
					GameEvents.OnToggleMute?.Invoke((muteEnabled) =>
					{
						if (muteEnabled)
							_soundBtImg.sprite = _uiSprites[(int)UISprite.MUTE_ON];
						else
							_soundBtImg.sprite = _uiSprites[(int)UISprite.MUTE_OFF];

						_gameData.MuteEnabled = muteEnabled;
					});

					break;

				case UIInteraction.RATE_REQ:
#if UNITY_ANDROID
					Application.OpenURL("market://details?id=" + Application.identifier);
#else
					Debug.Log($"Rate us clicked!");
#endif

					break;

				case UIInteraction.PRIVACY_REQ:
					_privacyPanel.gameObject.SetActive(status);

					break;
			}
		}

		//TODO: Load additive and remove loading screen only after the level is properly loaded
		private IEnumerator LoadMainGameplay()
		{
			_loadingPanel.gameObject.SetActive(true);
			Vector2 barSize = _loadingBarFillImg.rectTransform.sizeDelta;
			AsyncOperation loadOp = SceneManager.LoadSceneAsync((int)SceneIndex.MAIN_GAMEPLAY);     //, LoadSceneMode.Additive);

			// float testProgress = 0f;
			// float testMult = 0.25f;

			// while (testProgress < 1f)
			while (!loadOp.isDone)
			{
				// testProgress += Time.deltaTime * testMult;
				// barSize.x = testProgress * _loadingBarSize;

				float progress = Mathf.Clamp01(loadOp.progress / 0.9f);
				barSize.x = progress * _loadingBarSize;

				_loadingBarFillImg.rectTransform.sizeDelta = barSize;
				// Debug.Log("Loading progress: " + (progress * 100) + "%");

				yield return null;
			}
			barSize.x = _loadingBarSize;
			_loadingBarFillImg.rectTransform.sizeDelta = barSize;
		}
	}
}