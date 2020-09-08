using System.Collections;
using System.Collections.Generic;
using FeedTheBaby.GameData;
using FeedTheBaby.UI;
using TMPro;
using UnityEngine;

public class SkillSelectUI : DisableablePanel
{
    [SerializeField] TextMeshProUGUI numberOfStars = null;

    public override void Enable()
    {
        base.Enable();
        numberOfStars.text = DataService.Instance.GetStarsCollected().ToString();
    }
}
