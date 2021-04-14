using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	public interface IListViewDataSource
	{
		string ContainerId { get; }

		int StartRange { get; }

		int EndRange { get; }

		int RangeCount { get; }

		int TotalCount { get; }

		int TotalItemCount { get; }

		int UnreadCount { get; }

		bool UserHasRightToLoad { get; }

		void Load(string seekValue, int itemCount);

		void Load(int startRange, int itemCount);

		void Load(ObjectId seekToObjectId, SeekDirection seekDirection, int itemCount);

		bool LoadAdjacent(ObjectId adjacentObjectId, SeekDirection seekDirection, int itemCount);

		bool MoveNext();

		void MoveToItem(int itemIndex);

		int CurrentItem { get; }

		string GetItemId();

		string GetItemClass();

		T GetItemProperty<T>(PropertyDefinition propertyDefinition) where T : class;

		T GetItemProperty<T>(PropertyDefinition propertyDefinition, T defaultValue);

		object GetCurrentItem();
	}
}
