using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services
{
	[DataContract(Name = "CompanyResponseInfo", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	[Serializable]
	internal class CompanyResponseInfo : ResponseInfo
	{
		[DataMember]
		internal Guid? CompanyGuid
		{
			get
			{
				return this.CompanyGuidField;
			}
			set
			{
				this.CompanyGuidField = value;
			}
		}

		[DataMember]
		internal int CompanyId
		{
			get
			{
				return this.CompanyIdField;
			}
			set
			{
				this.CompanyIdField = value;
			}
		}

		[DataMember]
		internal string CompanyName
		{
			get
			{
				return this.CompanyNameField;
			}
			set
			{
				this.CompanyNameField = value;
			}
		}

		[OptionalField]
		private Guid? CompanyGuidField;

		[OptionalField]
		private int CompanyIdField;

		[OptionalField]
		private string CompanyNameField;
	}
}
