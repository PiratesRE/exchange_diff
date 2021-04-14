using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DataContract(Name = "RemoveServicePrincipalCredentialsBySpnRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	public class RemoveServicePrincipalCredentialsBySpnRequest : Request
	{
		[DataMember]
		public Guid[] KeyIds
		{
			get
			{
				return this.KeyIdsField;
			}
			set
			{
				this.KeyIdsField = value;
			}
		}

		[DataMember]
		public string ServicePrincipalName
		{
			get
			{
				return this.ServicePrincipalNameField;
			}
			set
			{
				this.ServicePrincipalNameField = value;
			}
		}

		private Guid[] KeyIdsField;

		private string ServicePrincipalNameField;
	}
}
