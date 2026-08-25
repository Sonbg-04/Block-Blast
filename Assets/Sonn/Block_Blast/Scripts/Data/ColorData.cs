using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BlockBlast
{
    [CreateAssetMenu(menuName = "Sonn/Data/Create New Color Data")]
    public class ColorData : ScriptableObject
    {
        public List<ColorBlockDefinition> colorBlockDefinitions;

        public Color GetColorForSprite(Sprite sp)
        {
            if (sp == null)
            {
                return Color.white;
            }    
            for (int i = 0; i < colorBlockDefinitions.Count; i++)
            {
                if (colorBlockDefinitions[i].block == sp)
                {
                    return colorBlockDefinitions[i].blockColor;
                }    
            }    
            return Color.white;
        }    
    }

    [Serializable]
    public class ColorBlockDefinition
    {
        public Sprite block;
        public Color blockColor;
    }
}

