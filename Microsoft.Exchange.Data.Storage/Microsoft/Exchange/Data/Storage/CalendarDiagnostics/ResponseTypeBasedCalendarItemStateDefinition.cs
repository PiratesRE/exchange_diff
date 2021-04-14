using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.CalendarDiagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ResponseTypeBasedCalendarItemStateDefinition : SinglePropertyValueBasedCalendarItemStateDefinition<int>
	{
		public ResponseTypeBasedCalendarItemStateDefinition(ResponseType response) : base(CalendarItemBaseSchema.ResponseType, (int)response)
		{
		}

		public override string SchemaKey
		{
			get
			{
				return "{0144B45E-68EF-4a2f-8970-AA1A597EB048}";
			}
		}

		public override StorePropertyDefinition[] RequiredProperties
		{
			get
			{
				return ResponseTypeBasedCalendarItemStateDefinition.requiredProperties;
			}
		}

		private static readonly StorePropertyDefinition[] requiredProperties = new StorePropertyDefinition[]
		{
			CalendarItemBaseSchema.ClientIntent,
			CalendarItemBaseSchema.ResponseType
		};
	}
}
