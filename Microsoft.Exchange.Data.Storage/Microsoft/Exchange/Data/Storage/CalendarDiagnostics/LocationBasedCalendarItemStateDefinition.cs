using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.CalendarDiagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class LocationBasedCalendarItemStateDefinition : SinglePropertyValueBasedCalendarItemStateDefinition<string>
	{
		public LocationBasedCalendarItemStateDefinition(string targetLocation) : base(CalendarItemBaseSchema.Location, targetLocation)
		{
		}

		public override string SchemaKey
		{
			get
			{
				return "{0F4C3DE4-541B-4159-B0A9-99869D67C819}";
			}
		}

		public override StorePropertyDefinition[] RequiredProperties
		{
			get
			{
				return LocationBasedCalendarItemStateDefinition.requiredProperties;
			}
		}

		private static readonly StorePropertyDefinition[] requiredProperties = new StorePropertyDefinition[]
		{
			CalendarItemBaseSchema.ClientIntent,
			CalendarItemBaseSchema.Location
		};
	}
}
