using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Auditing;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal class ConfigurationCache
	{
		public ConfigurationCache()
		{
			this.configCache = new CacheWithExpiration<OrganizationId, ConfigWrapper>(16, ConfigurationCache.cacheExpirationTime, null);
		}

		public ConfigWrapper FindOrCreate(OrganizationId organizationId, LogMessageDelegate logMessage)
		{
			DateTime utcNow = DateTime.UtcNow;
			ConfigWrapper configWrapper = null;
			if (!this.configCache.TryGetValue(organizationId, utcNow, out configWrapper))
			{
				ConfigurationCache.ConfigurationPolicy configurationPolicy = new ConfigurationCache.ConfigurationPolicy(organizationId);
				configWrapper = new ConfigWrapper(configurationPolicy, utcNow, logMessage);
				this.configCache.TryAdd(organizationId, utcNow, configWrapper);
			}
			return configWrapper;
		}

		private const int cacheSize = 16;

		private static readonly TimeSpan cacheExpirationTime = TimeSpan.FromMinutes(15.0);

		private readonly CacheWithExpiration<OrganizationId, ConfigWrapper> configCache;

		private class ConfigurationPolicy : IConfigurationPolicy
		{
			public ConfigurationPolicy(OrganizationId organizationId)
			{
				this.organizationId = organizationId;
			}

			public bool RunningOnDataCenter
			{
				get
				{
					return AdminAuditLogHelper.RunningOnDataCenter;
				}
			}

			public OrganizationId OrganizationId
			{
				get
				{
					return this.organizationId;
				}
			}

			public IExchangePrincipal ExchangePrincipal
			{
				get
				{
					return this.exchangePrincipal;
				}
			}

			public ArbitrationMailboxStatus CheckArbitrationMailboxStatus(out Exception initialError)
			{
				ADUser aduser;
				return AdminAuditLogHelper.CheckArbitrationMailboxStatus(this.organizationId, out aduser, out this.exchangePrincipal, out initialError);
			}

			public IAuditLog CreateLogger(ArbitrationMailboxStatus mailboxStatus)
			{
				if (mailboxStatus == ArbitrationMailboxStatus.FFO)
				{
					return AuditLoggerFactory.CreateForFFO(this.organizationId);
				}
				return AuditLoggerFactory.Create(this.exchangePrincipal, mailboxStatus);
			}

			public IAdminAuditLogConfig GetAdminAuditLogConfig()
			{
				IConfigurationSession configSession = AdminAuditLogHelper.CreateSession(this.organizationId, null);
				AdminAuditLogConfig adminAuditLogConfig = AdminAuditLogHelper.GetAdminAuditLogConfig(configSession);
				if (adminAuditLogConfig != null)
				{
					return new ConfigurationCache.ConfigurationPolicy.AdminAuditLogConfigAdapter(adminAuditLogConfig);
				}
				return null;
			}

			private readonly OrganizationId organizationId;

			private ExchangePrincipal exchangePrincipal;

			private class AdminAuditLogConfigAdapter : IAdminAuditLogConfig
			{
				public AdminAuditLogConfigAdapter(AdminAuditLogConfig adminAuditLogConfig)
				{
					this.adminAuditLogConfig = adminAuditLogConfig;
				}

				public MultiValuedProperty<string> AdminAuditLogParameters
				{
					get
					{
						return this.adminAuditLogConfig.AdminAuditLogParameters;
					}
				}

				public MultiValuedProperty<string> AdminAuditLogCmdlets
				{
					get
					{
						return this.adminAuditLogConfig.AdminAuditLogCmdlets;
					}
				}

				public MultiValuedProperty<string> AdminAuditLogExcludedCmdlets
				{
					get
					{
						return this.adminAuditLogConfig.AdminAuditLogExcludedCmdlets;
					}
				}

				public bool AdminAuditLogEnabled
				{
					get
					{
						return this.adminAuditLogConfig.AdminAuditLogEnabled;
					}
				}

				public bool IsValidAuditLogMailboxAddress
				{
					get
					{
						SmtpAddress adminAuditLogMailbox = this.adminAuditLogConfig.AdminAuditLogMailbox;
						return this.adminAuditLogConfig.AdminAuditLogMailbox.IsValidAddress;
					}
				}

				public bool TestCmdletLoggingEnabled
				{
					get
					{
						return this.adminAuditLogConfig.TestCmdletLoggingEnabled;
					}
				}

				public AuditLogLevel LogLevel
				{
					get
					{
						return this.adminAuditLogConfig.LogLevel;
					}
				}

				private AdminAuditLogConfig adminAuditLogConfig;
			}
		}
	}
}
