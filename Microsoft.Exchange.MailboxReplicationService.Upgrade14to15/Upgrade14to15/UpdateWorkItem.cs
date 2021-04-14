using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "UpdateWorkItem", Namespace = "http://tempuri.org/")]
	[DebuggerStepThrough]
	public class UpdateWorkItem : IExtensibleDataObject
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
		public WorkItem workItem
		{
			get
			{
				return this.workItemField;
			}
			set
			{
				this.workItemField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private WorkItem workItemField;
	}
}
