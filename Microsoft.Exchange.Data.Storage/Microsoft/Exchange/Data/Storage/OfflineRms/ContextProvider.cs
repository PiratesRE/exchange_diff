using System;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.RightsManagementServices.Provider;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ContextProvider : IContextProvider, IDisposable
	{
		public ContextProvider(IPerTenantRMSTrustedPublishingDomainConfiguration perTenantconfig)
		{
			if (perTenantconfig == null)
			{
				throw new ArgumentNullException("perTenantconfig");
			}
			this.configProvider = new ConfigurationInformationProvider(perTenantconfig);
			this.privateKeyProvider = new TrustedPublishingDomainPrivateKeyProvider(null, perTenantconfig.PrivateKeys);
			this.globalConfigurationProvider = new GlobalConfigurationCacheProvider();
			this.directoryServiceProvider = new DirectoryServiceProvider();
		}

		public ContextProvider(RmsClientManagerContext clientContext, PerTenantRMSTrustedPublishingDomainConfiguration perTenantconfig)
		{
			if (clientContext == null)
			{
				throw new ArgumentNullException("clientContext");
			}
			if (perTenantconfig == null)
			{
				throw new ArgumentNullException("perTenantconfig");
			}
			if (perTenantconfig.PrivateKeys == null)
			{
				throw new ArgumentNullException("perTenantconfig.PrivateKeys");
			}
			this.clientContext = clientContext;
			this.configProvider = new ConfigurationInformationProvider(perTenantconfig);
			this.privateKeyProvider = new TrustedPublishingDomainPrivateKeyProvider(clientContext, perTenantconfig.PrivateKeys);
			this.globalConfigurationProvider = new GlobalConfigurationCacheProvider();
			this.directoryServiceProvider = new DirectoryServiceProvider(clientContext);
		}

		public IConfigurationInformationProvider ConfigurationInformation
		{
			get
			{
				return this.configProvider;
			}
		}

		public IGlobalConfigurationAndCacheProvider GlobalConfigurationAndCache
		{
			get
			{
				return this.globalConfigurationProvider;
			}
		}

		public ITrustedPublishingDomainPrivateKeyProvider TrustedPublishingDomainPrivateKey
		{
			get
			{
				return this.privateKeyProvider;
			}
		}

		public IDirectoryServiceProvider DirectoryService
		{
			get
			{
				return this.directoryServiceProvider;
			}
		}

		public void Dispose()
		{
			if (this.privateKeyProvider != null)
			{
				this.privateKeyProvider.Dispose();
			}
		}

		public void Notify(EventNotificationEntry entry, EventNotificationPropertyBag propertyBag)
		{
			if (entry == 4)
			{
				string text = (propertyBag != null) ? propertyBag.ObjectGuid : string.Empty;
				StorageGlobals.EventLogger.LogEvent(this.clientContext.OrgId, StorageEventLogConstants.Tuple_UnknownTemplateInPublishingLicense, text, text);
				string notificationReason = string.Format("Exchange could not match the RMS template with Id {0} specified in the publishing license against templates configured for this tenant.", text);
				EventNotificationItem.Publish(ExchangeComponent.Rms.Name, "UnknownTemplateInPublishingLicense", null, notificationReason, ResultSeverityLevel.Warning, false);
			}
		}

		private readonly ConfigurationInformationProvider configProvider;

		private readonly TrustedPublishingDomainPrivateKeyProvider privateKeyProvider;

		private readonly IGlobalConfigurationAndCacheProvider globalConfigurationProvider;

		private readonly IDirectoryServiceProvider directoryServiceProvider;

		private readonly RmsClientManagerContext clientContext;
	}
}
