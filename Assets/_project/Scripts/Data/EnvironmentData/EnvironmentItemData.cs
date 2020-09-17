using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DashTerritory
{
    public class EnvironmentItemData : ScriptableObject
    {
        [HorizontalGroup("Split", 55, LabelWidth = 75)]
        [HideLabel, PreviewField(55, ObjectFieldAlignment.Left)]
        public Texture icon;
        
        [VerticalGroup("Split/Meta")]
        public string name;

        [VerticalGroup("Split/Meta")] 
        public GameObject cellPrefab;
    }
}
