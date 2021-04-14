using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Hygiene.Data.Optics
{
	internal interface IOpticsSession
	{
		IEnumerable<ReputationQueryResult> Query(IEnumerable<ReputationQueryInput> queryInputs);
	}
}
