using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [Header("Posição inicial do jogador")]
    public Vector3 startPosition = Vector3.zero;

    public void ApplyInitialSettings(Player player)
    {
        if (player == null) return;

        // -------------------------------
        // ➤ VIDA
        // -------------------------------
        player.CurrentHealth = player.maxHealth;
        player.transform.position = startPosition;

        // -------------------------------
        // ➤ ARMAS (player começa SEM armas)
        // -------------------------------
        if (player.TryGetComponent<WeaponManager>(out var wm))
        {
            wm.pistolUnlocked = false;
            wm.shotgunUnlocked = false;
            wm.SetWeapon(WeaponType.None);
        }

        // -------------------------------
        // ➤ MUNIÇÃO INICIAL (ZERO)
        // -------------------------------
        AmmoManager.pistolBullets = 0;
        AmmoManager.pistolMagazine = 0;
        AmmoManager.shotgunBullets = 0;
        AmmoManager.shotgunMagazine = 0;

        // -------------------------------
        // ➤ POÇÕES INICIAIS (ZERO)
        // -------------------------------
        HealthManager.potionCount = 0;

        Debug.Log("[GameSettings] Novo jogo iniciado com valores zerados.");
    }
}