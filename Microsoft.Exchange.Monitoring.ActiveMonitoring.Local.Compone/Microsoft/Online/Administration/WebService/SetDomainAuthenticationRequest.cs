using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DebuggerStepThrough]
	[DataContract(Name = "SetDomainAuthenticationRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class SetDomainAuthenticationRequest : Request
	{
		[DataMember]
		public DomainAuthenticationType Authentication
		{
			get
			{
				return this.AuthenticationField;
			}
			set
			{
				this.AuthenticationField = value;
			}
		}

		[DataMember]
		public string DomainName
		{
			get
			{
				return this.DomainNameField;
			}
			set
			{
				this.DomainNameField = value;
			}
		}

		[DataMember]
		public DomainFederationSettings FederationSettings
		{
			get
			{
				return this.FederationSettingsField;
			}
			set
			{
				this.FederationSettingsField = value;
			}
		}

		private DomainAuthenticationType AuthenticationField;

		private string DomainNameField;

		private DomainFederationSettings FederationSettingsField;
	}
}
