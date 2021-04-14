using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;
using Microsoft.Exchange.PushNotifications.Extensions;
using Microsoft.Exchange.PushNotifications.Publishers;
using Microsoft.Exchange.PushNotifications.Server.Core;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.PushNotifications.Server.Commands
{
	internal class GetAppConfigData : ServiceCommand<AzureAppConfigRequestInfo, AzureAppConfigResponseInfo>
	{
		public GetAppConfigData(AzureAppConfigRequestInfo requestConfig, PushNotificationPublisherManager publisherManager, PushNotificationPublisherConfiguration configuration, AsyncCallback asyncCallback, object asyncState) : base(requestConfig, asyncCallback, asyncState)
		{
			this.PublisherManager = publisherManager;
			this.Configuration = configuration;
		}

		public OrganizationId AuthenticatedTenantId { get; private set; }

		private PushNotificationPublisherManager PublisherManager { get; set; }

		private PushNotificationPublisherConfiguration Configuration { get; set; }

		protected override void InternalInitialize(IBudget budget)
		{
			base.InternalInitialize(budget);
			this.AuthenticatedTenantId = OrganizationId.ForestWideOrgId;
			TenantBudgetKey tenantBudgetKey = budget.Owner as TenantBudgetKey;
			if (tenantBudgetKey != null && tenantBudgetKey.OrganizationId != null)
			{
				this.AuthenticatedTenantId = tenantBudgetKey.OrganizationId;
			}
		}

		protected sealed override AzureAppConfigResponseInfo InternalExecute(TimeSpan queueAndDelay, TimeSpan totalTime)
		{
			List<AzureAppConfigData> list = new List<AzureAppConfigData>();
			string text = null;
			if (this.AuthenticatedTenantId != null && !OrganizationId.ForestWideOrgId.Equals(this.AuthenticatedTenantId))
			{
				text = this.AuthenticatedTenantId.ToExternalDirectoryOrganizationId();
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				string text2 = base.Budget.Owner.ToNullableString(null);
				PushNotificationsCrimsonEvents.CannotResolveOrganizationId.LogPeriodic<string, string>(text2, CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, text2, (base.RequestInstance != null) ? base.RequestInstance.ToFullString() : "null");
				return new AzureAppConfigResponseInfo(list.ToArray(), text);
			}
			PushNotificationPublishingContext context = new PushNotificationPublishingContext(base.GetType().Name, this.AuthenticatedTenantId, false, text);
			foreach (string text3 in base.RequestInstance.AppIds)
			{
				if (!this.Configuration.AzureSendPublisherSettings.ContainsKey(text3))
				{
					string text4 = base.Budget.Owner.ToNullableString(null);
					PushNotificationsCrimsonEvents.CannotResolvePublisherSettings.LogPeriodic<string, string, string>(text4, CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, text4, text3, (base.RequestInstance != null) ? base.RequestInstance.ToFullString() : "null");
				}
				else if (!this.Configuration.AzureSendPublisherSettings[text3].IsMultifactorRegistrationEnabled)
				{
					string text5 = (base.Budget == null) ? base.Budget.ToNullableString(null) : base.Budget.Owner.ToNullableString(null);
					PushNotificationsCrimsonEvents.MultifactorRegistrationDisabled.LogPeriodic<string, string, string>(text5, CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, text5, text3, (base.RequestInstance != null) ? base.RequestInstance.ToFullString() : "null");
				}
				else
				{
					AzureChannelSettings channelSettings = this.Configuration.AzureSendPublisherSettings[text3].ChannelSettings;
					this.PublisherManager.Publish(new AzureHubDefinition(text, text3), context);
					string resourceUri = channelSettings.UriTemplate.CreateOnPremResourceStringUri(text3, text);
					AzureSasToken azureSasToken = channelSettings.AzureSasTokenProvider.CreateSasToken(resourceUri, 93600);
					list.Add(new AzureAppConfigData(text3, azureSasToken.ToJson(), channelSettings.PartitionName));
				}
			}
			return new AzureAppConfigResponseInfo(list.ToArray(), text);
		}

		protected override ResourceKey[] InternalGetResources()
		{
			return new ResourceKey[]
			{
				ProcessorResourceKey.Local
			};
		}
	}
}
