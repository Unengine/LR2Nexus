using System.Collections.ObjectModel;

namespace LR2Nexus.Utils
{
	public static class ObservableExtension
	{
		public static void Sync<T>(this ObservableCollection<T> collection, IEnumerable<T> newData)
		{
			var newList = newData.ToList();

			var toRemove = collection.Except(newList).ToList();
			foreach (var item in toRemove) collection.Remove(item);

			foreach (var item in newList)
			{
				if (!collection.Contains(item)) collection.Add(item);
			}
		}
	}
}
