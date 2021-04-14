using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Hybrid.Entity
{
	internal class SendConnector : ISendConnector, IEntity<ISendConnector>
	{
		public SendConnector()
		{
		}

		public SendConnector(SmtpSendConnectorConfig sc)
		{
			this.Identity = (ADObjectId)sc.Identity;
			this.Name = sc.Name;
			this.AddressSpaces = sc.AddressSpaces;
			this.SourceTransportServers = sc.SourceTransportServers;
			this.DNSRoutingEnabled = sc.DNSRoutingEnabled;
			this.SmartHosts = sc.SmartHosts;
			this.RequireTLS = sc.RequireTLS;
			this.TlsAuthLevel = sc.TlsAuthLevel;
			this.TlsDomain = TaskCommon.ToStringOrNull(sc.TlsDomain);
			this.ErrorPolicies = sc.ErrorPolicies;
			this.TlsCertificateName = sc.TlsCertificateName;
			this.CloudServicesMailEnabled = sc.CloudServicesMailEnabled;
			this.Fqdn = TaskCommon.ToStringOrNull(sc.Fqdn);
		}

		public SendConnector(string name, MultiValuedProperty<AddressSpace> addressSpaces, MultiValuedProperty<ADObjectId> transportServers, string tlsDomain, SmtpX509Identifier tlsCertificateName, bool requireTLS, string fqdn)
		{
			this.Name = name;
			this.AddressSpaces = addressSpaces;
			this.SourceTransportServers = transportServers;
			this.DNSRoutingEnabled = true;
			this.SmartHosts = null;
			this.RequireTLS = requireTLS;
			this.TlsAuthLevel = (requireTLS ? new TlsAuthLevel?(Microsoft.Exchange.Data.TlsAuthLevel.DomainValidation) : null);
			this.TlsDomain = (requireTLS ? tlsDomain : null);
			this.ErrorPolicies = ErrorPolicies.Default;
			this.TlsCertificateName = tlsCertificateName;
			this.CloudServicesMailEnabled = true;
			this.Fqdn = fqdn;
		}

		public ADObjectId Identity { get; set; }

		public string Name { get; set; }

		public MultiValuedProperty<AddressSpace> AddressSpaces { get; set; }

		public MultiValuedProperty<ADObjectId> SourceTransportServers { get; set; }

		public bool DNSRoutingEnabled { get; set; }

		public MultiValuedProperty<SmartHost> SmartHosts { get; set; }

		public bool RequireTLS { get; set; }

		public TlsAuthLevel? TlsAuthLevel { get; set; }

		public string TlsDomain { get; set; }

		public ErrorPolicies ErrorPolicies { get; set; }

		public SmtpX509Identifier TlsCertificateName { get; set; }

		public string Fqdn { get; set; }

		public bool CloudServicesMailEnabled { get; set; }

		public override string ToString()
		{
			if (this.Identity != null)
			{
				return this.Identity.ToString();
			}
			return "<New>";
		}

		public bool Equals(ISendConnector obj)
		{
			return this.CloudServicesMailEnabled == obj.CloudServicesMailEnabled && string.Equals(this.Name, obj.Name, StringComparison.InvariantCultureIgnoreCase) && this.RequireTLS == obj.RequireTLS && this.DNSRoutingEnabled == obj.DNSRoutingEnabled && this.TlsAuthLevel == obj.TlsAuthLevel && this.ErrorPolicies == obj.ErrorPolicies && string.Equals(this.TlsDomain, obj.TlsDomain, StringComparison.InvariantCultureIgnoreCase) && TaskCommon.AreEqual(this.TlsCertificateName, obj.TlsCertificateName) && TaskCommon.ContainsSame<AddressSpace>(this.AddressSpaces, obj.AddressSpaces) && TaskCommon.ContainsSame<SmartHost>(this.SmartHosts, obj.SmartHosts) && TaskCommon.ContainsSame<ADObjectId>(this.SourceTransportServers, obj.SourceTransportServers) && string.Equals(this.Fqdn, obj.Fqdn, StringComparison.InvariantCultureIgnoreCase);
		}

		public ISendConnector Clone(ADObjectId identity)
		{
			SendConnector sendConnector = new SendConnector();
			sendConnector.UpdateFrom(this);
			sendConnector.Identity = identity;
			return sendConnector;
		}

		public void UpdateFrom(ISendConnector obj)
		{
			this.Name = obj.Name;
			this.AddressSpaces = obj.AddressSpaces;
			this.SourceTransportServers = obj.SourceTransportServers;
			this.DNSRoutingEnabled = obj.DNSRoutingEnabled;
			this.SmartHosts = obj.SmartHosts;
			this.RequireTLS = obj.RequireTLS;
			this.TlsAuthLevel = obj.TlsAuthLevel;
			this.TlsDomain = obj.TlsDomain;
			this.ErrorPolicies = obj.ErrorPolicies;
			this.TlsCertificateName = obj.TlsCertificateName;
			this.CloudServicesMailEnabled = obj.CloudServicesMailEnabled;
			this.Fqdn = obj.Fqdn;
		}
	}
}
