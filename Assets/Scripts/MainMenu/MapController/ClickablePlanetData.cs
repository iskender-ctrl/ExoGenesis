using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlanetDatabase", menuName = "Planet/Create Planet Database")]
public class ClickablePlanetDatabase : ScriptableObject
{
    [System.Serializable]
    public class DecorationItem
    {
        public string decorationName;
        public Sprite icon;
        public int cost;
    }
    
    [System.Serializable]
    public class PlanetData
    {
        public string planetName;
        public Sprite bG;
        public List<DecorationItem> items = new List<DecorationItem>();
    }

    public List<PlanetData> planets = new List<PlanetData>();
}
