using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FeedTheBaby.UI
{
    [ExecuteInEditMode]
    public class RadialLayoutUI : MonoBehaviour
    {
        public List<RectTransform> children = null;
        [SerializeField] float distanceFromCentre = 10f;
        [SerializeField] int childSpacingInDegrees = 2;

        RectTransform _rectTransform;

        void Awake()
        {
            Debug.Log("AWake");
            _rectTransform = GetComponent<RectTransform>();
            
            if (children != null && children.Count > 0)
            {
                PositionChildren();
            }
            else
            {
                Debug.Log("Try");
                children = new List<RectTransform>();

                foreach (RectTransform childTransform in _rectTransform)
                {
                    children.Add(childTransform);
                }

                childSpacingInDegrees = Mathf.Min(childSpacingInDegrees,
                    360 / children.Count);
            }
        }

        void PositionChildren()
        {
            for (int i = 0; i < children.Count; i++)
            {
                PositionChild(i);
            }
        }

        void Update()
        {
            childSpacingInDegrees = Mathf.Min(childSpacingInDegrees,
                360 / children.Count);
            PositionChildren();
        }

        void PositionChild(int childIndex)
        {
            RectTransform child = children[childIndex];
            child.anchorMax = new Vector2(0.5f, 0.5f);
            child.anchorMin = new Vector2(0.5f, 0.5f);
            
            float x = Mathf.Sin(childIndex * childSpacingInDegrees * Mathf.Deg2Rad);
            float y = Mathf.Cos(childIndex * childSpacingInDegrees * Mathf.Deg2Rad);
            child.anchoredPosition = new Vector2(x, y) * distanceFromCentre;
        }
    }
}