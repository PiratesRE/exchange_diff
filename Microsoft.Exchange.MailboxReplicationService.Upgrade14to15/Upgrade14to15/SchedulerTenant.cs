using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "SchedulerTenant", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.ManagementService")]
	public class SchedulerTenant : IExtensibleDataObject
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
		public DateTime? CanceledDate
		{
			get
			{
				return this.CanceledDateField;
			}
			set
			{
				this.CanceledDateField = value;
			}
		}

		[DataMember]
		public string CommunicationCulture
		{
			get
			{
				return this.CommunicationCultureField;
			}
			set
			{
				this.CommunicationCultureField = value;
			}
		}

		[DataMember]
		public DataSource DataSource
		{
			get
			{
				return this.DataSourceField;
			}
			set
			{
				this.DataSourceField = value;
			}
		}

		[DataMember]
		public bool HasPartner
		{
			get
			{
				return this.HasPartnerField;
			}
			set
			{
				this.HasPartnerField = value;
			}
		}

		[DataMember]
		public bool HasSyndicationPartner
		{
			get
			{
				return this.HasSyndicationPartnerField;
			}
			set
			{
				this.HasSyndicationPartnerField = value;
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
		public bool? IsAutoScheduled
		{
			get
			{
				return this.IsAutoScheduledField;
			}
			set
			{
				this.IsAutoScheduledField = value;
			}
		}

		[DataMember]
		public bool IsPTenant
		{
			get
			{
				return this.IsPTenantField;
			}
			set
			{
				this.IsPTenantField = value;
			}
		}

		[DataMember]
		public bool IsTestTenant
		{
			get
			{
				return this.IsTestTenantField;
			}
			set
			{
				this.IsTestTenantField = value;
			}
		}

		[DataMember]
		public string Name
		{
			get
			{
				return this.NameField;
			}
			set
			{
				this.NameField = value;
			}
		}

		[DataMember]
		public DateTime? PostponedDate
		{
			get
			{
				return this.PostponedDateField;
			}
			set
			{
				this.PostponedDateField = value;
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
		public SchedulerState SchedulerState
		{
			get
			{
				return this.SchedulerStateField;
			}
			set
			{
				this.SchedulerStateField = value;
			}
		}

		[DataMember]
		public TenantEmail[] TenantEmails
		{
			get
			{
				return this.TenantEmailsField;
			}
			set
			{
				this.TenantEmailsField = value;
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
		public SchedulerTenantWorkload[] TenantWorkloads
		{
			get
			{
				return this.TenantWorkloadsField;
			}
			set
			{
				this.TenantWorkloadsField = value;
			}
		}

		[DataMember]
		public string TierName
		{
			get
			{
				return this.TierNameField;
			}
			set
			{
				this.TierNameField = value;
			}
		}

		[DataMember]
		public DateTime? UpgradeEndDate
		{
			get
			{
				return this.UpgradeEndDateField;
			}
			set
			{
				this.UpgradeEndDateField = value;
			}
		}

		[DataMember]
		public DateTime? UpgradeStartDate
		{
			get
			{
				return this.UpgradeStartDateField;
			}
			set
			{
				this.UpgradeStartDateField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private DateTime? CanceledDateField;

		private string CommunicationCultureField;

		private DataSource DataSourceField;

		private bool HasPartnerField;

		private bool HasSyndicationPartnerField;

		private string InitialDomainField;

		private bool? IsAutoScheduledField;

		private bool IsPTenantField;

		private bool IsTestTenantField;

		private string NameField;

		private DateTime? PostponedDateField;

		private string PrimaryDomainField;

		private SchedulerState SchedulerStateField;

		private TenantEmail[] TenantEmailsField;

		private Guid TenantIdField;

		private SchedulerTenantWorkload[] TenantWorkloadsField;

		private string TierNameField;

		private DateTime? UpgradeEndDateField;

		private DateTime? UpgradeStartDateField;
	}
}
