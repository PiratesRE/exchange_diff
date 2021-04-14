using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services
{
	[DebuggerStepThrough]
	[DataContract(Name = "CompanyResponseInfoSet", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	[Serializable]
	internal class CompanyResponseInfoSet : ResponseInfoSet
	{
		[DataMember]
		internal CompanyResponseInfo[] ResponseInfo
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
		private CompanyResponseInfo[] ResponseInfoField;
	}
}
