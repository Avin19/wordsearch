using UnityEngine;
using UnityEngine.UI;

namespace WordSearch
{
    public class LBScoreCard : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_Text _playerRank, _playerID, _playerScore;

        [SerializeField] private Image _medalImg;

        public void SetData(int rank, string id, long score, Sprite medalSprite = null)
        {
            _playerID.text = id.ToString();
            _playerRank.text = rank.ToString();
            _playerScore.text = score.ToString();

            if (medalSprite == null)
                _medalImg.gameObject.SetActive(false);
            else
            {
                _medalImg.gameObject.SetActive(true);
                _medalImg.sprite = medalSprite;
            }
        }
    }
}