using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DataContract(Name = "LicenseQuotaExceededException", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class LicenseQuotaExceededException : InvalidUserLicenseException
	{
		[DataMember]
		public int? ConsumedLicenses
		{
			get
			{
				return this.ConsumedLicensesField;
			}
			set
			{
				this.ConsumedLicensesField = value;
			}
		}

		[DataMember]
		public int? TotalLicenses
		{
			get
			{
				return this.TotalLicensesField;
			}
			set
			{
				this.TotalLicensesField = value;
			}
		}

		private int? ConsumedLicensesField;

		private int? TotalLicensesField;
	}
}
