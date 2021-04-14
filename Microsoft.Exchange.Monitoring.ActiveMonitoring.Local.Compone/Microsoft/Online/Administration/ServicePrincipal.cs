using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "ServicePrincipal", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	[KnownType(typeof(ServicePrincipalExtended))]
	public class ServicePrincipal : IExtensibleDataObject
	{
		public ExtensionDataObject ExtensionData
		{
			get
			{
				return this.extensionDataField;
			}
			set
			{
				this.extensionDataField = value;
			}
		}

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
		public Guid? ObjectId
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

		private ExtensionDataObject extensionDataField;

		private bool? AccountEnabledField;

		private Guid? AppPrincipalIdField;

		private string DisplayNameField;

		private Guid? ObjectIdField;

		private string[] ServicePrincipalNamesField;

		private bool? TrustedForDelegationField;
	}
}
