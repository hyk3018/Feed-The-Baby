using System.Collections.Generic;
using FeedTheBaby.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FeedTheBaby.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] ItemUIData itemUIData = null;
        [SerializeField] Inventory inventory = null;
        [SerializeField] Fuel fuel = null;
        [SerializeField] GameObject itemSlotTemplate = null;
        [SerializeField] GameObject fuelSlotTemplate = null;

        [SerializeField] Transform inventoryUIContainer = null;
        [SerializeField] Transform fuelUIContainer = null;

        readonly List<GameObject> _itemSlots = new List<GameObject>();
        GameObject _fuelSlot;

        void Awake()
        {
            InitialiseInventorySlots();

            InitialiseFuelSlot();

            // Subscribe to InventoryChange, which gets called at Start() to fill
            // initial inventory values
            inventory.InventoryChange += UpdateInventoryUI;
            inventory.FuelChange += UpdateFuelUI;
        }

        void InitialiseInventorySlots()
        {
            // Set up empty item slots for all slots
            for (var i = 0; i < 6; i++)
            {
                var itemSlot = Instantiate(itemSlotTemplate, inventoryUIContainer);

                InitialiseEmptySlot(itemSlot);

                _itemSlots.Add(itemSlot);
            }
        }

        void InitialiseFuelSlot()
        {
            _fuelSlot = Instantiate(fuelSlotTemplate, fuelUIContainer);
            _fuelSlot.transform.localPosition = new Vector3(-26, 0, 0);
            InitialiseEmptySlot(_fuelSlot);

            FuelSlotUI fuelSlotUI = _fuelSlot.GetComponent<FuelSlotUI>();
            fuelSlotUI.fuel = fuel;
        }

        static void InitialiseEmptySlot(GameObject itemSlot)
        {
            var images = itemSlot.GetComponentsInChildren<Image>();
            images[1].enabled = false;

            var text = itemSlot.GetComponentInChildren<TextMeshProUGUI>();
            text.text = "";
        }

        void UpdateInventoryUI(List<ItemAmount> items)
        {
            for (var i = 0; i < _itemSlots.Count; i++)
            {
                GameObject itemSlot = _itemSlots[i];
                
                if (i < items.Count)
                {
                    var item = items[i];
                    FillItemSlot(itemSlot, item);
                }
                else
                {
                    EmptyItemSlot(itemSlot);
                }
            }
        }

        void UpdateFuelUI(ItemAmount fuel)
        {
            if (fuel.amount > 0)
                FillItemSlot(_fuelSlot, fuel);
            else
                EmptyItemSlot(_fuelSlot);
        }

        static void EmptyItemSlot(GameObject itemSlot)
        {
            TextMeshProUGUI text = itemSlot.GetComponentInChildren<TextMeshProUGUI>();
            Image itemImage = itemSlot.GetComponentsInChildren<Image>()[1];
            itemImage.enabled = false;
            itemImage.sprite = null;
            text.text = "";
        }

        void FillItemSlot(GameObject itemSlot, ItemAmount item)
        {
            TextMeshProUGUI text = itemSlot.GetComponentInChildren<TextMeshProUGUI>();
            Image itemImage = itemSlot.GetComponentsInChildren<Image>()[1];
            itemImage.enabled = true;
            itemImage.sprite = itemUIData.itemSpriteDictionary.dictionary[item.type];
            text.text = item.amount.ToString();
        }
    }
}