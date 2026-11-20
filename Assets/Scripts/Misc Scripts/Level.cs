using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level", order = 0)]
public class Level : ScriptableObject
{
    [SerializeField] private int levelIndex;
    [SerializeField] private string levelName;
    [SerializeField] private GameObject loadingScreen;

    public int LevelIndex { get => levelIndex; set => levelIndex = value; }
    public string LevelName { get => levelName; set => levelName = value; }
    public GameObject LoadingScreen { get => loadingScreen; set => loadingScreen = value; }
}