using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "VerifyDomainRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	public class VerifyDomainRequest : Request
	{
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

		private string DomainNameField;

		private DomainFederationSettings FederationSettingsField;
	}
}
