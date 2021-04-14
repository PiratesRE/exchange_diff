using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "GetSchedulerTenantResponse", Namespace = "http://tempuri.org/")]
	public class GetSchedulerTenantResponse : IExtensibleDataObject
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
		public SchedulerTenant GetSchedulerTenantResult
		{
			get
			{
				return this.GetSchedulerTenantResultField;
			}
			set
			{
				this.GetSchedulerTenantResultField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private SchedulerTenant GetSchedulerTenantResultField;
	}
}
