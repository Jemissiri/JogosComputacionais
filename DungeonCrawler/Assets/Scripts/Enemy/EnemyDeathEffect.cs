using UnityEngine;

public class EnemyDeathEffect : MonoBehaviour
{
    public static void Spawn(Vector3 position, Color color)
    {
        var go = new GameObject("DeathEffect");
        go.transform.position = position;

        var ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.duration        = 0.5f;
        main.loop            = false;
        main.startLifetime   = new ParticleSystem.MinMaxCurve(0.5f, 1.2f);
        main.startSpeed      = new ParticleSystem.MinMaxCurve(2f, 7f);
        main.startSize       = new ParticleSystem.MinMaxCurve(0.1f, 0.35f);
        main.startColor      = new ParticleSystem.MinMaxGradient(color, Color.white);
        main.gravityModifier = 0.4f;
        main.maxParticles    = 20;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 20) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius    = 0.4f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        var gradient = new Gradient();
        gradient.SetKeys(
            new[] { new GradientColorKey(color, 0f), new GradientColorKey(Color.white, 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = gradient;

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));

        go.AddComponent<EnemyDeathEffect>().ps = ps;
        ps.Play();
    }

    ParticleSystem ps;

    void Update()
    {
        if (ps != null && !ps.IsAlive())
            Destroy(gameObject);
    }
}
