using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "TenantInfo", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.WorkloadService")]
	[DebuggerStepThrough]
	public class TenantInfo : IExtensibleDataObject
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
		public string InitialDomain
		{
			get
			{
				return this.InitialDomainField;
			}
			set
			{
				this.InitialDomainField = value;
			}
		}

		[DataMember]
		public string PrimaryDomain
		{
			get
			{
				return this.PrimaryDomainField;
			}
			set
			{
				this.PrimaryDomainField = value;
			}
		}

		[DataMember]
		public DateTime? ScheduledUpgradeDate
		{
			get
			{
				return this.ScheduledUpgradeDateField;
			}
			set
			{
				this.ScheduledUpgradeDateField = value;
			}
		}

		[DataMember]
		public Guid TenantId
		{
			get
			{
				return this.TenantIdField;
			}
			set
			{
				this.TenantIdField = value;
			}
		}

		[DataMember]
		public string Tier
		{
			get
			{
				return this.TierField;
			}
			set
			{
				this.TierField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string InitialDomainField;

		private string PrimaryDomainField;

		private DateTime? ScheduledUpgradeDateField;

		private Guid TenantIdField;

		private string TierField;
	}
}
