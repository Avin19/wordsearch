using UnityEngine;
using static WordSearch.UniversalConstants;

[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Objects/GameData")]
public class GameData : ScriptableObject
{
	public int MasterVolume = 0;
	public bool MuteEnabled = false;
	public SceneIndex PrevSceneIndex;
	public bool LoadingPanelAvailable;
	public int PlayerCoinCount = 0, DailyLoginCount = 0;
	public bool RewardCollected = false;
}
