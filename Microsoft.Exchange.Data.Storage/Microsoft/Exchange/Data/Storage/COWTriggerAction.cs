using System;

namespace Microsoft.Exchange.Data.Storage
{
	public enum COWTriggerAction
	{
		Create,
		Update,
		ItemBind,
		Submit,
		Copy,
		Move,
		MoveToDeletedItems,
		SoftDelete,
		HardDelete,
		DoneWithMessageDelete,
		FolderBind
	}
}
