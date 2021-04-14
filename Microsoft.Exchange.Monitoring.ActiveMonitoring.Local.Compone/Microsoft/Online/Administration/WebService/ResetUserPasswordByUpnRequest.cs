using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "ResetUserPasswordByUpnRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	public class ResetUserPasswordByUpnRequest : Request
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
		public string NewPassword
		{
			get
			{
				return this.NewPasswordField;
			}
			set
			{
				this.NewPasswordField = value;
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

		private bool? ForceChangePasswordField;

		private string NewPasswordField;

		private string UserPrincipalNameField;
	}
}
