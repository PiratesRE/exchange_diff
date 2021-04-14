using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DebuggerStepThrough]
	[DataContract(Name = "RestoreUserByUpnRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class RestoreUserByUpnRequest : Request
	{
		[DataMember]
		public bool? AutoReconcileProxyConflicts
		{
			get
			{
				return this.AutoReconcileProxyConflictsField;
			}
			set
			{
				this.AutoReconcileProxyConflictsField = value;
			}
		}

		[DataMember]
		public string NewUserPrincipalName
		{
			get
			{
				return this.NewUserPrincipalNameField;
			}
			set
			{
				this.NewUserPrincipalNameField = value;
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

		private bool? AutoReconcileProxyConflictsField;

		private string NewUserPrincipalNameField;

		private string UserPrincipalNameField;
	}
}
