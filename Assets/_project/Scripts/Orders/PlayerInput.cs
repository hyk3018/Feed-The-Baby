using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FeedTheBaby
{
    [RequireComponent(typeof(Camera))]
    public class PlayerInput : MonoBehaviour
    {
        // [SerializeField] GameManager _gameManager = null;

        Camera _camera;

        // Start is called before the first frame update
        void Start()
        {
            _camera = GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            // If right click somewhere, check for collision
            if (Input.GetMouseButtonDown(1))
            {
                var mousePosition = Input.mousePosition;
                var ray = _camera.ScreenPointToRay(mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    // the object identified by hit.transform was clicked
                    // do whatever you want
                }
                else
                {
                    var positionOnWorld = _camera.ScreenToWorldPoint(new Vector3(
                        mousePosition.x, mousePosition.y, _camera.nearClipPlane));
                }
            }
        }
    }
}