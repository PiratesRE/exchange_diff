using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class TaskSchema : ItemSchema
	{
		public new static TaskSchema Instance
		{
			get
			{
				if (TaskSchema.instance == null)
				{
					TaskSchema.instance = new TaskSchema();
				}
				return TaskSchema.instance;
			}
		}

		protected override void AddConstraints(List<StoreObjectConstraint> constraints)
		{
			base.AddConstraints(constraints);
			constraints.Add(new CustomConstraint("Supported Recurrences for Tasks constraint", new PropertyDefinition[]
			{
				InternalSchema.TaskRecurrence,
				InternalSchema.IsTaskRecurring,
				InternalSchema.IsOneOff,
				InternalSchema.IconIndex
			}, new IsObjectValidDelegate(Task.IsTaskRecurrenceSupported), true));
		}

		internal override void CoreObjectUpdate(CoreItem coreItem, CoreItemOperation operation)
		{
			base.CoreObjectUpdate(coreItem, operation);
			Task.CoreObjectUpdateTaskStatus(coreItem);
			Task.CoreObjectUpdateRecurrence(coreItem);
			Task.CoreObjectUpdateTaskDates(coreItem);
		}

		protected override ICollection<PropertyRule> PropertyRules
		{
			get
			{
				if (this.propertyRulesCache == null)
				{
					this.propertyRulesCache = base.PropertyRules.Concat(TaskSchema.taskPropertyRules);
				}
				return this.propertyRulesCache;
			}
		}

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition TaskOwner = InternalSchema.TaskOwner;

		[Autoload]
		internal static readonly StorePropertyDefinition TaskRecurrencePattern = InternalSchema.TaskRecurrence;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition TaskChangeCount = InternalSchema.TaskChangeCount;

		[LegalTracking]
		public static readonly StorePropertyDefinition StatusDescription = InternalSchema.StatusDescription;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition IsTaskRecurring = InternalSchema.IsTaskRecurring;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition TotalWork = InternalSchema.TotalWork;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition ActualWork = InternalSchema.ActualWork;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition Mileage = InternalSchema.Mileage;

		[LegalTracking]
		public static readonly StorePropertyDefinition Contacts = InternalSchema.Contacts;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition RecurrenceType = InternalSchema.CalculatedRecurrenceType;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition IsRecurring = InternalSchema.IsRecurring;

		[LegalTracking]
		[Autoload]
		public static readonly StorePropertyDefinition TaskResetReminder = InternalSchema.TaskResetReminder;

		[Autoload]
		[LegalTracking]
		public static readonly StorePropertyDefinition IsOneOff = InternalSchema.IsOneOff;

		[LegalTracking]
		public static readonly StorePropertyDefinition LastUser = InternalSchema.LastModifiedBy;

		[LegalTracking]
		public static readonly StorePropertyDefinition TaskDelegator = InternalSchema.TaskDelegator;

		[LegalTracking]
		public static readonly StorePropertyDefinition AssignedTime = InternalSchema.AssignedTime;

		[LegalTracking]
		public static readonly StorePropertyDefinition OwnershipState = InternalSchema.OwnershipState;

		[LegalTracking]
		public static readonly StorePropertyDefinition DelegationState = InternalSchema.DelegationState;

		[LegalTracking]
		public static readonly StorePropertyDefinition IsAssignmentEditable = InternalSchema.IsAssignmentEditable;

		[LegalTracking]
		public static readonly StorePropertyDefinition TaskType = InternalSchema.TaskType;

		[LegalTracking]
		public static readonly StorePropertyDefinition IsTeamTask = InternalSchema.IsTeamTask;

		[LegalTracking]
		public static readonly StorePropertyDefinition LastUpdateType = InternalSchema.LastUpdateType;

		[LegalTracking]
		[ConditionallyRequired(CustomConstraintDelegateEnum.IsStartDateDefined)]
		[Autoload]
		public static StorePropertyDefinition DueDate = InternalSchema.DueDate;

		[LegalTracking]
		[Autoload]
		public static StorePropertyDefinition StartDate = InternalSchema.StartDate;

		[Autoload]
		public static StorePropertyDefinition DoItTime = InternalSchema.DoItTime;

		[LegalTracking]
		public static readonly StorePropertyDefinition TaskAccepted = InternalSchema.TaskAccepted;

		[LegalTracking]
		public static readonly StorePropertyDefinition BillingInformation = InternalSchema.BillingInformation;

		[LegalTracking]
		public static readonly StorePropertyDefinition Companies = InternalSchema.Companies;

		public static readonly StorePropertyDefinition QuickCaptureReminders = InternalSchema.ModernReminders;

		public static readonly StorePropertyDefinition ModernReminders = InternalSchema.ModernReminders;

		public static readonly StorePropertyDefinition ModernRemindersState = InternalSchema.ModernRemindersState;

		public static readonly StorePropertyDefinition EventTimeBasedInboxReminders = InternalSchema.EventTimeBasedInboxReminders;

		public static readonly StorePropertyDefinition EventTimeBasedInboxRemindersState = InternalSchema.EventTimeBasedInboxRemindersState;

		private static TaskSchema instance = null;

		private ICollection<PropertyRule> propertyRulesCache;

		private static PropertyRule[] taskPropertyRules = new PropertyRule[]
		{
			PropertyRuleLibrary.NoEmptyTaskRecurrenceBlob,
			PropertyRuleLibrary.DoItTimeProperty
		};
	}
}
