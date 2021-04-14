using System;

namespace Microsoft.Exchange.Transport
{
	internal interface IQueueQuotaThresholdFetcher
	{
		int GetOrganizationQuotaHighMark(Guid organizationId, QueueQuotaResources resource);

		int GetOrganizationWarningMark(Guid organizationId, QueueQuotaResources resource);

		int GetSenderQuotaHighMark(Guid organizationId, string sender, QueueQuotaResources resource);

		int GetSenderWarningMark(Guid organizationId, string sender, QueueQuotaResources resource);

		int GetAccountForestQuotaHighMark(QueueQuotaResources resource);

		int GetAccountForestWarningMark(QueueQuotaResources resource);

		int GetAvailableResourceSize(QueueQuotaResources resource);
	}
}
