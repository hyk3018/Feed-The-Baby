using System.Collections;
using System.Collections.Generic;
using FeedTheBaby.GameData;
using FeedTheBaby.LevelEditor;
using TMPro;
using UnityEngine;

namespace FeedTheBaby.UI
{
    public class FocusedLevelUI : DisableablePanel
    {
        [SerializeField] TextMeshProUGUI title = null;
        [SerializeField] GameObject itemSlotPrefab = null;
        [SerializeField] Transform[] itemSlotGrids = null;
        [SerializeField] Transform inventoryGrid = null;

        int _levelToLoad;
        LevelData _levelData;
        static readonly int Show = Animator.StringToHash("Show");

        // Instantiate empty item slots
        protected override void Awake()
        {
            base.Awake();

            foreach (var grid in itemSlotGrids)
                InitialiseGrid(grid);

            InitialiseGrid(inventoryGrid);
        }

        public IEnumerator FetchLevelData()
        {
            _levelToLoad = DataService.Instance.GetCurrentLevel();
            _levelData = DataService.Instance.GetLevels()[_levelToLoad];
            title.text = "Level : " + (_levelToLoad + 1);

            FillGoals();
            FillInventory();
            yield break;
        }

        void FillGoals()
        {
            for (var i = 0; i < _levelData.goals.Length; i++)
            {
                ClearGrid(itemSlotGrids[i]);
                FillGrid(_levelData.goals[i].items, itemSlotGrids[i], 0);
            }
        }

        void FillInventory()
        {
            ClearGrid(inventoryGrid);

            var potatoUI = inventoryGrid.GetChild(0).GetComponent<ItemSlotUI>();
            potatoUI.SetItemAmount(new ItemAmount(ItemType.Fuel, _levelData.fuelAmount));

            FillGrid(_levelData.initialInventory, inventoryGrid, 1);
        }

        void FillGrid(IReadOnlyList<ItemAmount> itemAmounts, Transform grid, int gridOffset)
        {
            for (var i = 0; i < itemAmounts.Count; i++)
            {
                var itemAmount = itemAmounts[i];
                grid.GetChild(i + gridOffset).GetComponent<ItemSlotUI>().SetItemAmount(itemAmount);
            }
        }

        static void ClearGrid(Transform transform)
        {
            foreach (Transform child in transform)
            {
                var itemSlotUI = child.GetComponent<ItemSlotUI>();
                itemSlotUI.SetEmpty();
            }
        }

        void InitialiseGrid(Transform grid)
        {
            for (var j = 0; j < 6; j++)
            {
                var itemSlotGameObject = Instantiate(itemSlotPrefab, grid);
                var itemSlotUI = itemSlotGameObject.GetComponent<ItemSlotUI>();
                itemSlotUI.SetEmpty();
            }
        }

        public void ShowUI()
        {
            GetComponent<Animator>().SetTrigger(Show);
        }
    }
}