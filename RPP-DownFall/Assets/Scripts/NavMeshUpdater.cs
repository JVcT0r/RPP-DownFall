using UnityEngine;
using System.Reflection;

public class NavMeshUpdater : MonoBehaviour
{
    // guardamos o componente “Surface” encontrado (seja NavigationSurface, NavMeshSurface, etc.)
    private static Component globalSurface;

    void Awake()
    {
        if (globalSurface == null)
            TryFindSurface();
    }

    /// <summary>Força rebake do NavMesh (funciona em qualquer variante).</summary>
    public static void UpdateNavMeshGlobal()
    {
        if (globalSurface == null && !TryFindSurface())
        {
            Debug.LogWarning("[NavMeshUpdater] Nenhuma Surface de navegação encontrada na cena.");
            return;
        }

        // chama BuildNavMesh() por reflection
        var m = globalSurface.GetType().GetMethod(
            "BuildNavMesh",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );

        if (m == null)
        {
            Debug.LogWarning($"[NavMeshUpdater] O tipo {globalSurface.GetType().Name} não possui BuildNavMesh().");
            return;
        }

        m.Invoke(globalSurface, null);
        
    }

    private static bool TryFindSurface()
    {
        // Percorre TODOS os componentes da cena e procura por tipos com nome “NavMeshSurface” ou “NavigationSurface”
        var comps = FindObjectsByType<Component>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (var c in comps)
        {
            var n = c.GetType().Name;
            if (n == "NavMeshSurface" || n == "NavigationSurface")
            {
                globalSurface = c;
                return true;
            }
        }

        return false;
    }
}