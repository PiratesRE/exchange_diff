using System;
using Microsoft.Exchange.Data.Metering;
using Microsoft.Exchange.Data.Metering.Throttling;
using Microsoft.Exchange.Transport.LoggingCommon;

namespace Microsoft.Exchange.Transport
{
	internal class QueueQuotaLoggingContext
	{
		public QueueQuotaLoggingContext(ICountedEntity<MeteredEntity> entity, QueueQuotaResources resource, IQueueQuotaThresholdFetcher thresholdFetcher)
		{
			this.Resource = QueueQuotaHelper.GetThrottlingResource(resource);
			if (entity.Name.Type == MeteredEntity.Tenant)
			{
				this.Scope = ThrottlingScope.Tenant;
				this.ScopeValue = entity.Name.Value;
				Guid guid;
				if (Guid.TryParse(entity.Name.Value, out guid))
				{
					this.OrgId = guid;
				}
				this.HighThreshold = thresholdFetcher.GetOrganizationQuotaHighMark(guid, resource);
				this.WarningThreshold = thresholdFetcher.GetOrganizationWarningMark(guid, resource);
				return;
			}
			if (entity.Name.Type == MeteredEntity.Sender)
			{
				this.Scope = ThrottlingScope.Sender;
				this.Sender = QueueQuotaHelper.GetRedactedSender(QueueQuotaEntity.Sender, entity.Name.Value);
				this.ScopeValue = this.Sender;
				Guid guid2;
				if (Guid.TryParse(entity.GroupName.Value, out guid2))
				{
					this.OrgId = guid2;
				}
				this.HighThreshold = thresholdFetcher.GetSenderQuotaHighMark(guid2, entity.Name.Value, resource);
				this.WarningThreshold = thresholdFetcher.GetSenderWarningMark(guid2, entity.Name.Value, resource);
				return;
			}
			if (entity.Name.Type == MeteredEntity.AccountForest)
			{
				this.Scope = ThrottlingScope.AccountForest;
				this.OrgId = Guid.Empty;
				this.ScopeValue = entity.Name.Value;
				this.HighThreshold = thresholdFetcher.GetAccountForestQuotaHighMark(resource);
				this.WarningThreshold = thresholdFetcher.GetAccountForestWarningMark(resource);
			}
		}

		internal Guid OrgId { get; private set; }

		internal string Sender { get; private set; }

		internal int HighThreshold { get; private set; }

		internal int WarningThreshold { get; private set; }

		internal ThrottlingScope Scope { get; private set; }

		internal string ScopeValue { get; private set; }

		internal ThrottlingResource Resource { get; private set; }
	}
}
