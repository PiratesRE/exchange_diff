using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[DataContract(Name = "PartnerContractSearchDefinition", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class PartnerContractSearchDefinition : SearchDefinition
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
		public Guid? ManagedTenantId
		{
			get
			{
				return this.ManagedTenantIdField;
			}
			set
			{
				this.ManagedTenantIdField = value;
			}
		}

		private string DomainNameField;

		private Guid? ManagedTenantIdField;
	}
}
