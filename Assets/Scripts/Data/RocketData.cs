using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewRocketList", menuName = "Shop/Rocket List")]
public class RocketData : ScriptableObject
{
    [System.Serializable]
    public class Rocket
    {
        public string rocketName;
        public int price;
        public Sprite icon;
    }

    public List<Rocket> rockets = new List<Rocket>(); // Birden fazla roketi saklayan liste
}
