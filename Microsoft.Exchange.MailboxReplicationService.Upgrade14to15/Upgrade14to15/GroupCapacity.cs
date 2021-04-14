using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "GroupCapacity", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.Common.DataContract")]
	[DebuggerStepThrough]
	public class GroupCapacity : IExtensibleDataObject
	{
		public GroupCapacity(string groupName, CapacityBlock[] capacities)
		{
			this.GroupName = groupName;
			this.Capacities = capacities;
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
		public CapacityBlock[] Capacities
		{
			get
			{
				return this.CapacitiesField;
			}
			set
			{
				this.CapacitiesField = value;
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

		private ExtensionDataObject extensionDataField;

		private CapacityBlock[] CapacitiesField;

		private string GroupNameField;
	}
}
