using System;
using FeedTheBaby.Commands;
using FeedTheBaby.Dictionary;
using UnityEngine;

namespace FeedTheBaby.UI
{
    [Serializable]
    public class CommandButtonDictionary : SerializableDictionary<CommandType, GameObject>
    {
    }
}