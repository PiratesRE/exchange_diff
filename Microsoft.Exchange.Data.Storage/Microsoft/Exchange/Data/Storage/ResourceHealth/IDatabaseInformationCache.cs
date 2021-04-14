using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ResourceHealth
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IDatabaseInformationCache
	{
		IDatabaseInformation Get(Guid key);
	}
}
