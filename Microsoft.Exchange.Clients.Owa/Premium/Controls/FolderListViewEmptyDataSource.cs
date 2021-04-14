using System;
using System.Collections;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	internal class FolderListViewEmptyDataSource : ExchangeListViewDataSource, IListViewDataSource
	{
		public FolderListViewEmptyDataSource(Folder folder, Hashtable properties) : base(properties)
		{
			this.folder = folder;
		}

		public Folder Folder
		{
			get
			{
				return this.folder;
			}
		}

		public string ContainerId
		{
			get
			{
				return Utilities.GetIdAsString(this.folder);
			}
		}

		public new int StartRange
		{
			get
			{
				return 0;
			}
		}

		public new int EndRange
		{
			get
			{
				return 0;
			}
		}

		public new int RangeCount
		{
			get
			{
				return 0;
			}
		}

		public override int TotalCount
		{
			get
			{
				return 0;
			}
		}

		public override int TotalItemCount
		{
			get
			{
				return 0;
			}
		}

		public int UnreadCount
		{
			get
			{
				return 0;
			}
		}

		public bool UserHasRightToLoad
		{
			get
			{
				return true;
			}
		}

		public void Load(string seekValue, int itemCount)
		{
		}

		public void Load(int startRange, int itemCount)
		{
		}

		public void Load(ObjectId seekToObjectId, SeekDirection seekDirection, int itemCount)
		{
		}

		public bool LoadAdjacent(ObjectId adjacentObjectId, SeekDirection seekDirection, int itemCount)
		{
			throw new NotImplementedException("This API is not needed while using the FolderListViewEmptyDataSource");
		}

		public new bool MoveNext()
		{
			throw new NotImplementedException("This API is not needed while using the FolderListViewEmptyDataSource");
		}

		public new void MoveToItem(int itemIndex)
		{
			throw new NotImplementedException("This API is not needed while using the FolderListViewEmptyDataSource");
		}

		public new int CurrentItem
		{
			get
			{
				return 0;
			}
		}

		public string GetItemId()
		{
			throw new NotImplementedException("This API is not needed while using the FolderListViewEmptyDataSource");
		}

		public string GetItemClass()
		{
			throw new NotImplementedException("This API is not needed while using the FolderListViewEmptyDataSource");
		}

		public override T GetItemProperty<T>(PropertyDefinition propertyDefinition, T defaultValue)
		{
			throw new NotImplementedException("This API is not needed while using the FolderListViewEmptyDataSource");
		}

		public new object GetCurrentItem()
		{
			throw new NotImplementedException("This API is not needed while using the FolderListViewEmptyDataSource");
		}

		private Folder folder;
	}
}
