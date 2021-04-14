using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.CalendarDiagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ActionBasedCalendarItemStateDefinition : SinglePropertyValueBasedCalendarItemStateDefinition<COWTriggerAction?>
	{
		static ActionBasedCalendarItemStateDefinition()
		{
			ActionBasedCalendarItemStateDefinition.deleteActionSet = new HashSet<COWTriggerAction?>();
			ActionBasedCalendarItemStateDefinition.deleteActionSet.Add(new COWTriggerAction?(COWTriggerAction.MoveToDeletedItems));
			ActionBasedCalendarItemStateDefinition.deleteActionSet.Add(new COWTriggerAction?(COWTriggerAction.SoftDelete));
			ActionBasedCalendarItemStateDefinition.deleteActionSet.Add(new COWTriggerAction?(COWTriggerAction.HardDelete));
		}

		public ActionBasedCalendarItemStateDefinition(HashSet<COWTriggerAction?> actionSet) : base(CalendarItemBaseSchema.CalendarLogTriggerAction, actionSet)
		{
		}

		public override string SchemaKey
		{
			get
			{
				return "{90B237BC-23D4-4dce-BB8A-B34CF58ECA56}";
			}
		}

		public override StorePropertyDefinition[] RequiredProperties
		{
			get
			{
				return ActionBasedCalendarItemStateDefinition.requiredProperties;
			}
		}

		protected override COWTriggerAction? GetValueFromPropertyBag(PropertyBag propertyBag, MailboxSession session)
		{
			string underlyingValue = base.GetUnderlyingValue<string>(propertyBag);
			COWTriggerAction? result;
			if (string.IsNullOrEmpty(underlyingValue))
			{
				result = null;
			}
			else
			{
				try
				{
					result = new COWTriggerAction?((COWTriggerAction)Enum.Parse(typeof(COWTriggerAction), underlyingValue));
				}
				catch (ArgumentException)
				{
					result = null;
				}
			}
			return result;
		}

		public static ActionBasedCalendarItemStateDefinition CreateDeletedNoneOccurrenceCalendarItemStateDefinition()
		{
			return new ActionBasedCalendarItemStateDefinition(ActionBasedCalendarItemStateDefinition.deleteActionSet);
		}

		private static readonly HashSet<COWTriggerAction?> deleteActionSet;

		private static readonly StorePropertyDefinition[] requiredProperties = new StorePropertyDefinition[]
		{
			CalendarItemBaseSchema.ClientIntent,
			CalendarItemBaseSchema.CalendarLogTriggerAction
		};
	}
}
