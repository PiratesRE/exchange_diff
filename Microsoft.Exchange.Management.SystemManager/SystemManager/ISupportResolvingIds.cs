using System;
using System.Collections;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager
{
	internal interface ISupportResolvingIds
	{
		PropertyDefinition PropertyForResolving { get; set; }

		IList IdentitiesToResolve { get; set; }

		bool IgnoreNonExistingObjects { get; set; }
	}
}
