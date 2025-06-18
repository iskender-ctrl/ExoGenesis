using System.Collections.Generic;

[System.Serializable]
public class PlanetDataLevel
{
    public string name;
    public string type;
    public float[] position;
    public float scale;
    public string orbitAround; // ← yeni eklendi
}

[System.Serializable]
public class LevelData
{
    public int level;
    public GravitySettingsData gravitySettings; // ← yeni
    public List<PlanetDataLevel> planets;
    public string targetPlanet;
    public int targetPopulation;
}

[System.Serializable]
public class LevelDatabase
{
    public List<LevelData> levels;
}
[System.Serializable]
public class GravitySettingsData
{
    public float gravitationalConstant = 0.1f;
    public float radiusMultiplier = 1f;
    public float orbitalSpeedMultiplier = 1f;
}

