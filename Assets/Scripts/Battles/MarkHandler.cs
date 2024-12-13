using System;
using System.Collections.Generic;
using DiceRoll.Map;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace DiceRoll.Battles
{
    public class MarkHandler: IDisposable
    {
        private readonly Transform _markPrefab;
        private readonly MapInfo _mapInfo;
        
        List<Transform> _marksStacks = new();
        
        [Inject]
        private MarkHandler(Transform markPrefab, MapInfo mapInfo)
        {
            _markPrefab = markPrefab;
            _mapInfo = mapInfo;
        }
        
        public void AddMark(Vector2Int position)
        {
            var mark = _marksStacks.Find(t => !t.gameObject.activeSelf);
            
            if (mark == null)
            {
                mark = Object.Instantiate(_markPrefab);
                _marksStacks.Add(mark);
            }
            
            mark.position = _mapInfo.MapToWorld(position);
            mark.gameObject.SetActive(true);
        }
        
        public void ResetMarks()
        {
            foreach (var mark in _marksStacks) 
                mark.gameObject.SetActive(false);
        }

        public void Dispose()
        {
            foreach (var mark in _marksStacks) 
                Object.Destroy(mark.gameObject);
        }

        public void AddMarks(Vector2Int[] attackMarks)
        {
            foreach (var mark in attackMarks)
                AddMark(mark);
        }
    }
}