using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RmsConfiguration
	{
		private RmsConfiguration()
		{
		}

		internal static RmsConfiguration Instance
		{
			get
			{
				return RmsConfiguration.instance;
			}
		}

		public void Load(bool enableOrgAndTemplateCache)
		{
			if (this.enabled)
			{
				return;
			}
			lock (this)
			{
				if (!this.enabled)
				{
					this.irmConfigCache = new TenantConfigurationCache<RmsConfiguration.PerTenantIRMConfiguration>(RmsConfiguration.CacheSizeInBytes, RmsConfiguration.CacheTimeout, RmsConfiguration.CacheTimeout, null, null);
					this.enableOrgAndTemplateCache = enableOrgAndTemplateCache;
					this.transportConfigCache = new TenantConfigurationCache<RmsConfiguration.PerTenantTransportSettings>(RmsConfiguration.CacheSizeInBytes, RmsConfiguration.CacheTimeout, RmsConfiguration.CacheTimeout, null, null);
					if (this.enableOrgAndTemplateCache)
					{
						this.irmOrgCache = new TenantConfigurationCache<RmsConfiguration.PerTenantOrganizationConfig>(RmsConfiguration.CacheSizeInBytes, RmsConfiguration.CacheTimeout, RmsConfiguration.CacheTimeout, null, null);
						this.templateCache = new TenantConfigurationCache<RmsConfiguration.PerTenantTemplateInfo>(RmsClientManager.AppSettings.TemplateCacheSizeInBytes, RmsClientManager.AppSettings.TemplateCacheExpirationInterval, TimeSpan.Zero, null, null);
					}
					else
					{
						this.firstOrgCache = new RmsConfiguration.FirstOrgServiceLocationsCache();
					}
					this.enabled = true;
				}
			}
		}

		public bool GetTenantExternalDirectoryOrgId(OrganizationId orgId, out Guid externalDirectoryOrgId)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			externalDirectoryOrgId = Guid.Empty;
			if (orgId == OrganizationId.ForestWideOrgId)
			{
				return false;
			}
			RmsConfiguration.PerTenantOrganizationConfig tenantOrganizationConfig = this.GetTenantOrganizationConfig(orgId);
			externalDirectoryOrgId = tenantOrganizationConfig.ExternalDirectoryOrgId;
			return true;
		}

		public Uri GetTenantServiceLocation(OrganizationId orgId)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			if (orgId == OrganizationId.ForestWideOrgId)
			{
				return this.GetFirstOrgServiceLocation(ServiceType.Certification);
			}
			RmsConfiguration.PerTenantIRMConfiguration tenantIrmConfig = this.GetTenantIrmConfig(orgId);
			if (tenantIrmConfig.ServiceLocation != null)
			{
				return tenantIrmConfig.ServiceLocation;
			}
			return null;
		}

		public Uri GetTenantPublishingLocation(OrganizationId orgId)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			if (orgId == OrganizationId.ForestWideOrgId)
			{
				return this.GetFirstOrgServiceLocation(ServiceType.Publishing);
			}
			RmsConfiguration.PerTenantIRMConfiguration tenantIrmConfig = this.GetTenantIrmConfig(orgId);
			if (tenantIrmConfig.PublishingLocation != null)
			{
				return tenantIrmConfig.PublishingLocation;
			}
			if (tenantIrmConfig.ServiceLocation != null)
			{
				return tenantIrmConfig.ServiceLocation;
			}
			return null;
		}

		public List<Uri> GetTenantLicensingLocations(OrganizationId orgId)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			RmsConfiguration.PerTenantIRMConfiguration tenantIrmConfig = this.GetTenantIrmConfig(orgId);
			return tenantIrmConfig.LicensingLocations;
		}

		public TransportDecryptionSetting GetTenantTransportDecryptionSetting(OrganizationId orgId)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			RmsConfiguration.PerTenantIRMConfiguration tenantIrmConfig = this.GetTenantIrmConfig(orgId);
			return tenantIrmConfig.TransportDecryptionSetting;
		}

		public bool IsJournalReportDecryptionEnabledForTenant(OrganizationId orgId)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			RmsConfiguration.PerTenantIRMConfiguration tenantIrmConfig = this.GetTenantIrmConfig(orgId);
			return tenantIrmConfig.JournalReportDecryptionEnabled;
		}

		public bool IsExternalLicensingEnabledForTenant(OrganizationId orgId)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			RmsConfiguration.PerTenantIRMConfiguration tenantIrmConfig = this.GetTenantIrmConfig(orgId);
			return tenantIrmConfig.ExternalLicensingEnabled;
		}

		public bool IsInternalLicensingEnabledForTenant(OrganizationId orgId)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			RmsConfiguration.PerTenantIRMConfiguration tenantIrmConfig = this.GetTenantIrmConfig(orgId);
			return tenantIrmConfig.InternalLicensingEnabled;
		}

		public bool IsSearchEnabledForTenant(OrganizationId orgId)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			RmsConfiguration.PerTenantIRMConfiguration tenantIrmConfig = this.GetTenantIrmConfig(orgId);
			return tenantIrmConfig.SearchEnabled;
		}

		public bool IsClientAccessServerEnabledForTenant(OrganizationId orgId)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			RmsConfiguration.PerTenantIRMConfiguration tenantIrmConfig = this.GetTenantIrmConfig(orgId);
			return tenantIrmConfig.ClientAccessServerEnabled;
		}

		public bool IsInternalServerPreLicensingEnabledForTenant(OrganizationId orgId)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			RmsConfiguration.PerTenantIRMConfiguration tenantIrmConfig = this.GetTenantIrmConfig(orgId);
			return (tenantIrmConfig.SearchEnabled || tenantIrmConfig.ClientAccessServerEnabled) && tenantIrmConfig.InternalLicensingEnabled;
		}

		public bool IsExternalServerPreLicensingEnabledForTenant(OrganizationId orgId)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			RmsConfiguration.PerTenantIRMConfiguration tenantIrmConfig = this.GetTenantIrmConfig(orgId);
			return (tenantIrmConfig.SearchEnabled || tenantIrmConfig.ClientAccessServerEnabled) && tenantIrmConfig.ExternalLicensingEnabled;
		}

		public bool IsInternetConfidentialEnabledForTenant(OrganizationId orgId)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			RmsConfiguration.PerTenantIRMConfiguration tenantIrmConfig = this.GetTenantIrmConfig(orgId);
			return tenantIrmConfig.InternetConfidentialEnabled;
		}

		public bool IsEDiscoverySuperUserEnabledForTenant(OrganizationId orgId)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			RmsConfiguration.PerTenantIRMConfiguration tenantIrmConfig = this.GetTenantIrmConfig(orgId);
			return tenantIrmConfig.EDiscoverySuperUserEnabled;
		}

		public Uri GetTenantRMSOnlineKeySharingLocation(OrganizationId orgId)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			if (orgId == OrganizationId.ForestWideOrgId)
			{
				throw new InvalidOperationException("RMSOnlineKeySharingLocation is a datacenter-only property");
			}
			RmsConfiguration.PerTenantIRMConfiguration tenantIrmConfig = this.GetTenantIrmConfig(orgId);
			return tenantIrmConfig.RMSOnlineKeySharingLocation;
		}

		public byte GetTenantServerCertificatesVersion(OrganizationId orgId)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			RmsConfiguration.PerTenantIRMConfiguration tenantIrmConfig = this.GetTenantIrmConfig(orgId);
			return tenantIrmConfig.ServerCertificatesVersion;
		}

		public string GetTenantFederatedMailbox(OrganizationId orgId)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			string result;
			try
			{
				RmsConfiguration.PerTenantTransportSettings value;
				if (RmsClientManager.ADSession != null)
				{
					value = this.transportConfigCache.GetValue(RmsClientManager.ADSession);
				}
				else
				{
					value = this.transportConfigCache.GetValue(orgId);
				}
				result = (string)value.OrgFederatedMailbox;
			}
			catch (ADTransientException innerException)
			{
				throw new ExchangeConfigurationException(ServerStrings.FailedToReadConfiguration, innerException);
			}
			catch (ADOperationException innerException2)
			{
				throw new ExchangeConfigurationException(ServerStrings.FailedToReadConfiguration, innerException2);
			}
			return result;
		}

		public bool AreRmsTemplatesInCache(OrganizationId organizationId)
		{
			return this.templateCache != null && this.templateCache.ContainsInCache(organizationId);
		}

		internal IEnumerable<RmsTemplate> GetRmsTemplates(OrganizationId orgId)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			if (orgId == OrganizationId.ForestWideOrgId)
			{
				throw new InvalidOperationException("GetRmsTemplates called with ForestWideOrgId");
			}
			if (!this.enableOrgAndTemplateCache)
			{
				throw new NotSupportedException("This function is not supported when offlineRms is not enabled");
			}
			RmsConfiguration.PerTenantTemplateInfo tenantTemplateConfig = this.GetTenantTemplateConfig(orgId);
			return tenantTemplateConfig.Templates;
		}

		internal RmsTemplate GetRmsTemplate(OrganizationId orgId, Guid templateId)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			if (!this.enableOrgAndTemplateCache)
			{
				throw new NotSupportedException("This function is only supported in datacenter where offlineRms is enabled");
			}
			if (orgId == OrganizationId.ForestWideOrgId)
			{
				throw new InvalidOperationException("GetRmsTemplate called with ForestWideOrgId");
			}
			if (templateId == RmsTemplate.DoNotForward.Id)
			{
				return RmsTemplate.DoNotForward;
			}
			if (templateId == RmsTemplate.InternetConfidential.Id)
			{
				return RmsTemplate.InternetConfidential;
			}
			RmsConfiguration.PerTenantTemplateInfo tenantTemplateConfig = this.GetTenantTemplateConfig(orgId);
			RmsTemplate result;
			if (!tenantTemplateConfig.TryGetValue(templateId, out result))
			{
				return null;
			}
			return result;
		}

		private RmsConfiguration.PerTenantOrganizationConfig GetTenantOrganizationConfig(OrganizationId orgId)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			RmsConfiguration.PerTenantOrganizationConfig value;
			try
			{
				value = this.irmOrgCache.GetValue(orgId);
			}
			catch (ADTransientException innerException)
			{
				throw new ExchangeConfigurationException(ServerStrings.FailedToReadConfiguration, innerException);
			}
			catch (ADOperationException innerException2)
			{
				throw new ExchangeConfigurationException(ServerStrings.FailedToReadConfiguration, innerException2);
			}
			return value;
		}

		private RmsConfiguration.PerTenantIRMConfiguration GetTenantIrmConfig(OrganizationId orgId)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			RmsConfiguration.PerTenantIRMConfiguration result;
			try
			{
				RmsConfiguration.PerTenantIRMConfiguration value;
				if (RmsClientManager.ADSession != null)
				{
					value = this.irmConfigCache.GetValue(RmsClientManager.ADSession);
				}
				else
				{
					value = this.irmConfigCache.GetValue(orgId);
				}
				result = value;
			}
			catch (ADTransientException innerException)
			{
				throw new ExchangeConfigurationException(ServerStrings.FailedToReadConfiguration, innerException);
			}
			catch (ADOperationException innerException2)
			{
				throw new ExchangeConfigurationException(ServerStrings.FailedToReadConfiguration, innerException2);
			}
			return result;
		}

		private RmsConfiguration.PerTenantTemplateInfo GetTenantTemplateConfig(OrganizationId orgId)
		{
			ArgumentValidator.ThrowIfNull("orgId", orgId);
			RmsConfiguration.PerTenantTemplateInfo value;
			try
			{
				value = this.templateCache.GetValue(orgId);
			}
			catch (ADTransientException innerException)
			{
				throw new ExchangeConfigurationException(ServerStrings.FailedToReadConfiguration, innerException);
			}
			catch (ADOperationException innerException2)
			{
				throw new ExchangeConfigurationException(ServerStrings.FailedToReadConfiguration, innerException2);
			}
			return value;
		}

		private Uri GetFirstOrgServiceLocation(ServiceType serviceType)
		{
			if (this.enableOrgAndTemplateCache)
			{
				return null;
			}
			RmsConfiguration.FirstOrgServiceLocationsCache.FirstOrgServiceLocations firstOrgServiceLocations = this.firstOrgCache.GetFirstOrgServiceLocations();
			if (firstOrgServiceLocations == null)
			{
				return null;
			}
			switch (serviceType)
			{
			case ServiceType.Certification:
				return firstOrgServiceLocations.CertificationLocation;
			case ServiceType.Activation | ServiceType.Certification:
				goto IL_45;
			case ServiceType.Publishing:
				break;
			default:
				if (serviceType != ServiceType.ClientLicensor)
				{
					goto IL_45;
				}
				break;
			}
			return firstOrgServiceLocations.PublishingLocaton;
			IL_45:
			return null;
		}

		private static readonly Trace Tracer = ExTraceGlobals.RightsManagementTracer;

		private static readonly TimeSpan CacheTimeout = TimeSpan.FromHours(8.0);

		private static readonly long CacheSizeInBytes = (long)ByteQuantifiedSize.FromMB(1UL).ToBytes();

		private static readonly RmsConfiguration instance = new RmsConfiguration();

		private TenantConfigurationCache<RmsConfiguration.PerTenantIRMConfiguration> irmConfigCache;

		private TenantConfigurationCache<RmsConfiguration.PerTenantOrganizationConfig> irmOrgCache;

		private TenantConfigurationCache<RmsConfiguration.PerTenantTransportSettings> transportConfigCache;

		private TenantConfigurationCache<RmsConfiguration.PerTenantTemplateInfo> templateCache;

		private bool enabled;

		private RmsConfiguration.FirstOrgServiceLocationsCache firstOrgCache;

		private bool enableOrgAndTemplateCache;

		private sealed class FirstOrgServiceLocationsCache
		{
			public FirstOrgServiceLocationsCache()
			{
				this.objHashCode = this.GetHashCode();
				this.Refresh(true);
			}

			public RmsConfiguration.FirstOrgServiceLocationsCache.FirstOrgServiceLocations GetFirstOrgServiceLocations()
			{
				this.Refresh(false);
				return this.firstOrgServiceLocations;
			}

			private void Refresh(bool throwExceptions)
			{
				if (DateTime.UtcNow < this.timeout)
				{
					return;
				}
				RmsConfiguration.Tracer.TraceDebug<bool>((long)this.objHashCode, "Refreshing the first org service locations cache. Initializing for the first time: {0}", throwExceptions);
				lock (this)
				{
					if (DateTime.UtcNow < this.timeout)
					{
						return;
					}
					RmsConfiguration.FirstOrgServiceLocationsCache.FirstOrgServiceLocations firstOrgServiceLocations = new RmsConfiguration.FirstOrgServiceLocationsCache.FirstOrgServiceLocations();
					try
					{
						firstOrgServiceLocations.CertificationLocation = DrmClientUtils.GetServiceLocation(SafeRightsManagementSessionHandle.InvalidHandle, ServiceType.Certification, ServiceLocation.Enterprise, null);
						firstOrgServiceLocations.PublishingLocaton = DrmClientUtils.GetServiceLocation(SafeRightsManagementSessionHandle.InvalidHandle, ServiceType.ClientLicensor, ServiceLocation.Enterprise, null);
						this.firstOrgServiceLocations = firstOrgServiceLocations;
					}
					catch (RightsManagementException ex)
					{
						RmsConfiguration.Tracer.TraceError<RightsManagementException>((long)this.objHashCode, "Failed to get the service locations. Error {0}", ex);
						if (ex.FailureCode != RightsManagementFailureCode.ServiceNotFound && throwExceptions)
						{
							throw;
						}
						RmsConfiguration.Tracer.TraceError((long)this.objHashCode, "Failed to refresh the first org cache");
					}
					if (this.firstOrgServiceLocations != null)
					{
						this.timeout = DateTime.UtcNow.Add(RmsConfiguration.FirstOrgServiceLocationsCache.ExpirationTime);
					}
				}
				RmsConfiguration.Tracer.TraceDebug<Uri, Uri, DateTime>((long)this.objHashCode, "Values for the first org cache. CertificationLocation: {0}, PublishingLocation: {1}, ExpirationTime: {2}", (this.firstOrgServiceLocations != null) ? this.firstOrgServiceLocations.CertificationLocation : null, (this.firstOrgServiceLocations != null) ? this.firstOrgServiceLocations.PublishingLocaton : null, this.timeout);
			}

			private static readonly TimeSpan ExpirationTime = TimeSpan.FromMinutes(15.0);

			private readonly int objHashCode;

			private DateTime timeout = DateTime.MinValue;

			private RmsConfiguration.FirstOrgServiceLocationsCache.FirstOrgServiceLocations firstOrgServiceLocations;

			public class FirstOrgServiceLocations
			{
				public Uri CertificationLocation;

				public Uri PublishingLocaton;
			}
		}

		private sealed class PerTenantIRMConfiguration : TenantConfigurationCacheableItem<IRMConfiguration>
		{
			public override long ItemSize
			{
				get
				{
					return (long)this.estimatedSize;
				}
			}

			public override void ReadData(IConfigurationSession session)
			{
				IRMConfiguration irmconfiguration = IRMConfiguration.Read(session);
				this.JournalReportDecryptionEnabled = irmconfiguration.JournalReportDecryptionEnabled;
				this.ClientAccessServerEnabled = irmconfiguration.ClientAccessServerEnabled;
				this.SearchEnabled = irmconfiguration.SearchEnabled;
				this.ExternalLicensingEnabled = irmconfiguration.ExternalLicensingEnabled;
				this.InternalLicensingEnabled = irmconfiguration.InternalLicensingEnabled;
				this.TransportDecryptionSetting = irmconfiguration.TransportDecryptionSetting;
				this.InternetConfidentialEnabled = irmconfiguration.InternetConfidentialEnabled;
				this.EDiscoverySuperUserEnabled = irmconfiguration.EDiscoverySuperUserEnabled;
				this.ServerCertificatesVersion = (byte)irmconfiguration.ServerCertificatesVersion;
				this.estimatedSize += 10;
				if (irmconfiguration.ServiceLocation != null)
				{
					this.ServiceLocation = irmconfiguration.ServiceLocation;
					this.estimatedSize += irmconfiguration.ServiceLocation.OriginalString.Length * 2;
				}
				if (irmconfiguration.PublishingLocation != null)
				{
					this.PublishingLocation = irmconfiguration.PublishingLocation;
					this.estimatedSize += irmconfiguration.PublishingLocation.OriginalString.Length * 2;
				}
				if (irmconfiguration.LicensingLocation != null && !MultiValuedPropertyBase.IsNullOrEmpty(irmconfiguration.LicensingLocation))
				{
					this.LicensingLocations = new List<Uri>(irmconfiguration.LicensingLocation.Count + 1);
					foreach (Uri uri in irmconfiguration.LicensingLocation)
					{
						if (uri != null)
						{
							this.LicensingLocations.Add(uri);
							this.estimatedSize += uri.OriginalString.Length * 2;
						}
					}
				}
				if (irmconfiguration.RMSOnlineKeySharingLocation != null)
				{
					this.RMSOnlineKeySharingLocation = irmconfiguration.RMSOnlineKeySharingLocation;
					this.estimatedSize += irmconfiguration.RMSOnlineKeySharingLocation.OriginalString.Length * 2;
				}
				if (base.OrganizationId == OrganizationId.ForestWideOrgId)
				{
					Uri firstOrgServiceLocation = RmsConfiguration.Instance.GetFirstOrgServiceLocation(ServiceType.ClientLicensor);
					if (firstOrgServiceLocation != null && !this.LicensingLocations.Contains(firstOrgServiceLocation))
					{
						this.LicensingLocations.Add(firstOrgServiceLocation);
						this.estimatedSize += firstOrgServiceLocation.OriginalString.Length * 2;
					}
				}
				else if (irmconfiguration.PublishingLocation != null && !this.LicensingLocations.Contains(irmconfiguration.PublishingLocation))
				{
					this.LicensingLocations.Add(irmconfiguration.PublishingLocation);
					this.estimatedSize += irmconfiguration.PublishingLocation.OriginalString.Length * 2;
				}
				else if (irmconfiguration.ServiceLocation != null && !this.LicensingLocations.Contains(irmconfiguration.ServiceLocation))
				{
					this.LicensingLocations.Add(irmconfiguration.ServiceLocation);
					this.estimatedSize += irmconfiguration.ServiceLocation.OriginalString.Length * 2;
				}
				if (base.OrganizationId == OrganizationId.ForestWideOrgId && (!this.InternalLicensingEnabled || RmsClientManager.FirstOrgTemplateCacheVersion != this.ServerCertificatesVersion))
				{
					RmsClientManager.TemplateCacheForFirstOrg = null;
				}
			}

			public Uri ServiceLocation;

			public Uri PublishingLocation;

			public List<Uri> LicensingLocations = new List<Uri>(1);

			public bool JournalReportDecryptionEnabled;

			public bool ExternalLicensingEnabled;

			public bool InternalLicensingEnabled;

			public bool SearchEnabled;

			public bool ClientAccessServerEnabled;

			public TransportDecryptionSetting TransportDecryptionSetting;

			public bool InternetConfidentialEnabled;

			public bool EDiscoverySuperUserEnabled;

			public Uri RMSOnlineKeySharingLocation;

			public byte ServerCertificatesVersion;

			private int estimatedSize;
		}

		private sealed class PerTenantOrganizationConfig : TenantConfigurationCacheableItem<ExchangeConfigurationUnit>
		{
			public override long ItemSize
			{
				get
				{
					return 16L;
				}
			}

			public Guid ExternalDirectoryOrgId
			{
				get
				{
					return this.externalDirectoryOrgId;
				}
			}

			public override void ReadData(IConfigurationSession session)
			{
				ExchangeConfigurationUnit[] array = session.Find<ExchangeConfigurationUnit>(null, QueryScope.SubTree, null, null, 0);
				if (array == null || array.Length == 0)
				{
					return;
				}
				ExchangeConfigurationUnit exchangeConfigurationUnit = array.FirstOrDefault<ExchangeConfigurationUnit>();
				if (exchangeConfigurationUnit != null)
				{
					this.externalDirectoryOrgId = Guid.Parse(exchangeConfigurationUnit.ExternalDirectoryOrganizationId);
				}
			}

			private Guid externalDirectoryOrgId;
		}

		private sealed class PerTenantTransportSettings : TenantConfigurationCacheableItem<TransportConfigContainer>
		{
			public override long ItemSize
			{
				get
				{
					return (long)this.orgFederatedMailbox.Length;
				}
			}

			public SmtpAddress OrgFederatedMailbox
			{
				get
				{
					return this.orgFederatedMailbox;
				}
			}

			public override void ReadData(IConfigurationSession session)
			{
				TransportConfigContainer[] array = session.Find<TransportConfigContainer>(null, QueryScope.SubTree, null, null, 0);
				if (array == null || array.Length == 0)
				{
					return;
				}
				this.orgFederatedMailbox = array[0].OrganizationFederatedMailbox;
			}

			private SmtpAddress orgFederatedMailbox;
		}

		private sealed class PerTenantTemplateInfo : TenantConfigurationCacheableItem<RMSTrustedPublishingDomain>
		{
			public override long ItemSize
			{
				get
				{
					return this.estimatedSize;
				}
			}

			public IEnumerable<RmsTemplate> Templates
			{
				get
				{
					return this.templates.Values;
				}
			}

			public bool TryGetValue(Guid templateId, out RmsTemplate template)
			{
				return this.templates.TryGetValue(templateId, out template);
			}

			public override void ReadData(IConfigurationSession session)
			{
				RMSTrustedPublishingDomain[] array = session.Find<RMSTrustedPublishingDomain>(null, QueryScope.SubTree, null, null, 0);
				if (array == null || array.Length == 0)
				{
					return;
				}
				foreach (RMSTrustedPublishingDomain rmstrustedPublishingDomain in array)
				{
					if (rmstrustedPublishingDomain.Default && !MultiValuedPropertyBase.IsNullOrEmpty(rmstrustedPublishingDomain.RMSTemplates))
					{
						foreach (string encodedTemplate in rmstrustedPublishingDomain.RMSTemplates)
						{
							string text = null;
							try
							{
								RmsTemplateType rmsTemplateType;
								text = RMUtil.DecompressTemplate(encodedTemplate, out rmsTemplateType);
								if (rmsTemplateType == RmsTemplateType.Distributed)
								{
									RmsTemplate rmsTemplate = RmsTemplate.CreateServerTemplateFromTemplateDefinition(text, rmsTemplateType);
									this.templates.Add(rmsTemplate.Id, rmsTemplate);
									this.estimatedSize += rmsTemplate.ItemSize + 16L;
								}
							}
							catch (FormatException arg)
							{
								RmsConfiguration.Tracer.TraceError<string, FormatException>((long)this.GetHashCode(), "Failed to read template {0}. Error {1}", text, arg);
							}
							catch (InvalidRpmsgFormatException arg2)
							{
								RmsConfiguration.Tracer.TraceError<string, InvalidRpmsgFormatException>((long)this.GetHashCode(), "Failed to read template {0}. Error {1}", text, arg2);
							}
							catch (RightsManagementException arg3)
							{
								RmsConfiguration.Tracer.TraceError<string, RightsManagementException>((long)this.GetHashCode(), "Failed to read template {0}. Error {1}", text, arg3);
							}
						}
					}
				}
			}

			private readonly Dictionary<Guid, RmsTemplate> templates = new Dictionary<Guid, RmsTemplate>();

			private long estimatedSize;
		}
	}
}
