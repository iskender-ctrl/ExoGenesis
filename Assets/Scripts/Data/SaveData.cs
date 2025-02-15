using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public List<string> activeObjects = new List<string>();  // Aktif objeler
    public List<string> removedItems = new List<string>();   // Silinen liste öğeleri
}

