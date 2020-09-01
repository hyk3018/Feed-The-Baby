using FeedTheBaby.Commands;
using UnityEngine;

namespace FeedTheBaby.UI
{
    [CreateAssetMenu(fileName = "Command Button Data", menuName = "Feed The Baby/Command Button Data", order = 0)]
    public class CommandButtonMap : ScriptableObject
    {
        [SerializeField]
        public CommandButtonDictionary dictionary = CommandButtonDictionary.New<CommandButtonDictionary>();

        public GameObject GetPrefab(CommandType type)
        {
            return dictionary.dictionary.ContainsKey(type) ? dictionary.dictionary[type] : null;
        }
    }
}