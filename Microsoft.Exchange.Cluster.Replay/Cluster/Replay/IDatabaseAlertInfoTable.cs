using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal interface IDatabaseAlertInfoTable
	{
		void RaiseAppropriateAlertIfNecessary(IHealthValidationResultMinimal result);

		void ResetState(Guid dbGuid);

		void Cleanup(HashSet<Guid> currentlyKnownDatabaseGuids);
	}
}
