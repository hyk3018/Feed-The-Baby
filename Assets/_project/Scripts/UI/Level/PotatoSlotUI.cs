using FeedTheBaby.Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FeedTheBaby.UI
{
    public class PotatoSlotUI : ItemSlotUI, IPointerClickHandler
    {
        public Fuel fuel = null;

        public void OnPointerClick(PointerEventData eventData)
        {
            fuel.ConsumeFuel(1);
        }
    }
}