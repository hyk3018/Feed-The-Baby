using System;
using UnityEngine;

namespace FeedTheBaby.LevelObjects
{
    public abstract class LevelObject : MonoBehaviour
    {
        public Action DestroyTile;
    }
}