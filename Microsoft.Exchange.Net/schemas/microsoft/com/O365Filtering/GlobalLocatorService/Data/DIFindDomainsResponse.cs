using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "DIFindDomainsResponse", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	public class DIFindDomainsResponse : DIResponseBase
	{
		[DataMember]
		public DIDomainInfo[] DIDomainInformationList
		{
			get
			{
				return this.DIDomainInformationListField;
			}
			set
			{
				this.DIDomainInformationListField = value;
			}
		}

		[DataMember]
		public DITenantInfo DITenantInformation
		{
			get
			{
				return this.DITenantInformationField;
			}
			set
			{
				this.DITenantInformationField = value;
			}
		}

		private DIDomainInfo[] DIDomainInformationListField;

		private DITenantInfo DITenantInformationField;
	}
}
