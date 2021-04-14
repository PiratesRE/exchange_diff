using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum ChangeType
	{
		None,
		Add,
		Change,
		Delete = 4,
		ReadFlagChange,
		SoftDelete,
		OutOfFilter,
		AssociatedAdd,
		Send
	}
}
