using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DataContract(Name = "AddUserRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	public class AddUserRequest : Request
	{
		[DataMember]
		public bool? ForceChangePassword
		{
			get
			{
				return this.ForceChangePasswordField;
			}
			set
			{
				this.ForceChangePasswordField = value;
			}
		}

		[DataMember]
		public AccountSkuIdentifier[] LicenseAssignment
		{
			get
			{
				return this.LicenseAssignmentField;
			}
			set
			{
				this.LicenseAssignmentField = value;
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
		public string Password
		{
			get
			{
				return this.PasswordField;
			}
			set
			{
				this.PasswordField = value;
			}
		}

		[DataMember]
		public User User
		{
			get
			{
				return this.UserField;
			}
			set
			{
				this.UserField = value;
			}
		}

		private bool? ForceChangePasswordField;

		private AccountSkuIdentifier[] LicenseAssignmentField;

		private LicenseOption[] LicenseOptionsField;

		private string PasswordField;

		private User UserField;
	}
}
