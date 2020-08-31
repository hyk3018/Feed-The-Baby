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

        public Action<Vector3, Transform, HoldType> OnCommandPanelOpen;
        public Action OnCommandPanelClose;

        float _currentHoldDuration;
        bool _panelOpen;
        bool _buttonUpDuringPanelOpen;

        void Awake()
        {
            _commandManager = GetComponent<CommandManager>();
        }

        void Update()
        {
            if (Input.GetMouseButton(1))
            {
                _currentHoldDuration += Time.deltaTime;
                if (_currentHoldDuration > minimumHoldDuration)
                {
                    _panelOpen = true;
                    RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition),
                        Single.PositiveInfinity, collisionMask);
                    if (hit.transform == null)
                    {
                        Vector3 groundPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        OnCommandPanelOpen?.Invoke(groundPosition, null, HoldType.GROUND);
                    }
                    else if (hit.transform.gameObject.layer != LayerMask.NameToLayer("UI"))
                    {
                        OnCommandPanelOpen(default, hit.transform, HoldType.LEVEL_OBJECT);
                    }
                    _currentHoldDuration = 0;
                }
            }

            if (Input.GetMouseButtonUp(1))
            {
                if (_panelOpen && _buttonUpDuringPanelOpen)
                {
                    _buttonUpDuringPanelOpen = false;
                    OnCommandPanelClose?.Invoke();
                    ClosePanel();
                }
                else
                {
                    _buttonUpDuringPanelOpen = true;
                    _currentHoldDuration = 0;
                    RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition),
                        Single.PositiveInfinity, collisionMask);
                    if (hit.transform == null)
                    {
                        Vector3 groundPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        _commandManager.AddCommand(new MovePositionCommand(groundPosition));
                    }
                    else if (hit.transform.gameObject.layer != LayerMask.NameToLayer("UI"))
                    {
                        _commandManager.AddCommand(new MoveAndInteractCommand(hit.transform));
                    }
                }
            }
        }

        public void ClosePanel()
        {
            _panelOpen = false;
        }
    }

    public enum HoldType
    {
        GROUND, LEVEL_OBJECT
    }
}