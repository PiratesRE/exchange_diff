using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal sealed class StartDate : TaskDate
	{
		internal StartDate() : base("StartDate", InternalSchema.UtcStartDate, InternalSchema.LocalStartDate)
		{
		}
	}
}
