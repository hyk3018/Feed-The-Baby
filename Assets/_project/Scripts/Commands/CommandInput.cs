using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FeedTheBaby.Commands
{
    [RequireComponent(typeof(CommandManager))]
    public class CommandInput : MonoBehaviour
    {
        [SerializeField] LayerMask collisionMask = default;
        [SerializeField] float minimumHoldDuration = 1f;
        CommandManager _commandManager;

        public Action<Vector3, Transform, HoldType> OnTileHeld;

        float currentHoldDuration;
        bool holding;

        void Awake()
        {
            _commandManager = GetComponent<CommandManager>();
        }

        void Update()
        {
            if (Input.GetMouseButton(1))
            {
                currentHoldDuration += Time.deltaTime;
            }

            if (Input.GetMouseButtonUp(1))
            {
                RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition),
                    Single.PositiveInfinity, collisionMask);
                if (hit.transform == null)
                {
                    Vector3 groundPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    if (currentHoldDuration < minimumHoldDuration)
                    {
                        _commandManager.AddCommand(new MovePositionCommand(groundPosition));
                    }
                    else
                    {
                        currentHoldDuration = 0;
                        OnTileHeld(groundPosition, null, HoldType.GROUND);
                    }
                }
                else if (hit.transform.gameObject.layer != LayerMask.NameToLayer("UI"))
                {
                    if (currentHoldDuration < minimumHoldDuration)
                    {
                        _commandManager.AddCommand(new MoveAndInteractCommand(hit.transform));
                    }
                    else
                    {
                        currentHoldDuration = 0;
                        OnTileHeld(default, hit.transform, HoldType.LEVEL_OBJECT);
                    }
                }
            }
        }
    }

    public enum HoldType
    {
        GROUND, LEVEL_OBJECT
    }
}