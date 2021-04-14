using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "UserInfo", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	public class UserInfo : IExtensibleDataObject
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

		[DataMember(IsRequired = true)]
		public string MSAUserName
		{
			get
			{
				return this.MSAUserNameField;
			}
			set
			{
				this.MSAUserNameField = value;
			}
		}

		[DataMember(IsRequired = true)]
		public string UserKey
		{
			get
			{
				return this.UserKeyField;
			}
			set
			{
				this.UserKeyField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string MSAUserNameField;

		private string UserKeyField;
	}
}
