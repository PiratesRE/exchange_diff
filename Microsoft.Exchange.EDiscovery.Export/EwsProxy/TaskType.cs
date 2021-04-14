using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[Serializable]
	public class TaskType : ItemType
	{
		public int ActualWork
		{
			get
			{
				return this.actualWorkField;
			}
			set
			{
				this.actualWorkField = value;
			}
		}

		[XmlIgnore]
		public bool ActualWorkSpecified
		{
			get
			{
				return this.actualWorkFieldSpecified;
			}
			set
			{
				this.actualWorkFieldSpecified = value;
			}
		}

		public DateTime AssignedTime
		{
			get
			{
				return this.assignedTimeField;
			}
			set
			{
				this.assignedTimeField = value;
			}
		}

		[XmlIgnore]
		public bool AssignedTimeSpecified
		{
			get
			{
				return this.assignedTimeFieldSpecified;
			}
			set
			{
				this.assignedTimeFieldSpecified = value;
			}
		}

		public string BillingInformation
		{
			get
			{
				return this.billingInformationField;
			}
			set
			{
				this.billingInformationField = value;
			}
		}

		public int ChangeCount
		{
			get
			{
				return this.changeCountField;
			}
			set
			{
				this.changeCountField = value;
			}
		}

		[XmlIgnore]
		public bool ChangeCountSpecified
		{
			get
			{
				return this.changeCountFieldSpecified;
			}
			set
			{
				this.changeCountFieldSpecified = value;
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		public string[] Companies
		{
			get
			{
				return this.companiesField;
			}
			set
			{
				this.companiesField = value;
			}
		}

		public DateTime CompleteDate
		{
			get
			{
				return this.completeDateField;
			}
			set
			{
				this.completeDateField = value;
			}
		}

		[XmlIgnore]
		public bool CompleteDateSpecified
		{
			get
			{
				return this.completeDateFieldSpecified;
			}
			set
			{
				this.completeDateFieldSpecified = value;
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		public string[] Contacts
		{
			get
			{
				return this.contactsField;
			}
			set
			{
				this.contactsField = value;
			}
		}

		public TaskDelegateStateType DelegationState
		{
			get
			{
				return this.delegationStateField;
			}
			set
			{
				this.delegationStateField = value;
			}
		}

		[XmlIgnore]
		public bool DelegationStateSpecified
		{
			get
			{
				return this.delegationStateFieldSpecified;
			}
			set
			{
				this.delegationStateFieldSpecified = value;
			}
		}

		public string Delegator
		{
			get
			{
				return this.delegatorField;
			}
			set
			{
				this.delegatorField = value;
			}
		}

		public DateTime DueDate
		{
			get
			{
				return this.dueDateField;
			}
			set
			{
				this.dueDateField = value;
			}
		}

		[XmlIgnore]
		public bool DueDateSpecified
		{
			get
			{
				return this.dueDateFieldSpecified;
			}
			set
			{
				this.dueDateFieldSpecified = value;
			}
		}

		public int IsAssignmentEditable
		{
			get
			{
				return this.isAssignmentEditableField;
			}
			set
			{
				this.isAssignmentEditableField = value;
			}
		}

		[XmlIgnore]
		public bool IsAssignmentEditableSpecified
		{
			get
			{
				return this.isAssignmentEditableFieldSpecified;
			}
			set
			{
				this.isAssignmentEditableFieldSpecified = value;
			}
		}

		public bool IsComplete
		{
			get
			{
				return this.isCompleteField;
			}
			set
			{
				this.isCompleteField = value;
			}
		}

		[XmlIgnore]
		public bool IsCompleteSpecified
		{
			get
			{
				return this.isCompleteFieldSpecified;
			}
			set
			{
				this.isCompleteFieldSpecified = value;
			}
		}

		public bool IsRecurring
		{
			get
			{
				return this.isRecurringField;
			}
			set
			{
				this.isRecurringField = value;
			}
		}

		[XmlIgnore]
		public bool IsRecurringSpecified
		{
			get
			{
				return this.isRecurringFieldSpecified;
			}
			set
			{
				this.isRecurringFieldSpecified = value;
			}
		}

		public bool IsTeamTask
		{
			get
			{
				return this.isTeamTaskField;
			}
			set
			{
				this.isTeamTaskField = value;
			}
		}

		[XmlIgnore]
		public bool IsTeamTaskSpecified
		{
			get
			{
				return this.isTeamTaskFieldSpecified;
			}
			set
			{
				this.isTeamTaskFieldSpecified = value;
			}
		}

		public string Mileage
		{
			get
			{
				return this.mileageField;
			}
			set
			{
				this.mileageField = value;
			}
		}

		public string Owner
		{
			get
			{
				return this.ownerField;
			}
			set
			{
				this.ownerField = value;
			}
		}

		public double PercentComplete
		{
			get
			{
				return this.percentCompleteField;
			}
			set
			{
				this.percentCompleteField = value;
			}
		}

		[XmlIgnore]
		public bool PercentCompleteSpecified
		{
			get
			{
				return this.percentCompleteFieldSpecified;
			}
			set
			{
				this.percentCompleteFieldSpecified = value;
			}
		}

		public TaskRecurrenceType Recurrence
		{
			get
			{
				return this.recurrenceField;
			}
			set
			{
				this.recurrenceField = value;
			}
		}

		public DateTime StartDate
		{
			get
			{
				return this.startDateField;
			}
			set
			{
				this.startDateField = value;
			}
		}

		[XmlIgnore]
		public bool StartDateSpecified
		{
			get
			{
				return this.startDateFieldSpecified;
			}
			set
			{
				this.startDateFieldSpecified = value;
			}
		}

		public TaskStatusType Status
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

		[XmlIgnore]
		public bool StatusSpecified
		{
			get
			{
				return this.statusFieldSpecified;
			}
			set
			{
				this.statusFieldSpecified = value;
			}
		}

		public string StatusDescription
		{
			get
			{
				return this.statusDescriptionField;
			}
			set
			{
				this.statusDescriptionField = value;
			}
		}

		public int TotalWork
		{
			get
			{
				return this.totalWorkField;
			}
			set
			{
				this.totalWorkField = value;
			}
		}

		[XmlIgnore]
		public bool TotalWorkSpecified
		{
			get
			{
				return this.totalWorkFieldSpecified;
			}
			set
			{
				this.totalWorkFieldSpecified = value;
			}
		}

		private int actualWorkField;

		private bool actualWorkFieldSpecified;

		private DateTime assignedTimeField;

		private bool assignedTimeFieldSpecified;

		private string billingInformationField;

		private int changeCountField;

		private bool changeCountFieldSpecified;

		private string[] companiesField;

		private DateTime completeDateField;

		private bool completeDateFieldSpecified;

		private string[] contactsField;

		private TaskDelegateStateType delegationStateField;

		private bool delegationStateFieldSpecified;

		private string delegatorField;

		private DateTime dueDateField;

		private bool dueDateFieldSpecified;

		private int isAssignmentEditableField;

		private bool isAssignmentEditableFieldSpecified;

		private bool isCompleteField;

		private bool isCompleteFieldSpecified;

		private bool isRecurringField;

		private bool isRecurringFieldSpecified;

		private bool isTeamTaskField;

		private bool isTeamTaskFieldSpecified;

		private string mileageField;

		private string ownerField;

		private double percentCompleteField;

		private bool percentCompleteFieldSpecified;

		private TaskRecurrenceType recurrenceField;

		private DateTime startDateField;

		private bool startDateFieldSpecified;

		private TaskStatusType statusField;

		private bool statusFieldSpecified;

		private string statusDescriptionField;

		private int totalWorkField;

		private bool totalWorkFieldSpecified;
	}
}
