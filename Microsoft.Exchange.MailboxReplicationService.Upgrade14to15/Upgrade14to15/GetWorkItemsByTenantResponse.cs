using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DataContract(Name = "GetWorkItemsByTenantResponse", Namespace = "http://tempuri.org/")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class GetWorkItemsByTenantResponse : IExtensibleDataObject
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
		public WorkItem[] GetWorkItemsByTenantResult
		{
			get
			{
				return this.GetWorkItemsByTenantResultField;
			}
			set
			{
				this.GetWorkItemsByTenantResultField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private WorkItem[] GetWorkItemsByTenantResultField;
	}
}
