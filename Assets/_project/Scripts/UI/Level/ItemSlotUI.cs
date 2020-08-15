using FeedTheBaby.GameData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FeedTheBaby.UI
{
    public class ItemSlotUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI amountText = null;
        [SerializeField] Image image = null;

        ItemType _type;

        public void SetItemType(ItemType type)
        {
            image.enabled = true;
            var itemUiData = DataService.Instance.GetItemUIData();
            image.sprite = itemUiData.GetSprite(type);
        }

        public void SetAmount(int amount)
        {
            amountText.text = amount.ToString();
        }

        public void SetEmpty()
        {
            image.enabled = false;
            amountText.text = "";
        }

        public void SetItemAmount(ItemAmount itemAmount)
        {
            SetItemType(itemAmount.type);
            SetAmount(itemAmount.amount);
        }
    }
}