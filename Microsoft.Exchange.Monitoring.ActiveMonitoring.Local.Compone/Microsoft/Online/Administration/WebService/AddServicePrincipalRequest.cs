using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DataContract(Name = "AddServicePrincipalRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class AddServicePrincipalRequest : Request
	{
		[DataMember]
		public bool? AccountEnabled
		{
			get
			{
				return this.AccountEnabledField;
			}
			set
			{
				this.AccountEnabledField = value;
			}
		}

		[DataMember]
		public Guid? AppPrincipalId
		{
			get
			{
				return this.AppPrincipalIdField;
			}
			set
			{
				this.AppPrincipalIdField = value;
			}
		}

		[DataMember]
		public ServicePrincipalCredential[] Credentials
		{
			get
			{
				return this.CredentialsField;
			}
			set
			{
				this.CredentialsField = value;
			}
		}

		[DataMember]
		public string DisplayName
		{
			get
			{
				return this.DisplayNameField;
			}
			set
			{
				this.DisplayNameField = value;
			}
		}

		[DataMember]
		public string[] ServicePrincipalNames
		{
			get
			{
				return this.ServicePrincipalNamesField;
			}
			set
			{
				this.ServicePrincipalNamesField = value;
			}
		}

		[DataMember]
		public bool? TrustedForDelegation
		{
			get
			{
				return this.TrustedForDelegationField;
			}
			set
			{
				this.TrustedForDelegationField = value;
			}
		}

		private bool? AccountEnabledField;

		private Guid? AppPrincipalIdField;

		private ServicePrincipalCredential[] CredentialsField;

		private string DisplayNameField;

		private string[] ServicePrincipalNamesField;

		private bool? TrustedForDelegationField;
	}
}
