using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "UpdateWorkItemScheduledDate", Namespace = "http://tempuri.org/")]
	public class UpdateWorkItemScheduledDate : IExtensibleDataObject
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
		public string workItemId
		{
			get
			{
				return this.workItemIdField;
			}
			set
			{
				this.workItemIdField = value;
			}
		}

		[DataMember(Order = 1)]
		public DateTime scheduledDate
		{
			get
			{
				return this.scheduledDateField;
			}
			set
			{
				this.scheduledDateField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string workItemIdField;

		private DateTime scheduledDateField;
	}
}
