using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "SetServicePrincipalBySpnRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	public class SetServicePrincipalBySpnRequest : Request
	{
		[DataMember]
		public ServicePrincipal ServicePrincipal
		{
			get
			{
				return this.ServicePrincipalField;
			}
			set
			{
				this.ServicePrincipalField = value;
			}
		}

		private ServicePrincipal ServicePrincipalField;
	}
}
