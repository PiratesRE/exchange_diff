using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ResourceHealth
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IDatabaseInformation
	{
		Guid DatabaseGuid { get; }

		string DatabaseName { get; }

		string DatabaseVolumeName { get; }
	}
}
