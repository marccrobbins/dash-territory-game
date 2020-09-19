using Sirenix.OdinInspector;
using UnityEngine;

namespace DashTerritory
{
    public abstract class EnvironmentData : ScriptableObject, IDataDraggable
    {
        public abstract string SubDirectory { get; }
        
        [HorizontalGroup("Split", 55, LabelWidth = 75)]
        [HideLabel, PreviewField(55, ObjectFieldAlignment.Left)]
        public Texture icon;
        
        [VerticalGroup("Split/Meta")]
        public string name;

        [VerticalGroup("Split/Meta")] 
        public GameObject prefab;
    }
}
