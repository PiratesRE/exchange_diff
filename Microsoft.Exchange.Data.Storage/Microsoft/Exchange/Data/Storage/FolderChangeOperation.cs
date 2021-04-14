using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum FolderChangeOperation
	{
		Copy,
		Move,
		MoveToDeletedItems,
		SoftDelete,
		HardDelete,
		DoneWithMessageDelete,
		Create,
		Update,
		Empty
	}
}
