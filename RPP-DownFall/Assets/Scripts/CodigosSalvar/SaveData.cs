using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    public float[] playerPosition = new float[3];
    public int currentHealth;

    public int pistolBullets;
    public int pistolMagazine;
    public int shotgunBullets;
    public int shotgunMagazine;

    public string currentScene;
}
