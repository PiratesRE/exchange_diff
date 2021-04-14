using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DebuggerStepThrough]
	[DataContract(Name = "GroupBlackout", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.Common.DataContract")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class GroupBlackout : IExtensibleDataObject
	{
		public GroupBlackout(string groupName, BlackoutInterval[] intervals)
		{
			this.GroupName = groupName;
			this.Intervals = intervals;
		}

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
		public string GroupName
		{
			get
			{
				return this.GroupNameField;
			}
			set
			{
				this.GroupNameField = value;
			}
		}

		[DataMember]
		public BlackoutInterval[] Intervals
		{
			get
			{
				return this.IntervalsField;
			}
			set
			{
				this.IntervalsField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string GroupNameField;

		private BlackoutInterval[] IntervalsField;
	}
}
