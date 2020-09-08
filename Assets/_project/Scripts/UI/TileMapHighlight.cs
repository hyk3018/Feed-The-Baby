using UnityEngine;
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
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _tileMap.ClearAllTiles();
                _tileMap.SetTile(_tileMap.WorldToCell(mousePos), highlightTile);
            }
        }
    }
}
