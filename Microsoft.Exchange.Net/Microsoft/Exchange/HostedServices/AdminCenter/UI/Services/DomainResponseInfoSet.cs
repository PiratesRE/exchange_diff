using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services
{
	[DataContract(Name = "DomainResponseInfoSet", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	[Serializable]
	internal class DomainResponseInfoSet : ResponseInfoSet
	{
		[DataMember]
		internal DomainResponseInfo[] ResponseInfo
		{
			get
			{
				return this.ResponseInfoField;
			}
			set
			{
				this.ResponseInfoField = value;
			}
		}

		[OptionalField]
		private DomainResponseInfo[] ResponseInfoField;
	}
}
