using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FeedTheBaby.Commands
{
    public class RaycastObjectIdentifier
    {
        public static RaycastObjectData IdentifyRaycastObject(RaycastHit2D hit, LayerMask ignoreMask)
        {
            if (hit.transform == null)
            {
                // Did not hit any colliders so we're clicking at tile map
                Vector3 groundPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                Tilemap obstructionsTileMap = LevelManager.Instance.obstructionsTileMap;
                Vector3Int worldToCell = obstructionsTileMap.WorldToCell(groundPosition);
                TileBase obstructionTile = obstructionsTileMap.GetTile(worldToCell);
                if (obstructionTile)
                    return new RaycastObjectData(RaycastObjectType.NONE, null, null);
                
                Tilemap terrainTileMap = LevelManager.Instance.terrainTileMap;
                TileBase terrainTile = terrainTileMap.GetTile(worldToCell);
                if (terrainTile)
                    return new RaycastObjectData(RaycastObjectType.TERRAIN, terrainTile);

            }
            else
            {
                GameObject colliderObject = hit.transform.gameObject;
                if (ignoreMask != (ignoreMask | 1 << colliderObject.layer))
                {
                    return new RaycastObjectData(RaycastObjectType.COLLIDER_OBJECT, colliderObject);
                }
                else
                {
                    return new RaycastObjectData(RaycastObjectType.NONE, null, null);
                }
            }
            
            return new RaycastObjectData(RaycastObjectType.NONE, null, null);
        }
    }

    public struct RaycastObjectData
    {
        public TileBase tile;
        public GameObject colliderObject;
        public RaycastObjectType type;

        public RaycastObjectData(RaycastObjectType type, TileBase tile)
        {
            this.tile = tile;
            this.type = type;
            colliderObject = null;
        }

        public RaycastObjectData(RaycastObjectType type, GameObject colliderObject)
        {
            this.colliderObject = colliderObject;
            this.type = type;
            tile = null;
        }

        public RaycastObjectData(RaycastObjectType type, TileBase tile, GameObject colliderObject)
        {
            this.type = type;
            this.tile = tile;
            this.colliderObject = colliderObject;
        }
        
    }

    public enum RaycastObjectType
    {
        COLLIDER_OBJECT, TERRAIN, NONE
    }
}