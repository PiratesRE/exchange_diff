using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services
{
	[DataContract(Name = "DomainResponseInfo", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	[DebuggerStepThrough]
	[Serializable]
	internal class DomainResponseInfo : ResponseInfo
	{
		[DataMember]
		internal Guid? DomainGuid
		{
			get
			{
				return this.DomainGuidField;
			}
			set
			{
				this.DomainGuidField = value;
			}
		}

		[DataMember]
		internal string DomainName
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
		internal ProvisioningSource SourceId
		{
			get
			{
				return this.SourceIdField;
			}
			set
			{
				this.SourceIdField = value;
			}
		}

		[OptionalField]
		private Guid? DomainGuidField;

		[OptionalField]
		private string DomainNameField;

		[OptionalField]
		private ProvisioningSource SourceIdField;
	}
}
