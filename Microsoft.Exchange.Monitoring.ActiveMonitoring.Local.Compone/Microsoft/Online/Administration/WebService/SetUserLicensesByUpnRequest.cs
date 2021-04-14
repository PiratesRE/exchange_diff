using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DataContract(Name = "SetUserLicensesByUpnRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	public class SetUserLicensesByUpnRequest : Request
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

		[DataMember]
		public string UserPrincipalName
		{
			get
			{
				return this.UserPrincipalNameField;
			}
			set
			{
				this.UserPrincipalNameField = value;
			}
		}

		private AccountSkuIdentifier[] AddLicensesField;

		private LicenseOption[] LicenseOptionsField;

		private AccountSkuIdentifier[] RemoveLicensesField;

		private string UserPrincipalNameField;
	}
}
