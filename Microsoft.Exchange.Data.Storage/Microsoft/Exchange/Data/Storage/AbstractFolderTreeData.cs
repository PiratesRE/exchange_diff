using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class AbstractFolderTreeData : AbstractMessageItem, IFolderTreeData, IMessageItem, IToDoItem, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		public FolderTreeDataFlags FolderTreeDataFlags
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public FolderTreeDataType FolderTreeDataType
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public byte[] NodeOrdinal
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public int OutlookTagId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void SetNodeOrdinal(byte[] nodeBefore, byte[] nodeAfter)
		{
			throw new NotImplementedException();
		}
	}
}
