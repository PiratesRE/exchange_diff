using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class TaskSchema : Schema
	{
		static TaskSchema()
		{
			XmlElementInformation[] xmlElements = new XmlElementInformation[]
			{
				TaskSchema.ActualWork,
				TaskSchema.AssignedTime,
				TaskSchema.BillingInformation,
				TaskSchema.ChangeCount,
				TaskSchema.Companies,
				TaskSchema.CompleteDate,
				TaskSchema.Contacts,
				TaskSchema.DelegationState,
				TaskSchema.Delegator,
				TaskSchema.DueDate,
				TaskSchema.IsAssignmentEditable,
				TaskSchema.IsComplete,
				TaskSchema.IsRecurring,
				TaskSchema.IsTeamTask,
				TaskSchema.Mileage,
				TaskSchema.Owner,
				TaskSchema.PercentComplete,
				TaskSchema.Recurrence,
				TaskSchema.StartDate,
				TaskSchema.Status,
				TaskSchema.StatusDescription,
				TaskSchema.TotalWork,
				TaskSchema.ModernReminders,
				TaskSchema.DoItTime
			};
			TaskSchema.schema = new TaskSchema(xmlElements);
		}

		private TaskSchema(XmlElementInformation[] xmlElements) : base(xmlElements)
		{
		}

		public static Schema GetSchema()
		{
			return TaskSchema.schema;
		}

		private static Schema schema;

		public static readonly PropertyInformation ActualWork = new PropertyInformation("ActualWork", ExchangeVersion.Exchange2007, TaskSchema.ActualWork, new PropertyUri(PropertyUriEnum.TaskActualWork), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation AssignedTime = new PropertyInformation("AssignedTime", ExchangeVersion.Exchange2007, TaskSchema.AssignedTime, new PropertyUri(PropertyUriEnum.TaskAssignedTime), new PropertyCommand.CreatePropertyCommand(DateTimeProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation BillingInformation = new PropertyInformation("BillingInformation", ExchangeVersion.Exchange2007, TaskSchema.BillingInformation, new PropertyUri(PropertyUriEnum.TaskBillingInformation), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation ChangeCount = new PropertyInformation("ChangeCount", ExchangeVersion.Exchange2007, TaskSchema.TaskChangeCount, new PropertyUri(PropertyUriEnum.TaskChangeCount), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation Companies = new ArrayPropertyInformation("Companies", ExchangeVersion.Exchange2007, "String", TaskSchema.Companies, new PropertyUri(PropertyUriEnum.TaskCompanies), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation CompleteDate = new PropertyInformation("CompleteDate", ExchangeVersion.Exchange2007, ItemSchema.CompleteDate, new PropertyUri(PropertyUriEnum.TaskCompleteDate), new PropertyCommand.CreatePropertyCommand(CompleteDateProperty.CreateCommand));

		public static readonly PropertyInformation Contacts = new ArrayPropertyInformation("Contacts", ExchangeVersion.Exchange2007, "String", TaskSchema.Contacts, new PropertyUri(PropertyUriEnum.TaskContacts), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation DelegationState = new PropertyInformation("DelegationState", ExchangeVersion.Exchange2007, TaskSchema.DelegationState, new PropertyUri(PropertyUriEnum.TaskDelegationState), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation Delegator = new PropertyInformation("Delegator", ExchangeVersion.Exchange2007, TaskSchema.TaskDelegator, new PropertyUri(PropertyUriEnum.TaskDelegator), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation DueDate = new PropertyInformation("DueDate", ExchangeVersion.Exchange2007, TaskSchema.DueDate, new PropertyUri(PropertyUriEnum.TaskDueDate), new PropertyCommand.CreatePropertyCommand(TaskDueDateProperty.CreateCommand));

		public static readonly PropertyInformation IsAssignmentEditable = new PropertyInformation("IsAssignmentEditable", ExchangeVersion.Exchange2007, TaskSchema.IsAssignmentEditable, new PropertyUri(PropertyUriEnum.TaskIsAssignmentEditable), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation IsComplete = new PropertyInformation("IsComplete", ExchangeVersion.Exchange2007, ItemSchema.IsComplete, new PropertyUri(PropertyUriEnum.TaskIsComplete), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation IsRecurring = new PropertyInformation("IsRecurring", ExchangeVersion.Exchange2007, TaskSchema.IsTaskRecurring, new PropertyUri(PropertyUriEnum.TaskIsTaskRecurring), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation IsTeamTask = new PropertyInformation("IsTeamTask", ExchangeVersion.Exchange2007, TaskSchema.IsTeamTask, new PropertyUri(PropertyUriEnum.TaskIsTeamTask), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation Mileage = new PropertyInformation("Mileage", ExchangeVersion.Exchange2007, TaskSchema.Mileage, new PropertyUri(PropertyUriEnum.TaskMileage), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation Owner = new PropertyInformation("Owner", ExchangeVersion.Exchange2007, TaskSchema.TaskOwner, new PropertyUri(PropertyUriEnum.TaskOwner), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation PercentComplete = new PropertyInformation("PercentComplete", ExchangeVersion.Exchange2007, ItemSchema.PercentComplete, new PropertyUri(PropertyUriEnum.TaskPercentComplete), new PropertyCommand.CreatePropertyCommand(PercentCompleteProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation Recurrence = new PropertyInformation("Recurrence", ExchangeVersion.Exchange2007, null, new PropertyUri(PropertyUriEnum.TaskRecurrence), new PropertyCommand.CreatePropertyCommand(TaskRecurrenceProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsDeleteUpdateCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand);

		public static readonly PropertyInformation StartDate = new PropertyInformation("StartDate", ExchangeVersion.Exchange2007, TaskSchema.StartDate, new PropertyUri(PropertyUriEnum.TaskStartDate), new PropertyCommand.CreatePropertyCommand(TaskStartDateProperty.CreateCommand));

		public static readonly PropertyInformation Status = new PropertyInformation("Status", ExchangeVersion.Exchange2007, ItemSchema.TaskStatus, new PropertyUri(PropertyUriEnum.TaskStatus), new PropertyCommand.CreatePropertyCommand(TaskStatusProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsToXmlCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand | PropertyInformationAttributes.ImplementsToXmlForPropertyBagCommand | PropertyInformationAttributes.ImplementsToServiceObjectCommand | PropertyInformationAttributes.ImplementsToServiceObjectForPropertyBagCommand);

		public static readonly PropertyInformation StatusDescription = new PropertyInformation("StatusDescription", ExchangeVersion.Exchange2007, TaskSchema.StatusDescription, new PropertyUri(PropertyUriEnum.TaskStatusDescription), new PropertyCommand.CreatePropertyCommand(TaskStatusDescriptionProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);

		public static readonly PropertyInformation TotalWork = new PropertyInformation("TotalWork", ExchangeVersion.Exchange2007, TaskSchema.TotalWork, new PropertyUri(PropertyUriEnum.TaskTotalWork), new PropertyCommand.CreatePropertyCommand(SimpleProperty.CreateCommand));

		public static readonly PropertyInformation ModernReminders = new PropertyInformation("ModernReminders", ExchangeVersion.Exchange2013, TaskSchema.QuickCaptureReminders, new PropertyUri(PropertyUriEnum.ModernReminders), new PropertyCommand.CreatePropertyCommand(ModernRemindersTaskProperty.CreateCommand), PropertyInformationAttributes.ImplementsSetCommand | PropertyInformationAttributes.ImplementsSetUpdateCommand);

		public static readonly PropertyInformation DoItTime = new PropertyInformation("DoItTime", ExchangeVersion.Exchange2013, TaskSchema.DoItTime, new PropertyUri(PropertyUriEnum.TaskDoItTime), new PropertyCommand.CreatePropertyCommand(DateTimeProperty.CreateCommand), PropertyInformationAttributes.ImplementsReadOnlyCommands);
	}
}
