using UnityEngine;

public class BlinkGhost : MonoBehaviour
{
    const float Duration = 0.35f;

    Material[] mats;
    float timer = Duration;

    public static void Spawn(Transform character)
    {
        var renderers = character.GetComponentsInChildren<SkinnedMeshRenderer>();
        if (renderers.Length == 0) return;

        var ghost = new GameObject("BlinkGhost").AddComponent<BlinkGhost>();
        ghost.mats = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            var baked = new Mesh();
            renderers[i].BakeMesh(baked);

            var part = new GameObject();
            part.transform.SetParent(ghost.transform, false);
            part.transform.SetPositionAndRotation(
                renderers[i].transform.position,
                renderers[i].transform.rotation);
            part.transform.localScale = renderers[i].transform.lossyScale;

            part.AddComponent<MeshFilter>().mesh = baked;
            var mr = part.AddComponent<MeshRenderer>();

            var mat = new Material(Shader.Find("Standard"));
            MakeTransparent(mat);
            mat.color = new Color(0.1f, 0.85f, 1f, 0.85f);
            mr.material = mat;
            ghost.mats[i] = mat;
        }
    }

    public static void SpawnTrail(Vector3 from, Vector3 to)
    {
        var go = new GameObject("BlinkTrail");
        var lr = go.AddComponent<LineRenderer>();

        float height = 0.8f;
        lr.positionCount = 2;
        lr.SetPosition(0, from + Vector3.up * height);
        lr.SetPosition(1, to   + Vector3.up * height);
        lr.startWidth = 0.35f;
        lr.endWidth   = 0.05f;

        var mat = new Material(Shader.Find("Sprites/Default"));
        lr.material = mat;
        lr.startColor = new Color(0.1f, 0.85f, 1f, 0.9f);
        lr.endColor   = new Color(0.0f, 0.55f, 0.9f, 0.4f);
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows    = false;

        go.AddComponent<BlinkTrailFader>().Init(lr);
    }

    static void MakeTransparent(Material mat)
    {
        mat.SetFloat("_Mode", 3);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        float alpha = Mathf.Clamp01(timer / Duration) * 0.85f;
        foreach (var m in mats)
            m.color = new Color(m.color.r, m.color.g, m.color.b, alpha);

        if (timer <= 0f)
            Destroy(gameObject);
    }
}

public class BlinkTrailFader : MonoBehaviour
{
    const float Duration = 0.3f;
    LineRenderer lr;
    float timer = Duration;

    public void Init(LineRenderer lineRenderer) => lr = lineRenderer;

    void Update()
    {
        timer -= Time.deltaTime;
        float t = Mathf.Clamp01(timer / Duration);
        lr.startColor = new Color(0.1f, 0.85f, 1f,  0.9f * t);
        lr.endColor   = new Color(0.0f, 0.55f, 0.9f, 0.4f * t);

        if (timer <= 0f)
            Destroy(gameObject);
    }
}
