using System.Linq;
using UnityEngine;

namespace PracticePlugin
{
	public static class PoolExtensions
	{
		public static void DespawnAll<T> (this MemoryPoolWithActiveItems<T> memoryPool) where T : Component
		{
			var activeItems = memoryPool.activeItems.ToList();
			foreach (var activeItem in activeItems)
			{
				memoryPool.Despawn(activeItem);
			}
		}
	}
}