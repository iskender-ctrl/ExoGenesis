using System.Collections.Generic;

[System.Serializable]           // JSON’da ana model
public class SaveData
{
    public List<string> activeObjects = new();          // Dekorasyonda açılan objeler
    public List<string> removedItems = new();          // Liste dışı bırakılan item’ler
    public List<PlanetPopulation> planetPopulations = new(); // Nüfus verisi
    public List<PlanetEvolution> evolutions = new();
}

[System.Serializable]           // (anahtar-değer çiftini listede tutuyoruz)
public class PlanetPopulation
{
    public string planetName;
    public int population;
}
[System.Serializable]
public class PlanetEvolution
{
    public string planetName;
    public int stage;           // currentEvolutionStage
}