using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum ItemChangeOperation
	{
		Create,
		Update,
		ItemBind,
		Submit,
		Move,
		Copy,
		Delete
	}
}
