using System;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class WorkingHoursSchema : UserConfigurationObjectSchema
	{
		public static readonly SimplePropertyDefinition WorkDays = new SimplePropertyDefinition("WorkDays", ExchangeObjectVersion.Exchange2007, typeof(DaysOfWeek), PropertyDefinitionFlags.None, DaysOfWeek.Weekdays, DaysOfWeek.Weekdays, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimplePropertyDefinition WorkingHoursStartTime = new SimplePropertyDefinition("WorkingHoursStartTime", ExchangeObjectVersion.Exchange2007, typeof(TimeSpan), PropertyDefinitionFlags.None, new TimeSpan(0, 8, 0, 0), new TimeSpan(0, 8, 0, 0), PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<TimeSpan>(TimeSpan.FromDays(0.0), TimeSpan.FromDays(1.0) - TimeSpan.FromTicks(1L))
		});

		public static readonly SimplePropertyDefinition WorkingHoursEndTime = new SimplePropertyDefinition("WorkingHoursEndTime", ExchangeObjectVersion.Exchange2007, typeof(TimeSpan), PropertyDefinitionFlags.None, new TimeSpan(0, 17, 0, 0), new TimeSpan(0, 17, 0, 0), PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<TimeSpan>(TimeSpan.FromDays(0.0), TimeSpan.FromDays(1.0) - TimeSpan.FromTicks(1L))
		});

		public static readonly SimplePropertyDefinition WorkingHoursTimeZone = new SimplePropertyDefinition("WorkingHoursTimeZone", ExchangeObjectVersion.Exchange2010, typeof(ExTimeZoneValue), PropertyDefinitionFlags.None, ExTimeZoneValue.Parse("Pacific Standard Time"), ExTimeZoneValue.Parse("Pacific Standard Time"), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
