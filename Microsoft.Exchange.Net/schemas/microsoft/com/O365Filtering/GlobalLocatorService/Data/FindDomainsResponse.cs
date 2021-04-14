using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "FindDomainsResponse", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	public class FindDomainsResponse : ResponseBase
	{
		[DataMember]
		public FindDomainResponse[] DomainsResponse
		{
			get
			{
				return this.DomainsResponseField;
			}
			set
			{
				this.DomainsResponseField = value;
			}
		}

		private FindDomainResponse[] DomainsResponseField;
	}
}
