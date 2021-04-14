using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAggregatedUserConfigurationSchema
	{
		IEnumerable<AggregatedUserConfigurationDescriptor> All { get; }
	}
}
