using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DataContract(Name = "Group", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.Common.DataContract")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	public class Group : IExtensibleDataObject
	{
		public Group(string groupName, DataCenterRegion regionName)
		{
			this.GroupName = groupName;
			this.RegionName = regionName;
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
		public DataCenterRegion RegionName
		{
			get
			{
				return this.RegionNameField;
			}
			set
			{
				this.RegionNameField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string GroupNameField;

		private DataCenterRegion RegionNameField;
	}
}
