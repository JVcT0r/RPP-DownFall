using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveSystem
{
    private static readonly string savePath = Path.Combine(Application.persistentDataPath, "save.json");

    [System.Serializable]
    public class SaveData
    {
        public float[] playerPosition = new float[3];
        public int currentHealth;

        public int pistolBullets, pistolMagazine, pistolReserve;
        public int shotgunBullets, shotgunMagazine, shotgunReserve;

        public int potionCount;
        public string currentScene;
    }

    // ---------------- INICIALIZAÇÃO AUTOMÁTICA ----------------
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnSceneLoadedInit()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!HasSave()) return;

        string json = File.ReadAllText(savePath);
        var data = JsonUtility.FromJson<SaveData>(json);

        if (data.currentScene == scene.name)
        {
            var player = Object.FindAnyObjectByType<Player>();
            if (player != null)
            {
                LoadGame(player);
                Debug.Log($"[SaveSystem] Jogo carregado automaticamente ({scene.name})");
            }
        }
    }

    // ---------------- SALVAR ----------------
    public static void SaveGame(Player player)
    {
        var data = new SaveData();

        var p = player.transform.position;
        data.playerPosition[0] = p.x;
        data.playerPosition[1] = p.y;
        data.playerPosition[2] = p.z;

        data.currentHealth = player.CurrentHealth;

        data.pistolBullets  = AmmoManager.pistolBullets;
        data.pistolMagazine = AmmoManager.pistolMagazine;

        data.shotgunBullets  = AmmoManager.shotgunBullets;
        data.shotgunMagazine = AmmoManager.shotgunMagazine;

        data.potionCount = HealthManager.potionCount;

        data.currentScene = SceneManager.GetActiveScene().name;

        File.WriteAllText(savePath, JsonUtility.ToJson(data, true));
        Debug.Log("[SaveSystem] Jogo salvo em: " + savePath);
    }

    // ---------------- APAGAR SAVE ----------------
    public static void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("[SaveSystem] Save deletado!");
        }
    }

    // ---------------- VERIFICAR SAVE ----------------
    public static bool HasSave() => File.Exists(savePath);

    public static string GetSavedScene()
    {
        if (!File.Exists(savePath)) return null;

        string json = File.ReadAllText(savePath);
        var data = JsonUtility.FromJson<SaveData>(json);
        return data.currentScene;
    }

    // ---------------- CARREGAR ----------------
    public static void LoadGame(Player player)
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("[SaveSystem] Nenhum save encontrado!");
            return;
        }

        var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(savePath));

        player.transform.position = new Vector3(
            data.playerPosition[0],
            data.playerPosition[1],
            data.playerPosition[2]
        );

        player.CurrentHealth = data.currentHealth;

        AmmoManager.pistolBullets  = data.pistolBullets;
        AmmoManager.pistolMagazine = data.pistolMagazine;

        AmmoManager.shotgunBullets  = data.shotgunBullets;
        AmmoManager.shotgunMagazine = data.shotgunMagazine;

        HealthManager.potionCount = data.potionCount;

        var hud = Object.FindAnyObjectByType<HUDManager>();
        if (hud != null)
            hud.SendMessage("UpdatePotionUI", SendMessageOptions.DontRequireReceiver);
    }
}
