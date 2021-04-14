using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal sealed class DueDate : TaskDate
	{
		internal DueDate() : base("DueDate", InternalSchema.UtcDueDate, InternalSchema.LocalDueDate)
		{
		}
	}
}
