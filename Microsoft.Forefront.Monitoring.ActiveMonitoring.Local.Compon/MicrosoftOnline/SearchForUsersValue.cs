using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class SearchForUsersValue : DirectorySearch
	{
		public SearchForUsersValue()
		{
			this.administratorsOnlyField = false;
			this.assignedLicenseUnsetOnlyField = false;
			this.besServiceInstanceSetOnlyField = false;
			this.licenseReconciliationNeededOnlyField = false;
			this.mailboxGuidSetOnlyField = false;
			this.softDeletedField = false;
			this.synchronizedOnlyField = false;
			this.validationErrorUnresolvedOnlyField = false;
			this.validationOrProvisionErrorOnlyField = false;
		}

		[XmlArrayItem("AssignedPlanFilter", IsNullable = false)]
		public AssignedPlanFilterValue[] AssignedPlanFilters
		{
			get
			{
				return this.assignedPlanFiltersField;
			}
			set
			{
				this.assignedPlanFiltersField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool AdministratorsOnly
		{
			get
			{
				return this.administratorsOnlyField;
			}
			set
			{
				this.administratorsOnlyField = value;
			}
		}

		[XmlAttribute]
		public string[] AssignedLicenseFilter
		{
			get
			{
				return this.assignedLicenseFilterField;
			}
			set
			{
				this.assignedLicenseFilterField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool AssignedLicenseUnsetOnly
		{
			get
			{
				return this.assignedLicenseUnsetOnlyField;
			}
			set
			{
				this.assignedLicenseUnsetOnlyField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool BesServiceInstanceSetOnly
		{
			get
			{
				return this.besServiceInstanceSetOnlyField;
			}
			set
			{
				this.besServiceInstanceSetOnlyField = value;
			}
		}

		[XmlAttribute]
		public string City
		{
			get
			{
				return this.cityField;
			}
			set
			{
				this.cityField = value;
			}
		}

		[XmlAttribute]
		public string Country
		{
			get
			{
				return this.countryField;
			}
			set
			{
				this.countryField = value;
			}
		}

		[XmlAttribute]
		public string Department
		{
			get
			{
				return this.departmentField;
			}
			set
			{
				this.departmentField = value;
			}
		}

		[XmlAttribute]
		public string DomainInUse
		{
			get
			{
				return this.domainInUseField;
			}
			set
			{
				this.domainInUseField = value;
			}
		}

		[XmlAttribute]
		public bool Enabled
		{
			get
			{
				return this.enabledField;
			}
			set
			{
				this.enabledField = value;
			}
		}

		[XmlIgnore]
		public bool EnabledSpecified
		{
			get
			{
				return this.enabledFieldSpecified;
			}
			set
			{
				this.enabledFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public string JobTitle
		{
			get
			{
				return this.jobTitleField;
			}
			set
			{
				this.jobTitleField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool LicenseReconciliationNeededOnly
		{
			get
			{
				return this.licenseReconciliationNeededOnlyField;
			}
			set
			{
				this.licenseReconciliationNeededOnlyField = value;
			}
		}

		[DefaultValue(false)]
		[XmlAttribute]
		public bool MailboxGuidSetOnly
		{
			get
			{
				return this.mailboxGuidSetOnlyField;
			}
			set
			{
				this.mailboxGuidSetOnlyField = value;
			}
		}

		[XmlAttribute]
		public int MigrationState
		{
			get
			{
				return this.migrationStateField;
			}
			set
			{
				this.migrationStateField = value;
			}
		}

		[XmlIgnore]
		public bool MigrationStateSpecified
		{
			get
			{
				return this.migrationStateFieldSpecified;
			}
			set
			{
				this.migrationStateFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public string Name
		{
			get
			{
				return this.nameField;
			}
			set
			{
				this.nameField = value;
			}
		}

		[XmlAttribute]
		public string SearchId
		{
			get
			{
				return this.searchIdField;
			}
			set
			{
				this.searchIdField = value;
			}
		}

		[DefaultValue(false)]
		[XmlAttribute]
		public bool SoftDeleted
		{
			get
			{
				return this.softDeletedField;
			}
			set
			{
				this.softDeletedField = value;
			}
		}

		[XmlAttribute]
		public DateTime SoftDeletionTimestampAfterOrAt
		{
			get
			{
				return this.softDeletionTimestampAfterOrAtField;
			}
			set
			{
				this.softDeletionTimestampAfterOrAtField = value;
			}
		}

		[XmlIgnore]
		public bool SoftDeletionTimestampAfterOrAtSpecified
		{
			get
			{
				return this.softDeletionTimestampAfterOrAtFieldSpecified;
			}
			set
			{
				this.softDeletionTimestampAfterOrAtFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public DateTime SoftDeletionTimestampBeforeOrAt
		{
			get
			{
				return this.softDeletionTimestampBeforeOrAtField;
			}
			set
			{
				this.softDeletionTimestampBeforeOrAtField = value;
			}
		}

		[XmlIgnore]
		public bool SoftDeletionTimestampBeforeOrAtSpecified
		{
			get
			{
				return this.softDeletionTimestampBeforeOrAtFieldSpecified;
			}
			set
			{
				this.softDeletionTimestampBeforeOrAtFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public string State
		{
			get
			{
				return this.stateField;
			}
			set
			{
				this.stateField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool SynchronizedOnly
		{
			get
			{
				return this.synchronizedOnlyField;
			}
			set
			{
				this.synchronizedOnlyField = value;
			}
		}

		[XmlAttribute]
		public string UsageLocation
		{
			get
			{
				return this.usageLocationField;
			}
			set
			{
				this.usageLocationField = value;
			}
		}

		[XmlAttribute]
		public string ValidationErrorServiceType
		{
			get
			{
				return this.validationErrorServiceTypeField;
			}
			set
			{
				this.validationErrorServiceTypeField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool ValidationErrorUnresolvedOnly
		{
			get
			{
				return this.validationErrorUnresolvedOnlyField;
			}
			set
			{
				this.validationErrorUnresolvedOnlyField = value;
			}
		}

		[DefaultValue(false)]
		[XmlAttribute]
		public bool ValidationOrProvisionErrorOnly
		{
			get
			{
				return this.validationOrProvisionErrorOnlyField;
			}
			set
			{
				this.validationOrProvisionErrorOnlyField = value;
			}
		}

		private AssignedPlanFilterValue[] assignedPlanFiltersField;

		private bool administratorsOnlyField;

		private string[] assignedLicenseFilterField;

		private bool assignedLicenseUnsetOnlyField;

		private bool besServiceInstanceSetOnlyField;

		private string cityField;

		private string countryField;

		private string departmentField;

		private string domainInUseField;

		private bool enabledField;

		private bool enabledFieldSpecified;

		private string jobTitleField;

		private bool licenseReconciliationNeededOnlyField;

		private bool mailboxGuidSetOnlyField;

		private int migrationStateField;

		private bool migrationStateFieldSpecified;

		private string nameField;

		private string searchIdField;

		private bool softDeletedField;

		private DateTime softDeletionTimestampAfterOrAtField;

		private bool softDeletionTimestampAfterOrAtFieldSpecified;

		private DateTime softDeletionTimestampBeforeOrAtField;

		private bool softDeletionTimestampBeforeOrAtFieldSpecified;

		private string stateField;

		private bool synchronizedOnlyField;

		private string usageLocationField;

		private string validationErrorServiceTypeField;

		private bool validationErrorUnresolvedOnlyField;

		private bool validationOrProvisionErrorOnlyField;
	}
}
