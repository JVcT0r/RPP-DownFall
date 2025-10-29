using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveSystem
{
    private static string savePath = Application.persistentDataPath + "/save.json";

    public static void SaveGame(Player player)
    {
        var data = new SaveData();

        var p = player.transform.position;
        data.playerPosition[0] = p.x;
        data.playerPosition[1] = p.y;
        data.playerPosition[2] = p.z;

        data.currentHealth = player.CurrentHealth;

        data.pistolBullets = AmmoManager.pistolBullets;
        data.pistolMagazine = AmmoManager.pistolMagazine;
        data.shotgunBullets = AmmoManager.shotgunBullets;
        data.shotgunMagazine = AmmoManager.shotgunMagazine;

        data.currentScene = SceneManager.GetActiveScene().name;

        File.WriteAllText(savePath, JsonUtility.ToJson(data, true));
        Debug.Log("[SaveSystem] Jogo salvo em: " + savePath);
    }

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

        AmmoManager.pistolBullets = data.pistolBullets;
        AmmoManager.pistolMagazine = data.pistolMagazine;
        AmmoManager.shotgunBullets = data.shotgunBullets;
        AmmoManager.shotgunMagazine = data.shotgunMagazine;

        Debug.Log("[SaveSystem] Jogo carregado com sucesso!");
    }

    public static string GetSavedScene()
    {
        if (!File.Exists(savePath)) return "";
        var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(savePath));
        return data.currentScene;
    }
}