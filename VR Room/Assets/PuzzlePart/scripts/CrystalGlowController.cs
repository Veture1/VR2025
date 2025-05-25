using UnityEngine;
using System.Collections;

public class CrystalGlowController : MonoBehaviour
{
    [SerializeField] private float emissionMultiplier = 3f; 
    [SerializeField] private float duration = 1f; 

    private Material[] instanceMaterials;

    void Start()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        instanceMaterials = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {

            Material mat = new Material(renderers[i].sharedMaterial);
            renderers[i].material = mat;
            instanceMaterials[i] = mat;

            // ✅ Debug：确认初始 EmissionPower
            if (mat.HasProperty("_EmissionPower"))
            {
                Debug.Log($"[{gameObject.name}] [{i}] Initial _EmissionPower: {mat.GetFloat("_EmissionPower")}");
            }
        }
    }

    public void ActivateGlow()
    {
        StartCoroutine(AnimateGlow());
    }

    private IEnumerator AnimateGlow()
    {
        float time = 0f;

        Color[] originalColors = new Color[instanceMaterials.Length];

        for (int i = 0; i < instanceMaterials.Length; i++)
        {
            originalColors[i] = instanceMaterials[i].GetColor("_EmissionColor");
        }

        while (time < duration)
        {
            float t = time / duration;
            float factor = Mathf.Lerp(1f, emissionMultiplier, t);

            for (int i = 0; i < instanceMaterials.Length; i++)
            {
                Color boosted = originalColors[i] * factor;
                instanceMaterials[i].SetColor("_EmissionColor", boosted);
                instanceMaterials[i].SetFloat("_EmissionPower", factor); // 可选：设置 power
            }

            time += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < instanceMaterials.Length; i++)
        {
            Color boosted = originalColors[i] * emissionMultiplier;
            instanceMaterials[i].SetColor("_EmissionColor", boosted);
            instanceMaterials[i].SetFloat("_EmissionPower", emissionMultiplier); // 可选：确保最终 power
        }

        Debug.Log($"[CrystalGlow] Animated glow completed on {gameObject.name}");
    }
}
