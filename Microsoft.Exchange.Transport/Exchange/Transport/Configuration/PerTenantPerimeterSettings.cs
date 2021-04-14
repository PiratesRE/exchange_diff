using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport.Configuration
{
	internal sealed class PerTenantPerimeterSettings : TenantConfigurationCacheableItem<PerimeterConfig>
	{
		public PerTenantPerimeterSettings()
		{
		}

		public PerTenantPerimeterSettings(bool routeOutboundViaEhfEnabled, bool routeOutboundViaFfoFrontendEnabled, RoutingDomain partnerRoutingDomain, RoutingDomain partnerConnectorDomain) : base(true)
		{
			this.routeOutboundViaEhfEnabled = routeOutboundViaEhfEnabled;
			this.routeOutboundViaFfoFrontendEnabled = routeOutboundViaFfoFrontendEnabled;
			this.partnerRoutingDomain = partnerRoutingDomain;
			this.partnerConnectorDomain = partnerConnectorDomain;
		}

		public PerTenantPerimeterSettings(PerimeterConfig perimeterConfig) : base(true)
		{
			this.SetInternalData(perimeterConfig);
		}

		public bool RouteOutboundViaEhfEnabled
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.routeOutboundViaEhfEnabled;
			}
		}

		public bool MigrationInProgress
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.migrationInProgress;
			}
		}

		public bool RouteOutboundViaFfoFrontendEnabled
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.routeOutboundViaFfoFrontendEnabled;
			}
		}

		public bool EheEnabled
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.eheEnabled;
			}
		}

		public bool EheDecryptEnabled
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.eheDecryptEnabled;
			}
		}

		public RoutingDomain PartnerRoutingDomain
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.partnerRoutingDomain;
			}
		}

		public RoutingDomain PartnerConnectorDomain
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.partnerConnectorDomain;
			}
		}

		public ADObjectId MailFlowPartnerId
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				return this.mailFlowPartnerId;
			}
		}

		public override long ItemSize
		{
			get
			{
				base.ThrowIfNotInitialized(this);
				long num = (long)(3 + 3 * IntPtr.Size + 2 * (this.partnerRoutingDomain.ToString().Length + this.partnerConnectorDomain.ToString().Length));
				if (this.MailFlowPartnerId != null)
				{
					num += (long)(2 * this.MailFlowPartnerId.DistinguishedName.Length + 16);
				}
				return num;
			}
		}

		public override void ReadData(IConfigurationSession session)
		{
			PerimeterConfig[] array = session.Find<PerimeterConfig>(null, QueryScope.SubTree, null, null, 2);
			if (array == null || array.Length == 0)
			{
				ExTraceGlobals.ConfigurationTracer.TraceError<string>((long)this.GetHashCode(), "Could not find transport settings for {0}", PerTenantPerimeterSettings.GetOrgIdString(session));
				this.routeOutboundViaEhfEnabled = false;
				this.routeOutboundViaFfoFrontendEnabled = true;
				this.partnerRoutingDomain = RoutingDomain.Empty;
				this.eheEnabled = false;
				this.eheDecryptEnabled = false;
				return;
			}
			if (array.Length > 1)
			{
				ExTraceGlobals.ConfigurationTracer.TraceError<string>((long)this.GetHashCode(), "Found more than one transport settings for {0}", PerTenantPerimeterSettings.GetOrgIdString(session));
				throw new PerimeterSettingsAmbiguousException(PerTenantPerimeterSettings.GetOrgIdString(session));
			}
			this.SetInternalData(array[0]);
		}

		private static string GetOrgIdString(IConfigurationSession session)
		{
			if (!(session.SessionSettings.CurrentOrganizationId != null))
			{
				return "<First Organization>";
			}
			return session.SessionSettings.CurrentOrganizationId.ToString();
		}

		private void SetInternalData(PerimeterConfig perimeterConfig)
		{
			this.routeOutboundViaEhfEnabled = perimeterConfig.RouteOutboundViaEhfEnabled;
			this.migrationInProgress = perimeterConfig.MigrationInProgress;
			this.routeOutboundViaFfoFrontendEnabled = perimeterConfig.RouteOutboundViaFfoFrontendEnabled;
			this.eheEnabled = perimeterConfig.EheEnabled;
			this.eheDecryptEnabled = perimeterConfig.EheDecryptEnabled;
			this.mailFlowPartnerId = perimeterConfig.MailFlowPartner;
			if (perimeterConfig.PartnerRoutingDomain == null)
			{
				this.partnerRoutingDomain = RoutingDomain.Empty;
			}
			else
			{
				this.partnerRoutingDomain = new RoutingDomain(perimeterConfig.PartnerRoutingDomain.Domain);
			}
			if (perimeterConfig.PartnerConnectorDomain != null)
			{
				this.partnerConnectorDomain = new RoutingDomain(perimeterConfig.PartnerConnectorDomain.Domain);
				return;
			}
			if (perimeterConfig.PartnerRoutingDomain == null)
			{
				this.partnerConnectorDomain = RoutingDomain.Empty;
				return;
			}
			this.partnerConnectorDomain = PerTenantPerimeterSettings.defaultPartnerConnectorDomain;
		}

		private const int SizeOfGuid = 16;

		private static readonly RoutingDomain defaultPartnerConnectorDomain = new RoutingDomain("partner.routing");

		private bool routeOutboundViaEhfEnabled;

		private bool migrationInProgress;

		private bool routeOutboundViaFfoFrontendEnabled;

		private bool eheEnabled;

		private bool eheDecryptEnabled;

		private RoutingDomain partnerRoutingDomain;

		private RoutingDomain partnerConnectorDomain;

		private ADObjectId mailFlowPartnerId;
	}
}
