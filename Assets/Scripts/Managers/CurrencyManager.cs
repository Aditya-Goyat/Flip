using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    private int totalCores;
    private int runCores; // How many collected this specific run

    public int TotalCores => totalCores;
    public int RunCores => runCores;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Load saved currency from device
        totalCores = PlayerPrefs.GetInt("TOTAL_CORES", 0);
    }

    public void StartRun()
    {
        runCores = 0; // Reset run counter when restarting/playing
    }

    public void AddCore(int amount = 1)
    {
        runCores += amount;
        totalCores += amount;

        // Save immediately so they don't lose it if they close the app
        PlayerPrefs.SetInt("TOTAL_CORES", totalCores);
        PlayerPrefs.Save();
    }
}