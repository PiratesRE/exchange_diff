using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[DataContract(Name = "Tenant", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.ManagementService")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class Tenant : IExtensibleDataObject
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
		public int CanceledCount
		{
			get
			{
				return this.CanceledCountField;
			}
			set
			{
				this.CanceledCountField = value;
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
		public string CanceledReason
		{
			get
			{
				return this.CanceledReasonField;
			}
			set
			{
				this.CanceledReasonField = value;
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
		public bool HasPilotedUsers
		{
			get
			{
				return this.HasPilotedUsersField;
			}
			set
			{
				this.HasPilotedUsersField = value;
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
		public string PhaseName
		{
			get
			{
				return this.PhaseNameField;
			}
			set
			{
				this.PhaseNameField = value;
			}
		}

		[DataMember]
		public Status PhaseStatus
		{
			get
			{
				return this.PhaseStatusField;
			}
			set
			{
				this.PhaseStatusField = value;
			}
		}

		[DataMember]
		public DateTime? PilotEndDate
		{
			get
			{
				return this.PilotEndDateField;
			}
			set
			{
				this.PilotEndDateField = value;
			}
		}

		[DataMember]
		public DateTime? PilotStartDate
		{
			get
			{
				return this.PilotStartDateField;
			}
			set
			{
				this.PilotStartDateField = value;
			}
		}

		[DataMember]
		public DateTime? PostponeEndDate
		{
			get
			{
				return this.PostponeEndDateField;
			}
			set
			{
				this.PostponeEndDateField = value;
			}
		}

		[DataMember]
		public DateTime? PostponeStartDate
		{
			get
			{
				return this.PostponeStartDateField;
			}
			set
			{
				this.PostponeStartDateField = value;
			}
		}

		[DataMember]
		public string PostponedByUserUpn
		{
			get
			{
				return this.PostponedByUserUpnField;
			}
			set
			{
				this.PostponedByUserUpnField = value;
			}
		}

		[DataMember]
		public short PostponedCount
		{
			get
			{
				return this.PostponedCountField;
			}
			set
			{
				this.PostponedCountField = value;
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
		public TenantWorkload[] TenantWorkloads
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

		private int CanceledCountField;

		private DateTime? CanceledDateField;

		private string CanceledReasonField;

		private string CommunicationCultureField;

		private bool HasPartnerField;

		private bool HasPilotedUsersField;

		private bool HasSyndicationPartnerField;

		private string InitialDomainField;

		private bool? IsAutoScheduledField;

		private bool IsPTenantField;

		private bool IsTestTenantField;

		private string NameField;

		private string PhaseNameField;

		private Status PhaseStatusField;

		private DateTime? PilotEndDateField;

		private DateTime? PilotStartDateField;

		private DateTime? PostponeEndDateField;

		private DateTime? PostponeStartDateField;

		private string PostponedByUserUpnField;

		private short PostponedCountField;

		private DateTime? PostponedDateField;

		private string PrimaryDomainField;

		private TenantEmail[] TenantEmailsField;

		private Guid TenantIdField;

		private TenantWorkload[] TenantWorkloadsField;

		private string TierNameField;

		private DateTime? UpgradeEndDateField;

		private DateTime? UpgradeStartDateField;
	}
}
