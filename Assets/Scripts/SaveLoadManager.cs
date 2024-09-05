using UnityEngine;

public enum PersistentKeys
{
    Score,
    Moves,
    Timer
}
public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }  // Singleton pattern

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame(PersistentKeys key, int value)
    {
        PlayerPrefs.SetInt(key.ToString(), value);
        PlayerPrefs.Save();
    }

    public int LoadScore(PersistentKeys key)
    {
        return PlayerPrefs.GetInt(key.ToString(), 0);
    }
}
