using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using static WordSearch.UniversalConstants;

namespace WordSearch
{
    public class LoadingPanelManager : MonoBehaviour
    {
        //			TRANSITION
        [Header("Transition")]
        [SerializeField] private RectTransform _loadingPanel;
        [SerializeField] private Image _loadingBarFillImg;

        private WaitForSeconds _waitFor1Second;

        private float _loadingBarSize;

        void Start()
        {
            _waitFor1Second = new WaitForSeconds(1f);

            Vector2 barSize = _loadingBarFillImg.rectTransform.sizeDelta;
            _loadingBarSize = barSize.x;
            barSize.x = 0f;
            _loadingBarFillImg.rectTransform.sizeDelta = barSize;

            GameEvents.OnLoadingUpdate += UpdateLoadingBarUI;
        }

        private void UpdateLoadingBarUI(LoadingPanelStatus status, float value)
        {
            Debug.Log($"UpdateLoadingBarUI called | status: {status}");
            switch (status)
            {
                case LoadingPanelStatus.DISABLE:
                    _loadingPanel.gameObject.SetActive(false);

                    break;

                case LoadingPanelStatus.ENABLE:
                    _loadingPanel.gameObject.SetActive(true);
                    StartCoroutine(LoadSceneAsync((int)value));

                    break;

                case LoadingPanelStatus.UPDATE:
                    Vector2 barSize = _loadingBarFillImg.rectTransform.sizeDelta;
                    barSize.x = value * _loadingBarSize;

                    _loadingBarFillImg.rectTransform.sizeDelta = barSize;
                    break;
            }
        }

        //TODO: Load additive and remove loading screen only after the level is properly loaded
        private IEnumerator LoadSceneAsync(int sceneIndex)
        {
            yield return _waitFor1Second;                     // Wait a second before loading

            Vector2 barSize = _loadingBarFillImg.rectTransform.sizeDelta;
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);     //, LoadSceneMode.Additive);

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