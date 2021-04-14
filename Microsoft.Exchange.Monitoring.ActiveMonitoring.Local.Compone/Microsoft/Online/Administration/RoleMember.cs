using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[DataContract(Name = "RoleMember", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class RoleMember : IExtensibleDataObject
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
		public string EmailAddress
		{
			get
			{
				return this.EmailAddressField;
			}
			set
			{
				this.EmailAddressField = value;
			}
		}

		[DataMember]
		public bool? IsLicensed
		{
			get
			{
				return this.IsLicensedField;
			}
			set
			{
				this.IsLicensedField = value;
			}
		}

		[DataMember]
		public DateTime? LastDirSyncTime
		{
			get
			{
				return this.LastDirSyncTimeField;
			}
			set
			{
				this.LastDirSyncTimeField = value;
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
		public ProvisioningStatus? OverallProvisioningStatus
		{
			get
			{
				return this.OverallProvisioningStatusField;
			}
			set
			{
				this.OverallProvisioningStatusField = value;
			}
		}

		[DataMember]
		public RoleMemberType RoleMemberType
		{
			get
			{
				return this.RoleMemberTypeField;
			}
			set
			{
				this.RoleMemberTypeField = value;
			}
		}

		[DataMember]
		public ValidationStatus? ValidationStatus
		{
			get
			{
				return this.ValidationStatusField;
			}
			set
			{
				this.ValidationStatusField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string DisplayNameField;

		private string EmailAddressField;

		private bool? IsLicensedField;

		private DateTime? LastDirSyncTimeField;

		private Guid? ObjectIdField;

		private ProvisioningStatus? OverallProvisioningStatusField;

		private RoleMemberType RoleMemberTypeField;

		private ValidationStatus? ValidationStatusField;
	}
}
