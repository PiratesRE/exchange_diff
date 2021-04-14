using System;
using Microsoft.Exchange.Common.HA;

namespace Microsoft.Exchange.HA.FailureItem
{
	internal class FailureItemPublisherImpl : IFailureItemPublisher
	{
		public void PublishAction(FailureTag failureTag, Guid databaseGuid, string dbInstanceName, IoErrorInfo ioErrorInfo)
		{
			FailureItemPublisherHelper.PublishAction(failureTag, databaseGuid, dbInstanceName, ioErrorInfo);
		}

		public void PublishAction(FailureTag failureTag, Guid databaseGuid, string dbInstanceName)
		{
			FailureItemPublisherHelper.PublishAction(failureTag, databaseGuid, dbInstanceName);
		}
	}
}
