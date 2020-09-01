using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FeedTheBaby.UI
{
    [ExecuteInEditMode]
    public class RadialLayoutUI : MonoBehaviour
    {
        [SerializeField] float distanceFromCentre = 10f;
        [SerializeField] int childSpacingInDegrees = 2;
        
        protected virtual void Awake()
        {
            if (transform.childCount > 0)
            {
                childSpacingInDegrees = Mathf.Min(childSpacingInDegrees,
                    360 / transform.childCount);
            }
        }

        void Update()
        {
            if (transform.childCount > 0)
            {
                PositionChildren();
            }
        }

        public void PositionChildren()
        {
            childSpacingInDegrees = Mathf.Min(childSpacingInDegrees,
                360 / transform.childCount);
            for (int i = 0; i < transform.childCount; i++)
            {
                PositionChild(i);
            }
        }

        void PositionChild(int childIndex)
        {
            RectTransform child = transform.GetChild(childIndex).GetComponent<RectTransform>();
            child.anchorMax = new Vector2(0.5f, 0.5f);
            child.anchorMin = new Vector2(0.5f, 0.5f);
            
            float x = Mathf.Sin(childIndex * childSpacingInDegrees * Mathf.Deg2Rad);
            float y = Mathf.Cos(childIndex * childSpacingInDegrees * Mathf.Deg2Rad);
            child.anchoredPosition = new Vector2(x, y) * distanceFromCentre;
        }

        public void Clear()
        {
            var children = transform.Cast<Transform>().ToList();
            foreach (var child in children)
            {
                Destroy(child.gameObject);
            }
        }
    }
}