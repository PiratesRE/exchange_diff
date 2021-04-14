using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Hybrid.Entity
{
	internal class OutboundConnector : IOutboundConnector, IEntity<IOutboundConnector>
	{
		public OutboundConnector()
		{
		}

		public OutboundConnector(TenantOutboundConnector oc)
		{
			this.Identity = (ADObjectId)oc.Identity;
			this.Name = oc.Name;
			this.TlsDomain = TaskCommon.ToStringOrNull(oc.TlsDomain);
			this.ConnectorType = oc.ConnectorType;
			this.ConnectorSource = oc.ConnectorSource;
			this.RecipientDomains = oc.RecipientDomains;
			this.SmartHosts = oc.SmartHosts;
			this.TlsSettings = oc.TlsSettings;
			this.CloudServicesMailEnabled = oc.CloudServicesMailEnabled;
			this.RouteAllMessagesViaOnPremises = oc.RouteAllMessagesViaOnPremises;
		}

		public OutboundConnector(string name, string tlsDomain, MultiValuedProperty<SmtpDomainWithSubdomains> recipientDomains, MultiValuedProperty<SmartHost> smartHosts, bool routeAllMessagesViaOnPremises)
		{
			this.Name = name;
			this.TlsDomain = tlsDomain;
			this.ConnectorType = TenantConnectorType.OnPremises;
			this.RecipientDomains = recipientDomains;
			this.SmartHosts = smartHosts;
			this.TlsSettings = new TlsAuthLevel?(TlsAuthLevel.DomainValidation);
			this.CloudServicesMailEnabled = true;
			this.RouteAllMessagesViaOnPremises = routeAllMessagesViaOnPremises;
		}

		public ADObjectId Identity { get; set; }

		public string Name { get; set; }

		public string TlsDomain { get; set; }

		public TenantConnectorType ConnectorType { get; set; }

		public TenantConnectorSource ConnectorSource { get; set; }

		public MultiValuedProperty<SmtpDomainWithSubdomains> RecipientDomains { get; set; }

		public MultiValuedProperty<SmartHost> SmartHosts { get; set; }

		public TlsAuthLevel? TlsSettings { get; set; }

		public bool CloudServicesMailEnabled { get; set; }

		public bool RouteAllMessagesViaOnPremises { get; set; }

		public override string ToString()
		{
			if (this.Identity != null)
			{
				return this.Identity.ToString();
			}
			return "<New>";
		}

		public bool Equals(IOutboundConnector obj)
		{
			return this.TlsSettings == obj.TlsSettings && this.ConnectorType == obj.ConnectorType && this.CloudServicesMailEnabled == obj.CloudServicesMailEnabled && this.RouteAllMessagesViaOnPremises == obj.RouteAllMessagesViaOnPremises && TaskCommon.ContainsSame<SmartHost>(this.SmartHosts, obj.SmartHosts) && string.Equals(this.Name, obj.Name, StringComparison.InvariantCultureIgnoreCase) && string.Equals(this.TlsDomain, obj.TlsDomain, StringComparison.InvariantCultureIgnoreCase) && TaskCommon.ContainsSame<SmtpDomainWithSubdomains>(this.RecipientDomains, obj.RecipientDomains);
		}

		public IOutboundConnector Clone(ADObjectId identity)
		{
			OutboundConnector outboundConnector = new OutboundConnector();
			outboundConnector.UpdateFrom(this);
			outboundConnector.Identity = identity;
			return outboundConnector;
		}

		public void UpdateFrom(IOutboundConnector obj)
		{
			this.Name = obj.Name;
			this.TlsDomain = obj.TlsDomain;
			this.ConnectorType = obj.ConnectorType;
			this.ConnectorSource = obj.ConnectorSource;
			this.RecipientDomains = obj.RecipientDomains;
			this.SmartHosts = obj.SmartHosts;
			this.TlsSettings = obj.TlsSettings;
			this.CloudServicesMailEnabled = obj.CloudServicesMailEnabled;
			this.RouteAllMessagesViaOnPremises = obj.RouteAllMessagesViaOnPremises;
		}
	}
}
