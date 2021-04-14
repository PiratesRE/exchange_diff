using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IReminder
	{
		Guid Identifier { get; set; }

		int GetCurrentVersion();
	}
}
