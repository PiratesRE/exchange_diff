using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.EhfHybridMailflow
{
	[Serializable]
	public sealed class HybridMailflowConfiguration : ConfigurableObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return HybridMailflowConfiguration.schema;
			}
		}

		[Parameter]
		public List<SmtpDomainWithSubdomains> OutboundDomains
		{
			get
			{
				return this.myOutboundDomains;
			}
			set
			{
				this.myOutboundDomains = value;
			}
		}

		[Parameter]
		public List<IPRange> InboundIPs
		{
			get
			{
				return this.myInboundIPs;
			}
			set
			{
				this.myInboundIPs = value;
			}
		}

		[Parameter]
		public Fqdn OnPremisesFQDN
		{
			get
			{
				return this.myOnPremisesFQDN;
			}
			set
			{
				this.myOnPremisesFQDN = value;
			}
		}

		[Parameter]
		public string CertificateSubject
		{
			get
			{
				return this.myCertificateSubject;
			}
			set
			{
				this.myCertificateSubject = value;
			}
		}

		[Parameter]
		public bool? SecureMailEnabled
		{
			get
			{
				return this.mySecureMailEnabled;
			}
			set
			{
				this.mySecureMailEnabled = value;
			}
		}

		[Parameter]
		public bool? CentralizedTransportEnabled
		{
			get
			{
				return this.myCentralizedTransportEnabled;
			}
			set
			{
				this.myCentralizedTransportEnabled = value;
			}
		}

		internal HybridMailflowConfiguration() : base(new SimpleProviderPropertyBag())
		{
		}

		internal HybridMailflowConfiguration(List<SmtpDomainWithSubdomains> outboundDomains, List<IPRange> inboundIPs, Fqdn onPremisesFQDN, string certificateSubject, bool? secureMailEnabled, bool? centralizedTransportEnabled) : base(new SimpleProviderPropertyBag())
		{
			this.OutboundDomains = outboundDomains;
			this.InboundIPs = inboundIPs;
			this.OnPremisesFQDN = onPremisesFQDN;
			this.CertificateSubject = certificateSubject;
			this.SecureMailEnabled = secureMailEnabled;
			this.CentralizedTransportEnabled = centralizedTransportEnabled;
		}

		private const string MostDerivedClass = "msHybridMailflowEhfConfiguration";

		private static HybridMailflowConfigurationSchema schema = ObjectSchema.GetInstance<HybridMailflowConfigurationSchema>();

		private List<SmtpDomainWithSubdomains> myOutboundDomains;

		private List<IPRange> myInboundIPs;

		private Fqdn myOnPremisesFQDN;

		private string myCertificateSubject;

		private bool? mySecureMailEnabled;

		private bool? myCentralizedTransportEnabled;
	}
}
