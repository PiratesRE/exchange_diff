using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DataContract(Name = "ReschedulePartnerTenant", Namespace = "http://tempuri.org/")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	public class ReschedulePartnerTenant : IExtensibleDataObject
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
		public DateTime upgradeStartDate
		{
			get
			{
				return this.upgradeStartDateField;
			}
			set
			{
				this.upgradeStartDateField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private Guid tenantIdField;

		private DateTime upgradeStartDateField;
	}
}
