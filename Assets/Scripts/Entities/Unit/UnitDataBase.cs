using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/UnitDataBase")]
public class UnitDataBase : ScriptableObject
{
    public List<UnitData> allUnits;

        public UnitData Get(string name)
        {
            return allUnits.Find(x => x.Name == name);
        }
}