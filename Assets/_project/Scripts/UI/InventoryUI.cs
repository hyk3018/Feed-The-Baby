using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FeedTheBaby.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] ItemUIData itemUiData = null;
        [SerializeField] Inventory inventory = null;
        [SerializeField] GameObject itemSlotTemplate = null;

        Transform _uiContainer = null;

        readonly List<GameObject> _itemSlots = new List<GameObject>();

        void Awake()
        {
            _uiContainer = transform.Find("ItemSlotsContainer");
            for (var i = 0; i < 5; i++)
            {
                var itemSlot = Instantiate(itemSlotTemplate, _uiContainer);
                itemSlot.transform.localPosition = new Vector3(i * 52 - 148, 0, 0);

                var images = itemSlot.GetComponentsInChildren<Image>();
                images[1].enabled = false;

                var text = itemSlot.GetComponentInChildren<TextMeshProUGUI>();
                text.text = "";

                _itemSlots.Add(itemSlot);
            }

            inventory.InventoryChange += UpdateUI;
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void UpdateUI(List<ItemAmount> items)
        {
            for (var i = 0; i < _itemSlots.Count; i++)
            {
                var itemSlot = _itemSlots[i];
                var images = itemSlot.GetComponentsInChildren<Image>();
                var text = itemSlot.GetComponentInChildren<TextMeshProUGUI>();

                if (i < items.Count)
                {
                    var item = items[i];
                    images[1].enabled = true;
                    images[1].sprite = itemUiData.itemSpriteDictionary.dictionary[item.itemName];
                    text.text = item.amount.ToString();
                }
                else
                {
                    images[1].enabled = false;
                    images[1].sprite = null;
                    text.text = "";
                }
            }
        }
    }
}