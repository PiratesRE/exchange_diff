using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "SchedulePartnerTenant", Namespace = "http://tempuri.org/")]
	[DebuggerStepThrough]
	public class SchedulePartnerTenant : IExtensibleDataObject
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
		public Tenant tenant
		{
			get
			{
				return this.tenantField;
			}
			set
			{
				this.tenantField = value;
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

		[DataMember(Order = 2)]
		public Tuple<string[], string>[] redirectWorkLoads
		{
			get
			{
				return this.redirectWorkLoadsField;
			}
			set
			{
				this.redirectWorkLoadsField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private Tenant tenantField;

		private DateTime upgradeStartDateField;

		private Tuple<string[], string>[] redirectWorkLoadsField;
	}
}
