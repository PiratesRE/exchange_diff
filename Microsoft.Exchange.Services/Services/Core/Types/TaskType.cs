using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "Task")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class TaskType : ItemType
	{
		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 1)]
		public int ActualWork
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<int>(TaskSchema.ActualWork);
			}
			set
			{
				base.PropertyBag[TaskSchema.ActualWork] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool ActualWorkSpecified
		{
			get
			{
				return base.PropertyBag.Contains(TaskSchema.ActualWork);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 2)]
		public string AssignedTime
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(TaskSchema.AssignedTime);
			}
			set
			{
				base.PropertyBag[TaskSchema.AssignedTime] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool AssignedTimeSpecified
		{
			get
			{
				return base.PropertyBag.Contains(TaskSchema.AssignedTime);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 3)]
		public string BillingInformation
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(TaskSchema.BillingInformation);
			}
			set
			{
				base.PropertyBag[TaskSchema.BillingInformation] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 4)]
		public int ChangeCount
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<int>(TaskSchema.ChangeCount);
			}
			set
			{
				base.PropertyBag[TaskSchema.ChangeCount] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool ChangeCountSpecified
		{
			get
			{
				return base.PropertyBag.Contains(TaskSchema.ChangeCount);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 5)]
		[XmlArrayItem("String", IsNullable = false)]
		public string[] Companies
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string[]>(TaskSchema.Companies);
			}
			set
			{
				base.PropertyBag[TaskSchema.Companies] = value;
			}
		}

		[DateTimeString]
		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 6)]
		public string CompleteDate
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(TaskSchema.CompleteDate);
			}
			set
			{
				base.PropertyBag[TaskSchema.CompleteDate] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool CompleteDateSpecified
		{
			get
			{
				return base.PropertyBag.Contains(TaskSchema.CompleteDate);
			}
			set
			{
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 7)]
		public string[] Contacts
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string[]>(TaskSchema.Contacts);
			}
			set
			{
				base.PropertyBag[TaskSchema.Contacts] = value;
			}
		}

		[XmlElement]
		[IgnoreDataMember]
		public TaskDelegateStateType DelegationState
		{
			get
			{
				if (!this.DelegationStateSpecified)
				{
					return TaskDelegateStateType.NoMatch;
				}
				return EnumUtilities.Parse<TaskDelegateStateType>(this.DelegationStateString);
			}
			set
			{
				base.PropertyBag[TaskSchema.DelegationState] = value;
			}
		}

		[DataMember(Name = "DelegationState", EmitDefaultValue = false, Order = 8)]
		[XmlIgnore]
		public string DelegationStateString
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(TaskSchema.DelegationState);
			}
			set
			{
				base.PropertyBag[TaskSchema.DelegationState] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool DelegationStateSpecified
		{
			get
			{
				return base.IsSet(TaskSchema.DelegationState);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 9)]
		public string Delegator
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(TaskSchema.Delegator);
			}
			set
			{
				base.PropertyBag[TaskSchema.Delegator] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 10)]
		[DateTimeString]
		public string DueDate
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(TaskSchema.DueDate);
			}
			set
			{
				base.PropertyBag[TaskSchema.DueDate] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool DueDateSpecified
		{
			get
			{
				return base.PropertyBag.Contains(TaskSchema.DueDate);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 11)]
		public int IsAssignmentEditable
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<int>(TaskSchema.IsAssignmentEditable);
			}
			set
			{
				base.PropertyBag[TaskSchema.IsAssignmentEditable] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IsAssignmentEditableSpecified
		{
			get
			{
				return base.PropertyBag.Contains(TaskSchema.IsAssignmentEditable);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 12)]
		public bool IsComplete
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<bool>(TaskSchema.IsComplete);
			}
			set
			{
				base.PropertyBag[TaskSchema.IsComplete] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool IsCompleteSpecified
		{
			get
			{
				return base.PropertyBag.Contains(TaskSchema.IsComplete);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 13)]
		public bool IsRecurring
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<bool>(TaskSchema.IsRecurring);
			}
			set
			{
				base.PropertyBag[TaskSchema.IsRecurring] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool IsRecurringSpecified
		{
			get
			{
				return base.PropertyBag.Contains(TaskSchema.IsRecurring);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 14)]
		public bool IsTeamTask
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<bool>(TaskSchema.IsTeamTask);
			}
			set
			{
				base.PropertyBag[TaskSchema.IsTeamTask] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool IsTeamTaskSpecified
		{
			get
			{
				return base.PropertyBag.Contains(TaskSchema.IsTeamTask);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 15)]
		public string Mileage
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(TaskSchema.Mileage);
			}
			set
			{
				base.PropertyBag[TaskSchema.Mileage] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 16)]
		public string Owner
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(TaskSchema.Owner);
			}
			set
			{
				base.PropertyBag[TaskSchema.Owner] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 17)]
		public string PercentComplete
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(TaskSchema.PercentComplete);
			}
			set
			{
				base.PropertyBag[TaskSchema.PercentComplete] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool PercentCompleteSpecified
		{
			get
			{
				return base.PropertyBag.Contains(TaskSchema.PercentComplete);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 18)]
		public TaskRecurrenceType Recurrence
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<TaskRecurrenceType>(TaskSchema.Recurrence);
			}
			set
			{
				base.PropertyBag[TaskSchema.Recurrence] = value;
			}
		}

		[DateTimeString]
		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 19)]
		public string StartDate
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(TaskSchema.StartDate);
			}
			set
			{
				base.PropertyBag[TaskSchema.StartDate] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool StartDateSpecified
		{
			get
			{
				return base.PropertyBag.Contains(TaskSchema.StartDate);
			}
			set
			{
			}
		}

		[XmlElement]
		[IgnoreDataMember]
		public TaskStatusType Status
		{
			get
			{
				if (!this.StatusSpecified)
				{
					return TaskStatusType.NotStarted;
				}
				return EnumUtilities.Parse<TaskStatusType>(this.StatusString);
			}
			set
			{
				base.PropertyBag[TaskSchema.Status] = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "Status", EmitDefaultValue = false, Order = 20)]
		public string StatusString
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(TaskSchema.Status);
			}
			set
			{
				base.PropertyBag[TaskSchema.Status] = value;
			}
		}

		[IgnoreDataMember]
		[XmlIgnore]
		public bool StatusSpecified
		{
			get
			{
				return base.IsSet(TaskSchema.Status);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 21)]
		public string StatusDescription
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(TaskSchema.StatusDescription);
			}
			set
			{
				base.PropertyBag[TaskSchema.StatusDescription] = value;
			}
		}

		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 22)]
		public int TotalWork
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<int>(TaskSchema.TotalWork);
			}
			set
			{
				base.PropertyBag[TaskSchema.TotalWork] = value;
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool TotalWorkSpecified
		{
			get
			{
				return base.PropertyBag.Contains(TaskSchema.TotalWork);
			}
			set
			{
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 23)]
		[XmlIgnore]
		public ModernReminderType[] ModernReminders
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<ModernReminderType[]>(TaskSchema.ModernReminders);
			}
			set
			{
				base.PropertyBag[TaskSchema.ModernReminders] = value;
			}
		}

		[DateTimeString]
		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, IsRequired = false, Order = 24)]
		public string DoItTime
		{
			get
			{
				return base.PropertyBag.GetValueOrDefault<string>(TaskSchema.DoItTime);
			}
			set
			{
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool DoItTimeSpecified
		{
			get
			{
				return base.PropertyBag.Contains(TaskSchema.DoItTime);
			}
			set
			{
			}
		}

		internal override StoreObjectType StoreObjectType
		{
			get
			{
				return StoreObjectType.Task;
			}
		}
	}
}
