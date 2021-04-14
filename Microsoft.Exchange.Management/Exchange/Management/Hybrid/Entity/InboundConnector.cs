using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Hybrid.Entity
{
	internal class InboundConnector : IInboundConnector, IEntity<IInboundConnector>
	{
		public InboundConnector()
		{
		}

		public InboundConnector(TenantInboundConnector ic)
		{
			this.Identity = (ADObjectId)ic.Identity;
			this.Name = ic.Name;
			this.TLSSenderCertificateName = TaskCommon.ToStringOrNull(ic.TlsSenderCertificateName);
			this.ConnectorType = ic.ConnectorType;
			this.ConnectorSource = ic.ConnectorSource;
			this.SenderDomains = ic.SenderDomains;
			this.RequireTls = ic.RequireTls;
			this.CloudServicesMailEnabled = ic.CloudServicesMailEnabled;
		}

		public InboundConnector(string name, SmtpX509Identifier tlsSenderCertificateName)
		{
			MultiValuedProperty<AddressSpace> multiValuedProperty = new MultiValuedProperty<AddressSpace>();
			multiValuedProperty.Add(new AddressSpace("*"));
			this.Name = name;
			this.TLSSenderCertificateName = TaskCommon.ToStringOrNull(tlsSenderCertificateName);
			this.ConnectorType = TenantConnectorType.OnPremises;
			this.SenderDomains = multiValuedProperty;
			this.RequireTls = true;
			this.CloudServicesMailEnabled = true;
		}

		public ADObjectId Identity { get; set; }

		public string Name { get; set; }

		public string TLSSenderCertificateName { get; set; }

		public TenantConnectorType ConnectorType { get; set; }

		public TenantConnectorSource ConnectorSource { get; set; }

		public MultiValuedProperty<AddressSpace> SenderDomains { get; set; }

		public bool RequireTls { get; set; }

		public bool CloudServicesMailEnabled { get; set; }

		public override string ToString()
		{
			if (this.Identity != null)
			{
				return this.Identity.ToString();
			}
			return "<New>";
		}

		public bool Equals(IInboundConnector obj)
		{
			return this.RequireTls == obj.RequireTls && this.ConnectorType == obj.ConnectorType && this.CloudServicesMailEnabled == obj.CloudServicesMailEnabled && string.Equals(this.Name, obj.Name, StringComparison.InvariantCultureIgnoreCase) && string.Equals(this.TLSSenderCertificateName, obj.TLSSenderCertificateName, StringComparison.InvariantCultureIgnoreCase) && TaskCommon.ContainsSame<AddressSpace>(this.SenderDomains, obj.SenderDomains);
		}

		public IInboundConnector Clone(ADObjectId identity)
		{
			InboundConnector inboundConnector = new InboundConnector();
			inboundConnector.UpdateFrom(this);
			inboundConnector.Identity = identity;
			return inboundConnector;
		}

		public void UpdateFrom(IInboundConnector obj)
		{
			this.Name = obj.Name;
			this.TLSSenderCertificateName = obj.TLSSenderCertificateName;
			this.ConnectorType = obj.ConnectorType;
			this.ConnectorSource = obj.ConnectorSource;
			this.SenderDomains = obj.SenderDomains;
			this.RequireTls = obj.RequireTls;
			this.CloudServicesMailEnabled = obj.CloudServicesMailEnabled;
		}
	}
}
