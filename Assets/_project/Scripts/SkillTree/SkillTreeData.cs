using System;
using UnityEngine;
using UnityEngine.UI;

namespace FeedTheBaby.SkillTree
{
    [CreateAssetMenu(fileName = "Skill Tree Data", menuName = "Feed The Baby/Skill Tree Data", order = 0)]
    public class SkillTreeData : ScriptableObject
    {
        [SerializeField] public SkillPath[] skillTree;
        public int[] levelledSkills;
        bool _firstTimeLoad = true;

        int[] CreateSkillsTracker()
        {
            return new int[skillTree.Length];
        }

        public void SetFirstTime()
        {
            if (_firstTimeLoad)
            {
                _firstTimeLoad = false;
                levelledSkills = CreateSkillsTracker();
            }
        }
    }

    [Serializable]
    public struct SkillPath
    {
        [SerializeField] public Skill[] skills;
    }

    [Serializable]
    public struct Skill
    {
        [SerializeField] public Image image;
        [SerializeField] public string name;
        [SerializeField] public int cost;
    }
}