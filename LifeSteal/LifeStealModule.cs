using System;
using System.Collections;
using UnityEngine;
using ThunderRoad;

namespace LifeSteal
{
    public class LifeStealModule : ItemModule
    {
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<LifeSteal>();
        }
    }

    
}
