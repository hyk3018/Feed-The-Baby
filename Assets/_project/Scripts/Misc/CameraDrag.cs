using System;
using UnityEngine;

namespace FeedTheBaby
{
    public class CameraDrag : MonoBehaviour
    {
        [SerializeField] CameraBounds cameraBounds = null;
        
        Vector3 _dragRoot;
        Vector3 _cameraRoot;

        void Update()
        {
            if (LevelManager.Instance.playing)
            {
                if (Input.GetMouseButtonDown(2))
                {
                    cameraBounds.canReturnToBounds = false;
                    _dragRoot = cameraBounds.mainCamera.ScreenToViewportPoint(Input.mousePosition);
                    _cameraRoot = cameraBounds.mainCamera.transform.position;
                }

                if (Input.GetMouseButton(2))
                {
                    Vector3 currentMouse = cameraBounds.mainCamera.ScreenToViewportPoint(Input.mousePosition);
                    cameraBounds.MoveToMoveBounds(_cameraRoot + (_dragRoot - currentMouse) * 10);
                }

                if (Input.GetMouseButtonUp(2))
                    cameraBounds.canReturnToBounds = true;
            }
        }
    }
}