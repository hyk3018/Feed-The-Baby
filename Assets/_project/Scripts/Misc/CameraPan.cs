using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FeedTheBaby
{
    public class CameraPan : MonoBehaviour
    {
        [SerializeField] int panEdgeWidthPixels = 100;
        [SerializeField] float panSpeed = 5f;
        [SerializeField] CameraBounds cameraBounds = null;

        Bounds _nonPanPixelBounds;

        void Awake()
        {
            _nonPanPixelBounds.min = new Vector3(panEdgeWidthPixels, panEdgeWidthPixels);
            _nonPanPixelBounds.max = new Vector3(cameraBounds.mainCamera.pixelWidth - panEdgeWidthPixels,
                cameraBounds.mainCamera.pixelHeight - panEdgeWidthPixels);
        }

        void Update()
        {
            if (LevelManager.Instance.playing)
            {
                if (!_nonPanPixelBounds.Contains(Input.mousePosition))
                {
                    cameraBounds.canReturnToBounds = false;
                    Vector3 velocity = Input.mousePosition - _nonPanPixelBounds.center;
                    Vector3 movement = velocity.normalized * (panSpeed * Time.deltaTime);
                    movement.z = 0;

                    cameraBounds.MoveWithinBounds(movement);
                }
                else
                {
                    cameraBounds.canReturnToBounds = true;
                }
            }
        }
    }
}