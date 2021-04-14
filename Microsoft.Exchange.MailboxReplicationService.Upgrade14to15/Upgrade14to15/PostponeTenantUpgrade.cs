using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "PostponeTenantUpgrade", Namespace = "http://tempuri.org/")]
	public class PostponeTenantUpgrade : IExtensibleDataObject
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
		public Guid tenantId
		{
			get
			{
				return this.tenantIdField;
			}
			set
			{
				this.tenantIdField = value;
			}
		}

		[DataMember]
		public string userUpn
		{
			get
			{
				return this.userUpnField;
			}
			set
			{
				this.userUpnField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private Guid tenantIdField;

		private string userUpnField;
	}
}
