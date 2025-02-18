using System.Collections.Generic;

[System.Serializable]
public class PlanetData
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
    public List<PlanetData> planets;
    public string targetPlanet;
}

[System.Serializable]
public class LevelDatabase
{
    public List<LevelData> levels;
}
