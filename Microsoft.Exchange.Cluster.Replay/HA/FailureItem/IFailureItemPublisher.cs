using System;
using Microsoft.Exchange.Common.HA;

namespace Microsoft.Exchange.HA.FailureItem
{
	internal interface IFailureItemPublisher
	{
		void PublishAction(FailureTag failureTag, Guid databaseGuid, string dbInstanceName, IoErrorInfo ioErrorInfo);

		void PublishAction(FailureTag failureTag, Guid databaseGuid, string dbInstanceName);
	}
}
