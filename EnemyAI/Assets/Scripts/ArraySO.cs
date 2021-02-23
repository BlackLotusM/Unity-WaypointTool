using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "ArraySO_", menuName = "Scrip/ArraySO")]
public class ArraySO : ScriptableObject
{
    [Serializable]
    public class Property
    {
        public string name = string.Empty;
        public string value = string.Empty;
    }

    [Serializable]
    public class Item
    {
        public List<Property> properties = new List<Property>();
    }

    public List<Item> itemList = new List<Item>();
}
