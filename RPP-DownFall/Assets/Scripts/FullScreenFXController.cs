using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FullScreenFXController : MonoBehaviour
{
   [Header("References")] 
   [SerializeField]
   private ScriptableRendererFeature _fullScreenDamage;
   [SerializeField]
   private Material _material;
   
   private int _voronoiIntensity = Shader.PropertyToID("_VoronoiIntensity");
   private int _vignetteIntensity = Shader.PropertyToID("_VignetteIntensity");

   private void Start()
   {
      _fullScreenDamage.SetActive(false);
   }

   public void Hurt()
   {
      _fullScreenDamage.SetActive(true);
   }

   public void NotHurt()
   {
      _fullScreenDamage.SetActive(false);
   }

}
