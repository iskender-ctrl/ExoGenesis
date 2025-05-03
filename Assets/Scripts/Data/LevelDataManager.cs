using System.Collections.Generic;

[System.Serializable]
public class PlanetDataLevel
{
    public string name;
    public string type;
    public float[] position;
    public float scale;
}

[System.Serializable]
public class LevelData
{
    public int level;
    public List<PlanetDataLevel> planets;
    public string targetPlanet;
    public int targetPopulation;
}

[System.Serializable]
public class LevelDatabase
{
    public List<LevelData> levels;
}
