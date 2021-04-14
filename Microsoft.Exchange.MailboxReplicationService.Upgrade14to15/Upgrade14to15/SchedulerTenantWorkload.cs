using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "SchedulerTenantWorkload", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.ManagementService")]
	public class SchedulerTenantWorkload : IExtensibleDataObject
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
		public int? ConsumedUnits
		{
			get
			{
				return this.ConsumedUnitsField;
			}
			set
			{
				this.ConsumedUnitsField = value;
			}
		}

		[DataMember]
		public DateTime? ExpirationDate
		{
			get
			{
				return this.ExpirationDateField;
			}
			set
			{
				this.ExpirationDateField = value;
			}
		}

		[DataMember]
		public string WorkloadName
		{
			get
			{
				return this.WorkloadNameField;
			}
			set
			{
				this.WorkloadNameField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private int? ConsumedUnitsField;

		private DateTime? ExpirationDateField;

		private string WorkloadNameField;
	}
}
