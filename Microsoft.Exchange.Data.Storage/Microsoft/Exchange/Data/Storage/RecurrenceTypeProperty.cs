using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RecurrenceTypeProperty : SmartPropertyDefinition
	{
		internal RecurrenceTypeProperty() : base("RecurrenceType", typeof(RecurrenceType), PropertyFlags.ReadOnly, PropertyDefinitionConstraint.None, new PropertyDependency[]
		{
			new PropertyDependency(InternalSchema.IsOneOff, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.ItemClass, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.TaskRecurrence, PropertyDependencyType.NeedForRead),
			new PropertyDependency(InternalSchema.MapiRecurrenceType, PropertyDependencyType.NeedForRead)
		})
		{
		}

		protected override object InternalTryGetValue(PropertyBag.BasicPropertyStore propertyBag)
		{
			string valueOrDefault = propertyBag.GetValueOrDefault<string>(InternalSchema.ItemClass);
			if (valueOrDefault == null)
			{
				return RecurrenceType.None;
			}
			if (ObjectClass.IsTask(valueOrDefault) || ObjectClass.IsTaskRequest(valueOrDefault))
			{
				if (propertyBag.GetValueOrDefault<bool>(InternalSchema.IsOneOff))
				{
					return RecurrenceType.None;
				}
				byte[] valueOrDefault2 = propertyBag.GetValueOrDefault<byte[]>(InternalSchema.TaskRecurrence);
				return RecurrenceTypeProperty.GetTaskRecurrenceTypeFromBlob(valueOrDefault2);
			}
			else
			{
				if (ObjectClass.IsCalendarItemCalendarItemOccurrenceOrRecurrenceException(valueOrDefault) || ObjectClass.IsMeetingMessage(valueOrDefault))
				{
					return propertyBag.GetValueOrDefault<RecurrenceType>(InternalSchema.MapiRecurrenceType);
				}
				return RecurrenceType.None;
			}
		}

		private static RecurrenceType GetTaskRecurrenceTypeFromBlob(byte[] blob)
		{
			if (blob == null)
			{
				return RecurrenceType.None;
			}
			RecurrenceType result;
			try
			{
				TaskRecurrence taskRecurrence = InternalRecurrence.InternalParseTask(blob, null, null, null);
				result = RecurrenceTypeProperty.TaskRecurrencePatternToRecurrenceType(taskRecurrence);
			}
			catch (RecurrenceFormatException)
			{
				result = RecurrenceType.None;
			}
			return result;
		}

		private static RecurrenceType TaskRecurrencePatternToRecurrenceType(TaskRecurrence taskRecurrence)
		{
			if (taskRecurrence == null)
			{
				return RecurrenceType.None;
			}
			RecurrencePattern pattern = taskRecurrence.Pattern;
			if (pattern is DailyRegeneratingPattern)
			{
				return RecurrenceType.DailyRegenerating;
			}
			if (pattern is WeeklyRegeneratingPattern)
			{
				return RecurrenceType.WeeklyRegenerating;
			}
			if (pattern is MonthlyRegeneratingPattern)
			{
				return RecurrenceType.MonthlyRegenerating;
			}
			if (pattern is YearlyRegeneratingPattern)
			{
				return RecurrenceType.YearlyRegenerating;
			}
			if (pattern is DailyRecurrencePattern)
			{
				return RecurrenceType.Daily;
			}
			if (pattern is WeeklyRecurrencePattern)
			{
				return RecurrenceType.Weekly;
			}
			if (pattern is MonthlyRecurrencePattern || pattern is MonthlyThRecurrencePattern)
			{
				return RecurrenceType.Monthly;
			}
			if (pattern is YearlyRecurrencePattern || pattern is YearlyThRecurrencePattern)
			{
				return RecurrenceType.Yearly;
			}
			return RecurrenceType.None;
		}
	}
}
