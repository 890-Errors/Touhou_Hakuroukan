using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ViridaxGameStudios.AI
{
    [Serializable]
    public class CandiceBehaviorNode
    {
        public int id;
        public int type;
        public List<int> childrenIDs;
        public int function = -1;
        public bool isRoot;
        public float x;
        public float y;
        public float width;
        public float height;
        public int number;

        public CandiceBehaviorNode(int id, int type, List<int> childrenIDs, int function, bool isRoot, float x, float y, float width, float height, int number)
        {
            this.id = id;
            this.type = type;
            this.childrenIDs = childrenIDs;
            this.function = function;
            this.isRoot = isRoot;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.number = number;
        }
    }
}
