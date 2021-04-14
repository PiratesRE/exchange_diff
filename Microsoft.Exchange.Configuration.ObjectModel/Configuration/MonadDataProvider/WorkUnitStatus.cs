using System;
using Microsoft.Exchange.Configuration.Common.LocStrings;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	public enum WorkUnitStatus
	{
		[LocDescription(Strings.IDs.WorkUnitStatusNotStarted)]
		NotStarted,
		[LocDescription(Strings.IDs.WorkUnitStatusInProgress)]
		InProgress,
		[LocDescription(Strings.IDs.WorkUnitStatusCompleted)]
		Completed,
		[LocDescription(Strings.IDs.WorkUnitStatusFailed)]
		Failed
	}
}
