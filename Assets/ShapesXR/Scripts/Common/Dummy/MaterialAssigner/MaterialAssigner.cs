using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShapesXr
{
    [Serializable]
    public class MaterialGroup
    {
        [SerializeField] private List<Renderer> _renderers = new List<Renderer>();

        public List<Renderer> Renderers => _renderers;
    }
    
    public class MaterialAssigner : RemoveAfterImportBehaviour
    {
        [SerializeField] protected bool _onlyColor;
        [SerializeField] protected List<MaterialGroup> _groups = new List<MaterialGroup>();

        public List<MaterialGroup> Groups => _groups;
    }
}