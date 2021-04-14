using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.RightsManagementServices.Online
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "TenantProvisioningMessage", Namespace = "http://microsoft.com/RightsManagementServiceOnline/2011/04")]
	public class TenantProvisioningMessage : IExtensibleDataObject
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
		public string TenantId
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

		[DataMember(Order = 1)]
		public string FriendlyName
		{
			get
			{
				return this.FriendlyNameField;
			}
			set
			{
				this.FriendlyNameField = value;
			}
		}

		[DataMember(Order = 2)]
		public TenantStatus Status
		{
			get
			{
				return this.StatusField;
			}
			set
			{
				this.StatusField = value;
			}
		}

		[DataMember(Order = 3)]
		public string InitialDomain
		{
			get
			{
				return this.InitialDomainField;
			}
			set
			{
				this.InitialDomainField = value;
			}
		}

		[DataMember(Order = 4)]
		public long Version
		{
			get
			{
				return this.VersionField;
			}
			set
			{
				this.VersionField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string TenantIdField;

		private string FriendlyNameField;

		private TenantStatus StatusField;

		private string InitialDomainField;

		private long VersionField;
	}
}
