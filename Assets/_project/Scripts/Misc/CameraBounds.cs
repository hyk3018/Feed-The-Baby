using UnityEngine;
using UnityEngine.Tilemaps;

namespace FeedTheBaby
{
    public class CameraBounds : MonoBehaviour
    {
        [SerializeField] float returnSpeed = 5f;
        [SerializeField] float movementSlack = 5;
        
        public Camera mainCamera;
        public Bounds bounds;
        public Bounds moveBounds;
        public Bounds mouseWorldMoveBounds;
        
        public bool canReturnToBounds;

        void Awake()
        {
            mainCamera = GetComponent<Camera>();
            canReturnToBounds = true;
        }

        void Start()
        {
            CalculateBoundsRelativeToPosition();
        }

        void CalculateBoundsRelativeToPosition()
        {
            Tilemap boundMap = LevelManager.Instance.terrainTileMap;
            bounds.min = boundMap.transform.TransformPoint(boundMap.localBounds.min);
            bounds.max = boundMap.transform.TransformPoint(boundMap.localBounds.max);
            bounds.Expand(5);

            Vector3 cameraPosition = transform.position;
            mouseWorldMoveBounds.min = bounds.min - new Vector3(movementSlack, movementSlack, -cameraPosition.z);
            mouseWorldMoveBounds.max = bounds.max + new Vector3(movementSlack, movementSlack, -cameraPosition.z);
            
            float cameraHalfHeight = mainCamera.orthographicSize;
            bounds.min += new Vector3(cameraHalfHeight * mainCamera.aspect, cameraHalfHeight, cameraPosition.z);
            bounds.max -= new Vector3(cameraHalfHeight * mainCamera.aspect, cameraHalfHeight, cameraPosition.z);

            moveBounds.min = bounds.min - new Vector3(movementSlack,movementSlack);
            moveBounds.max = bounds.max + new Vector3(movementSlack, movementSlack);
        }

        void Update()
        {
            ReturnToBounds();
        }

        public void MoveToBounds(Vector3 position)
        {
            float x = Mathf.Clamp(position.x, moveBounds.min.x, moveBounds.max.x);
            float y = Mathf.Clamp(position.y, moveBounds.min.y, moveBounds.max.y);
            transform.position = new Vector3(x, y, transform.position.z);
        }

        public void MoveWithinBounds(Vector3 movement)
        {
            if (moveBounds.Contains(transform.position + new Vector3(movement.x, 0)))
            {
                Vector3 cameraPosition = transform.position;
                float clampedX = Mathf.Clamp(cameraPosition.x + movement.x,
                    moveBounds.min.x,
                    moveBounds.max.x);
                transform.position = new Vector3(clampedX, cameraPosition.y,
                    cameraPosition.z);
            }

            if (moveBounds.Contains(transform.position + new Vector3(0, movement.y)))
            {
                Vector3 cameraPosition = transform.position;
                float clampedY = Mathf.Clamp(cameraPosition.y + movement.y,
                    moveBounds.min.y,
                    moveBounds.max.y);
                transform.position = new Vector3(cameraPosition.x, clampedY, cameraPosition.z);
            }
        }

        void ReturnToBounds()
        {
            if (bounds.Contains(transform.position))
                return;

            if (canReturnToBounds)
            {
                Vector3 movement = (bounds.center - transform.position).normalized * (returnSpeed * Time.deltaTime);
                movement.z = 0;
                transform.position += movement;
            }
        }
    }
}