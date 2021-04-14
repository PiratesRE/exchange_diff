using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IFolderTreeData : IMessageItem, IToDoItem, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		byte[] NodeOrdinal { get; }

		int OutlookTagId { get; }

		FolderTreeDataType FolderTreeDataType { get; }

		FolderTreeDataFlags FolderTreeDataFlags { get; }

		void SetNodeOrdinal(byte[] nodeBefore, byte[] nodeAfter);
	}
}
