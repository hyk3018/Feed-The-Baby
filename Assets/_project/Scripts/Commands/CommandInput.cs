using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FeedTheBaby.Commands
{
    [RequireComponent(typeof(CommandManager))]
    public class CommandInput : MonoBehaviour
    {
        [SerializeField] LayerMask collisionMask = default;
        CommandManager _commandManager;

        void Awake()
        {
            _commandManager = GetComponent<CommandManager>();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition), Single.PositiveInfinity, collisionMask);
                if (hit.transform == null)
                {
                    Vector3 groundPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    _commandManager.AddCommand(new MovePositionCommand(groundPosition));
                } else
                {
                    IInteractable interactable = hit.transform.GetComponent<IInteractable>();
                    if (interactable != null)
                    {
                        _commandManager.AddCommand(new MoveAndInteractCommand(interactable, hit.transform));
                    }
                }

            }
        }
    }
}