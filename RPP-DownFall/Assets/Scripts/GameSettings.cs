using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "DownFall/Game Settings")]
public class GameSettings : ScriptableObject
{
    [Header("Vida Inicial")]
    public int startHealth = 3;

    [Header("Poções")]
    public int startPotions = 3;
    public int maxPotions = 9;

    [Header("Pistola")]
    public int pistolStartBullets = 12;
    public int pistolStartMagazine = 60;

    [Header("Shotgun")]
    public int shotgunStartBullets = 6;
    public int shotgunStartMagazine = 30;

    [Header("Armas Liberadas")]
    public bool pistolUnlocked = true;
    public bool shotgunUnlocked = true;
}