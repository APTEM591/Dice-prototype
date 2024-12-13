using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DiceRoll.Map
{
    public class MapScope : LifetimeScope
    {
        [SerializeField] private SpriteRenderer floor;
        
        protected override void Configure(IContainerBuilder builder)
        {
            var mapInfo = new MapInfo
            {
                CellSize = floor.bounds.size.x / floor.size.x,
                MapSize = new Vector2Int((int)floor.size.x, (int)floor.size.y)
            };

            builder.RegisterInstance(mapInfo);
        }
    }
}