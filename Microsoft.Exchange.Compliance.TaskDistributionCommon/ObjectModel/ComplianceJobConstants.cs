using System;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.ObjectModel
{
	internal static class ComplianceJobConstants
	{
		internal static DateTime MinComplianceTime
		{
			get
			{
				return new DateTime(1900, 1, 1);
			}
		}

		internal const int MaxTopDetailedRecordsToReturn = 500;
	}
}
