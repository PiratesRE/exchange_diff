using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "SetUserLicensesRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	public class SetUserLicensesRequest : Request
	{
		[DataMember]
		public AccountSkuIdentifier[] AddLicenses
		{
			get
			{
				return this.AddLicensesField;
			}
			set
			{
				this.AddLicensesField = value;
			}
		}

		[DataMember]
		public LicenseOption[] LicenseOptions
		{
			get
			{
				return this.LicenseOptionsField;
			}
			set
			{
				this.LicenseOptionsField = value;
			}
		}

		[DataMember]
		public Guid ObjectId
		{
			get
			{
				return this.ObjectIdField;
			}
			set
			{
				this.ObjectIdField = value;
			}
		}

		[DataMember]
		public AccountSkuIdentifier[] RemoveLicenses
		{
			get
			{
				return this.RemoveLicensesField;
			}
			set
			{
				this.RemoveLicensesField = value;
			}
		}

		private AccountSkuIdentifier[] AddLicensesField;

		private LicenseOption[] LicenseOptionsField;

		private Guid ObjectIdField;

		private AccountSkuIdentifier[] RemoveLicensesField;
	}
}
