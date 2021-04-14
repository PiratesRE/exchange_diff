using System;
using System.Configuration;
using System.Threading;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Servicelets.AuthAdmin
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AuthAdminContext : AnchorContext
	{
		internal AuthAdminContext(string applicationName) : base(OrganizationCapability.Management)
		{
			AnchorConfig config = AuthAdminContext.CreateConfigSchema();
			AuthAdminLogger logger = new AuthAdminLogger(applicationName, config, new ExEventLog(AuthAdminContext.ComponentGuid, "MSExchange AuthAdmin"));
			base.Initialize(applicationName, logger, config);
			this.IsMultiTenancyEnabled = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled;
		}

		internal bool IsMultiTenancyEnabled { get; private set; }

		public override CacheProcessorBase[] CreateCacheComponents(WaitHandle stopEvent)
		{
			return new CacheProcessorBase[]
			{
				new FirstOrgCacheScanner(this, stopEvent),
				new AuthAdminScheduler(this, stopEvent)
			};
		}

		internal static AnchorConfig CreateConfigSchema()
		{
			return new AuthAdminContext.AuthAdminConfig();
		}

		internal const string TrustAnySSLCertificate = "TrustAnySSLCertificate";

		internal const string EventLogSourceName = "MSExchange AuthAdmin";

		private static readonly Guid ComponentGuid = new Guid("B03300F0-060B-4EC3-89BB-1180DD8FE1BF");

		private class AuthAdminConfig : AnchorConfig
		{
			internal AuthAdminConfig() : base("AuthAdmin")
			{
				base.UpdateConfig<TimeSpan>("IdleRunDelay", TimeSpan.FromHours(12.0));
				base.UpdateConfig<TimeSpan>("ActiveRunDelay", TimeSpan.FromHours(12.0));
				base.UpdateConfig<TimeSpan>("TransientErrorRunDelay", TimeSpan.FromMinutes(15.0));
				base.UpdateConfig<int>("MaximumCacheEntrySchedulerRun", 1);
			}

			[ConfigurationProperty("TrustAnySSLCertificate", DefaultValue = false)]
			public bool TrustAnySSLCertificate
			{
				get
				{
					return this.InternalGetConfig<bool>("TrustAnySSLCertificate");
				}
				set
				{
					this.InternalSetConfig<bool>(value, "TrustAnySSLCertificate");
				}
			}
		}
	}
}
