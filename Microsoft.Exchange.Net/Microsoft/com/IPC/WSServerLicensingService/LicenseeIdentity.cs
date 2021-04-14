using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.com.IPC.WSServerLicensingService
{
	[DataContract(Name = "LicenseeIdentity", Namespace = "http://microsoft.com/IPC/WSServerLicensingService")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	public class LicenseeIdentity : IExtensibleDataObject
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
		public string Email
		{
			get
			{
				return this.EmailField;
			}
			set
			{
				this.EmailField = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string[] ProxyAddresses
		{
			get
			{
				return this.ProxyAddressesField;
			}
			set
			{
				this.ProxyAddressesField = value;
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public string[] GroupMemberships
		{
			get
			{
				return this.GroupMembershipsField;
			}
			set
			{
				this.GroupMembershipsField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string EmailField;

		private string[] ProxyAddressesField;

		private string[] GroupMembershipsField;
	}
}
