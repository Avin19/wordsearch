using UnityEngine;
using static WordSearch.UniversalConstants;

[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Objects/GameData")]
public class GameData : ScriptableObject
{
	public int MasterVolume = 0;
	public bool MuteEnabled = false;
	public SceneIndex PrevSceneIndex;
}
