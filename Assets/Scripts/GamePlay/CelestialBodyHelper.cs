using System.Linq;
using UnityEngine;

public static class CelestialBodyHelper
{
    public static CelestialBodyData.CelestialBody FindBodyByName(string name)
    {
        return GameManager.Instance.celestialBodyData.celestialBodies
            .FirstOrDefault(b => b.bodyName == name);
    }
}
