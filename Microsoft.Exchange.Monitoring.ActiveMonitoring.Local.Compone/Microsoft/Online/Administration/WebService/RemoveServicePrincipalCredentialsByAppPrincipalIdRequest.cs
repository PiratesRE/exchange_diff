using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DataContract(Name = "RemoveServicePrincipalCredentialsByAppPrincipalIdRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	public class RemoveServicePrincipalCredentialsByAppPrincipalIdRequest : Request
	{
		[DataMember]
		public Guid AppPrincipalId
		{
			get
			{
				return this.AppPrincipalIdField;
			}
			set
			{
				this.AppPrincipalIdField = value;
			}
		}

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

		private Guid AppPrincipalIdField;

		private Guid[] KeyIdsField;
	}
}
