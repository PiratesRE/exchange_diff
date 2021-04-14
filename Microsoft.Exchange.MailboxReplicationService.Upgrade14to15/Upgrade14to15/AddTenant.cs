using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "AddTenant", Namespace = "http://tempuri.org/")]
	public class AddTenant : IExtensibleDataObject
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
		public Tenant tenant
		{
			get
			{
				return this.tenantField;
			}
			set
			{
				this.tenantField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private Tenant tenantField;
	}
}
