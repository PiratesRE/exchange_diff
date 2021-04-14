using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data
{
	[DataContract(Name = "FindDomainResponse", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class FindDomainResponse : ResponseBase
	{
		[DataMember]
		public DomainInfo DomainInfo
		{
			get
			{
				return this.DomainInfoField;
			}
			set
			{
				this.DomainInfoField = value;
			}
		}

		[DataMember]
		public TenantInfo TenantInfo
		{
			get
			{
				return this.TenantInfoField;
			}
			set
			{
				this.TenantInfoField = value;
			}
		}

		private DomainInfo DomainInfoField;

		private TenantInfo TenantInfoField;
	}
}
