using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DebuggerStepThrough]
	[DataContract(Name = "GetWorkItemResponse", Namespace = "http://tempuri.org/")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class GetWorkItemResponse : IExtensibleDataObject
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
		public WorkItem GetWorkItemResult
		{
			get
			{
				return this.GetWorkItemResultField;
			}
			set
			{
				this.GetWorkItemResultField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private WorkItem GetWorkItemResultField;
	}
}
