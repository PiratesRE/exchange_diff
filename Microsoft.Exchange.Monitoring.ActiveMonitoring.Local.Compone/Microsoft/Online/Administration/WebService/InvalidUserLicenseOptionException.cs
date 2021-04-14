using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DebuggerStepThrough]
	[DataContract(Name = "InvalidUserLicenseOptionException", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class InvalidUserLicenseOptionException : InvalidParameterException
	{
		[DataMember]
		public string AccountName
		{
			get
			{
				return this.AccountNameField;
			}
			set
			{
				this.AccountNameField = value;
			}
		}

		[DataMember]
		public string ServicePlanName
		{
			get
			{
				return this.ServicePlanNameField;
			}
			set
			{
				this.ServicePlanNameField = value;
			}
		}

		[DataMember]
		public string SkuPartNumber
		{
			get
			{
				return this.SkuPartNumberField;
			}
			set
			{
				this.SkuPartNumberField = value;
			}
		}

		private string AccountNameField;

		private string ServicePlanNameField;

		private string SkuPartNumberField;
	}
}
