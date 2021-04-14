using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.BOX.Shell
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "UserInfo", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.Shell")]
	[DebuggerStepThrough]
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

		[DataMember]
		public string BecContextToken
		{
			get
			{
				return this.BecContextTokenField;
			}
			set
			{
				this.BecContextTokenField = value;
			}
		}

		[DataMember]
		public string Puid
		{
			get
			{
				return this.PuidField;
			}
			set
			{
				this.PuidField = value;
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

		private ExtensionDataObject extensionDataField;

		private string BecContextTokenField;

		private string PuidField;

		private string UserPrincipalNameField;
	}
}
