using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Compliance.DAR
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TaskAggregateStoreObjectExtendedStoreSchema : ObjectSchema
	{
		public static readonly ExtendedPropertyDefinition Enabled = new ExtendedPropertyDefinition(WellKnownPropertySet.Compliance, "Enabled", 4);

		public static readonly ExtendedPropertyDefinition MaxRunningTasks = new ExtendedPropertyDefinition(WellKnownPropertySet.Compliance, "MaxRunningTasks", 14);

		public static readonly ExtendedPropertyDefinition RecurrenceType = new ExtendedPropertyDefinition(WellKnownPropertySet.Compliance, "RecurrenceType", 14);

		public static readonly ExtendedPropertyDefinition RecurrenceFrequency = new ExtendedPropertyDefinition(WellKnownPropertySet.Compliance, "RecurrenceFrequency", 14);

		public static readonly ExtendedPropertyDefinition RecurrenceInterval = new ExtendedPropertyDefinition(WellKnownPropertySet.Compliance, "RecurrenceInterval", 14);
	}
}
