using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlanetLifeProgressManager : MonoBehaviour
{
    /* ---------- DATA ---------- */
    [System.Serializable] public class LifeData
    {
        public string planetName;
        public int    population;
        public List<string> unlockedDecorations = new();
        public int    currentStage = -1;        // son gösterilen prefab seviyesi
    }
    [System.Serializable] public class PlanetLifeSaveData
    { public List<LifeData> planets = new(); }

    public static PlanetLifeProgressManager Instance { get; private set; }

    /* ---------- TWEAKABLE ---------- */
    [Header("Weights / Caps")]
    [SerializeField] private int   maxPopulation     = 500;
    [SerializeField] private int   maxDecorations    = 10;

    [Header("Score → Stage Thresholds")]
    [SerializeField, Range(0,1)] private float grassT     = 0.20f;
    [SerializeField, Range(0,1)] private float greeneryT  = 0.50f;
    [SerializeField, Range(0,1)] private float treeT      = 0.80f;

    [Header("Stage Prefabs (order: grass, greenery, tree)")]
    [SerializeField] private GameObject grassPrefab;
    [SerializeField] private GameObject greeneryPrefab;
    [SerializeField] private GameObject treePrefab;

    /* ---------- RUNTIME POOLS ---------- */
    private readonly Dictionary<int, Queue<GameObject>> _pool        = new();  // stage → queue
    private readonly Dictionary<Transform, GameObject>  _activeStage = new();  // planet → active GO
    private PlanetLifeSaveData _save;
    private string _path;

    /* ---------- LIFE-CYCLE ---------- */
    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        _path = Path.Combine(Application.persistentDataPath, "planet_life.json");
        LoadFile();
        PrepareEmptyPools();
        DontDestroyOnLoad(this);                 // ister kalıcı, ister sil
    }

    /* ---------- PUBLIC API ---------- */
    public void RegisterDecoration(string planet, string decoId)
    {
        var d = GetOrCreate(planet);
        if (d.unlockedDecorations.Contains(decoId)) return;
        d.unlockedDecorations.Add(decoId);
        SaveFile();
    }

    public void UpdatePopulation(string planet, int newPop)
    {
        var d = GetOrCreate(planet);
        d.population = newPop;
        SaveFile();
    }

    public float GetLifeScore(string planet)
    {
        var d = GetOrCreate(planet);
        float popPct  = Mathf.Clamp01((float)d.population / maxPopulation);
        float decoPct = Mathf.Clamp01((float)d.unlockedDecorations.Count / maxDecorations);
        return (popPct + decoPct) * 0.5f;
    }

    public void ApplyVisuals(string planet, Transform root)
    {
        float score = GetLifeScore(planet);
        int   stage = ScoreToStage(score);

        // Önceki prefab’ı kapat
        if (_activeStage.TryGetValue(root, out var oldGo) && oldGo != null)
            oldGo.SetActive(false);

        if (stage >= 0)
        {
            // Havuzdan al veya Instantiate et
            if (!_pool.TryGetValue(stage, out var q)) { q = new Queue<GameObject>(); _pool[stage] = q; }
            GameObject go = q.Count > 0 ? q.Dequeue() : Instantiate(StagePrefab(stage));
            go.transform.SetParent(root, false);
            go.transform.localPosition = Vector3.up * 0.25f * stage;
            go.SetActive(true);
            _activeStage[root] = go;
        }

        // JSON’a kaydet
        GetOrCreate(planet).currentStage = stage;
        SaveFile();
    }

    /* ---------- HELPERS ---------- */
    private int ScoreToStage(float s) => s >= treeT ? 2 : s >= greeneryT ? 1 : s >= grassT ? 0 : -1;
    private GameObject StagePrefab(int s) => s switch
    {
        0 => grassPrefab,
        1 => greeneryPrefab,
        2 => treePrefab,
        _ => null
    };
    private void PrepareEmptyPools() { for (int i = 0; i <= 2; i++) _pool[i] = new Queue<GameObject>(); }
    private LifeData GetOrCreate(string p)
    {
        var d = _save.planets.Find(x => x.planetName == p);
        if (d != null) return d;
        d = new LifeData { planetName = p };
        _save.planets.Add(d);
        return d;
    }
    private void LoadFile()
    {
        _save = File.Exists(_path)
            ? JsonUtility.FromJson<PlanetLifeSaveData>(File.ReadAllText(_path))
            : new PlanetLifeSaveData();
    }
    private void SaveFile() =>
        File.WriteAllText(_path, JsonUtility.ToJson(_save, true));
}
