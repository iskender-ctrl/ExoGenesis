using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewRocketList", menuName = "Shop/Rocket List")]
public class RocketData : ScriptableObject
{
    public enum PaymentType   // <<< Yeni
    {
        Gold,     // oyun-içi para
        IAP       // gerçek para / mağaza ürünü
    }

    [System.Serializable]
    public class Rocket
    {
        public string rocketName;
        public PaymentType payment;   // <<< Yeni alan
        public int price;             // Gold için kullanılacak
        public string iapProductId;   // IAP ise Store’daki ürün kimliği (Google, Apple…)
        public Sprite icon;
        public bool isDefault;
    }

    public List<Rocket> rockets = new();   // C# 9 kısaltması
    public Rocket DefaultRocket => rockets.Find(r => r.isDefault);
}
