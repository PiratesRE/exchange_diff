using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "CapacityBlock", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.Common.DataContract")]
	public class CapacityBlock : IExtensibleDataObject
	{
		public CapacityBlock(DateTime startDate, int upgradeUnits)
		{
			this.StartDate = startDate;
			this.UpgradeUnits = upgradeUnits;
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
		public DateTime StartDate
		{
			get
			{
				return this.StartDateField;
			}
			set
			{
				this.StartDateField = value;
			}
		}

		[DataMember]
		public int UpgradeUnits
		{
			get
			{
				return this.UpgradeUnitsField;
			}
			set
			{
				this.UpgradeUnitsField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private DateTime StartDateField;

		private int UpgradeUnitsField;
	}
}
