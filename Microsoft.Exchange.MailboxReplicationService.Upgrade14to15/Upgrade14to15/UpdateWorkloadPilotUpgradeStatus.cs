using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "UpdateWorkloadPilotUpgradeStatus", Namespace = "http://tempuri.org/")]
	[DebuggerStepThrough]
	public class UpdateWorkloadPilotUpgradeStatus : IExtensibleDataObject
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
		public Guid tenantId
		{
			get
			{
				return this.tenantIdField;
			}
			set
			{
				this.tenantIdField = value;
			}
		}

		[DataMember]
		public string workloadName
		{
			get
			{
				return this.workloadNameField;
			}
			set
			{
				this.workloadNameField = value;
			}
		}

		[DataMember(Order = 2)]
		public string pilotUserUpn
		{
			get
			{
				return this.pilotUserUpnField;
			}
			set
			{
				this.pilotUserUpnField = value;
			}
		}

		[DataMember(Order = 3)]
		public WorkItemStatus status
		{
			get
			{
				return this.statusField;
			}
			set
			{
				this.statusField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private Guid tenantIdField;

		private string workloadNameField;

		private string pilotUserUpnField;

		private WorkItemStatus statusField;
	}
}
