using System;
using System.Collections.Generic;
using FeedTheBaby.Game;
using FeedTheBaby.LevelObjects;
using FeedTheBaby.Tilemaps.Tiles;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FeedTheBaby.Commands
{
    [RequireComponent(typeof(CommandManager))]
    public class CommandInput : MonoBehaviour
    {
        [SerializeField] LayerMask collisionMask = default;
        [SerializeField] LayerMask ignoreMask = default;
        [SerializeField] float minimumHoldDuration = 1f;
        CommandManager _commandManager;

        public Action<Vector3, Transform, CommandType> OnCommandPanelOpen;
        public Action OnCommandPanelClose;
        public bool panelOpen;

        float _currentHoldDuration;
        bool _closeOnNextButtonUp;
        bool _currentlyHolding;

        void Awake()
        {
            _commandManager = GetComponent<CommandManager>();
        }

        void Update()
        {
            if (!LevelManager.Instance.playing)
                return;

            // When we click down and the panel is open, the next button up has
            // potential to close the current open panel
            if (Input.GetMouseButtonDown(1))
            {
                if (panelOpen)
                {
                    _closeOnNextButtonUp = true;
                }
            }
            
            // If we reach time to open a new panel, then the next button up
            // should not close the panel
            if (Input.GetMouseButton(1))
            {
                if (EventSystem.current.IsPointerOverGameObject() &&
                    EventSystem.current.currentSelectedGameObject != null) 
                    return;

                _currentHoldDuration += Time.deltaTime;
                
                if (_currentHoldDuration >= minimumHoldDuration && !_currentlyHolding)
                {
                    if (panelOpen)
                    {
                        OnCommandPanelClose?.Invoke();
                    }

                    _currentlyHolding = true;
                    _closeOnNextButtonUp = false;
                    
                    RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition),
                        Single.PositiveInfinity, collisionMask);
                    
                    // Find out what we are using commands on
                    RaycastObjectData raycastObjectData =
                        RaycastObjectIdentifier.IdentifyRaycastObject(hit, ignoreMask);

                    if (raycastObjectData.type == RaycastObjectType.NONE)
                        return;

                    // Determine what commands are possible for the object / tile
                    CommandType possibleCommands = GetPossibleCommands(raycastObjectData);
                    
                    Vector3 groundPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    if (raycastObjectData.type == RaycastObjectType.TERRAIN)
                    {
                        OnCommandPanelOpen?.Invoke(groundPosition, null, possibleCommands);
                    }
                    else if (raycastObjectData.type == RaycastObjectType.COLLIDER_OBJECT)
                    {
                        OnCommandPanelOpen?.Invoke(groundPosition, hit.transform, possibleCommands);
                    }
                }
            }
            
            if (Input.GetMouseButtonUp(1))
            {
                _currentlyHolding = false;
                _currentHoldDuration = 0;
                if (_closeOnNextButtonUp)
                {
                    _closeOnNextButtonUp = false;
                    OnCommandPanelClose?.Invoke();
                }
                else if (!panelOpen)
                {
                    RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition),
                        Single.PositiveInfinity, collisionMask);
                    if (hit.transform == null)
                    {
                        Vector3 groundPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        _commandManager.AddCommand(new MovePositionCommand(groundPosition));
                    }
                    else if (hit.transform.gameObject.layer != LayerMask.NameToLayer("UI"))
                    {
                        _commandManager.AddCommand(new MoveAndInteractCommand(hit.transform, null));
                    }
                }
            }
        }

        static CommandType GetPossibleCommands(RaycastObjectData raycastObjectData)
        {
            CommandType possibleCommands = CommandType.NONE;

            if (raycastObjectData.type == RaycastObjectType.TERRAIN)
            {
                if (raycastObjectData.tile is TerrainTile terrainTile)
                {
                    possibleCommands |= CommandType.MOVE;
                    possibleCommands |= CommandType.CRAFT;

                    if (terrainTile.terrainType == TerrainType.Grassland)
                    {
                        possibleCommands |= CommandType.PLANT_FUEL;
                    }
                }
            }
            else if (raycastObjectData.type == RaycastObjectType.COLLIDER_OBJECT)
            {
                possibleCommands |= CommandType.MOVE;
                
                IInteractable[] interactables = raycastObjectData.colliderObject.GetComponents<IInteractable>();
                foreach (IInteractable interactable in interactables)
                    possibleCommands |= interactable.PossibleCommands();
            }

            if (possibleCommands != CommandType.NONE)
            {
                possibleCommands ^= CommandType.NONE;
            }
            
            return possibleCommands;
        }
    }
    
    [Flags]
    public enum CommandType
    {
        NONE = 1,
        MOVE = 2,
        FEED = 4,
        PLANT_FUEL = 8,
        HARVEST = 16,
        CRAFT = 32
    }
}