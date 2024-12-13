using System.Collections.Generic;
using UnityEngine;

namespace DiceRoll.Map
{
    public class MapInfo
    {
        public float CellSize { get; internal set; }
        
        public int Width => MapSize.x;
        public int Height => MapSize.y;
        public Vector2Int MapSize { get; internal set; }
        
        public List<GameObject> Entities => new (_entities);
        
        private List<GameObject> _entities = new ();

        public Vector3 MapToWorld(Vector2Int mapPos)
        {
            //Debug.Log(CellSize);
            float x = mapPos.x * CellSize - Width * 0.5f + CellSize * 0.5f;
            float z = mapPos.y * CellSize - Height * 0.5f + CellSize * 0.5f;
            return new Vector3(x, 0, z);
        }

        public Vector2Int WorldToMap(Vector3 worldPos)
        {
            int x = Mathf.RoundToInt((worldPos.x + Width * 0.5f) / CellSize);
            int y = Mathf.RoundToInt((worldPos.z + Height * 0.5f) / CellSize);
            return new Vector2Int(x, y);
        }
        
        public void AddEntity(GameObject entity)
        {
            if(!_entities.Contains(entity))
                _entities.Add(entity);
        }
        
        public void RemoveEntity(GameObject entity)
        {
            _entities.Remove(entity);
        }
    }
}