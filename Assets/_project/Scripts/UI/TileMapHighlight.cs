using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

namespace FeedTheBaby.UI
{
    public class TileMapHighlight : MonoBehaviour
    {
        [SerializeField] TileBase highlightTile = null;
        
        Tilemap _tileMap;

        void Awake()
        {
            _tileMap = GetComponent<Tilemap>();
        }

        // Update is called once per frame
        void Update()
        {
            if (LevelManager.Instance.playing)
            {
                _tileMap.ClearAllTiles();
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    _tileMap.SetTile(_tileMap.WorldToCell(mousePos), highlightTile);
                }
            }
        }
    }
}
