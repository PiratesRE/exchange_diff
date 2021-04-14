using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "SaveUserRequest", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	public class SaveUserRequest : IExtensibleDataObject
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
		public string CustomTag
		{
			get
			{
				return this.CustomTagField;
			}
			set
			{
				this.CustomTagField = value;
			}
		}

		[DataMember(IsRequired = true)]
		public Guid TenantId
		{
			get
			{
				return this.TenantIdField;
			}
			set
			{
				this.TenantIdField = value;
			}
		}

		[DataMember(IsRequired = true)]
		public UserInfo UserInfo
		{
			get
			{
				return this.UserInfoField;
			}
			set
			{
				this.UserInfoField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string CustomTagField;

		private Guid TenantIdField;

		private UserInfo UserInfoField;
	}
}
