using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "AddSchedulerTenant", Namespace = "http://tempuri.org/")]
	public class AddSchedulerTenant : IExtensibleDataObject
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
		public SchedulerTenant tenant
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

		private SchedulerTenant tenantField;
	}
}
