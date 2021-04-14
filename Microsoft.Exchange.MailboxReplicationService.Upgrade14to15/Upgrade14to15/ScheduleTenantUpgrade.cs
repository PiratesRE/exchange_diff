using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "ScheduleTenantUpgrade", Namespace = "http://tempuri.org/")]
	[DebuggerStepThrough]
	public class ScheduleTenantUpgrade : IExtensibleDataObject
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
		public DateTime upgradeDate
		{
			get
			{
				return this.upgradeDateField;
			}
			set
			{
				this.upgradeDateField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private Guid tenantIdField;

		private DateTime upgradeDateField;
	}
}
