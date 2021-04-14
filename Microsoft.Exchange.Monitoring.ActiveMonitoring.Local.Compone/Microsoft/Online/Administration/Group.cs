using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "Group", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	public class Group : IExtensibleDataObject
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
		public string CommonName
		{
			get
			{
				return this.CommonNameField;
			}
			set
			{
				this.CommonNameField = value;
			}
		}

		[DataMember]
		public string Description
		{
			get
			{
				return this.DescriptionField;
			}
			set
			{
				this.DescriptionField = value;
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
		public ValidationError[] Errors
		{
			get
			{
				return this.ErrorsField;
			}
			set
			{
				this.ErrorsField = value;
			}
		}

		[DataMember]
		public GroupType? GroupType
		{
			get
			{
				return this.GroupTypeField;
			}
			set
			{
				this.GroupTypeField = value;
			}
		}

		[DataMember]
		public bool? IsSystem
		{
			get
			{
				return this.IsSystemField;
			}
			set
			{
				this.IsSystemField = value;
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
		public string ManagedBy
		{
			get
			{
				return this.ManagedByField;
			}
			set
			{
				this.ManagedByField = value;
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

		private string CommonNameField;

		private string DescriptionField;

		private string DisplayNameField;

		private string EmailAddressField;

		private ValidationError[] ErrorsField;

		private GroupType? GroupTypeField;

		private bool? IsSystemField;

		private DateTime? LastDirSyncTimeField;

		private string ManagedByField;

		private Guid? ObjectIdField;

		private ValidationStatus? ValidationStatusField;
	}
}
