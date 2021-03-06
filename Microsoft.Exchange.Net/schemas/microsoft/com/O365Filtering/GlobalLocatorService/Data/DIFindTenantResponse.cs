using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data
{
	[DataContract(Name = "DIFindTenantResponse", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class DIFindTenantResponse : DIResponseBase
	{
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

		private DITenantInfo DITenantInformationField;
	}
}
