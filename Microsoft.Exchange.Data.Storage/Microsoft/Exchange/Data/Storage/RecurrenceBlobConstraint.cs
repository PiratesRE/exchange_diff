using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class RecurrenceBlobConstraint : StoreObjectConstraint
	{
		internal RecurrenceBlobConstraint() : base(new PropertyDefinition[]
		{
			InternalSchema.AppointmentRecurrenceBlob,
			InternalSchema.TimeZone,
			InternalSchema.TimeZoneBlob,
			InternalSchema.TimeZoneDefinitionRecurring
		})
		{
		}

		internal override StoreObjectValidationError Validate(ValidationContext context, IValidatablePropertyBag validatablePropertyBag)
		{
			if (ObjectClass.IsCalendarItem((validatablePropertyBag.TryGetProperty(InternalSchema.ItemClass) as string) ?? string.Empty))
			{
				PersistablePropertyBag persistablePropertyBag = validatablePropertyBag as PersistablePropertyBag;
				if (persistablePropertyBag != null)
				{
					byte[] largeBinaryProperty = persistablePropertyBag.GetLargeBinaryProperty(InternalSchema.AppointmentRecurrenceBlob);
					if (largeBinaryProperty != null)
					{
						ExTimeZone createTimeZone = TimeZoneHelper.GetRecurringTimeZoneFromPropertyBag(persistablePropertyBag) ?? ExTimeZone.CurrentTimeZone;
						try
						{
							InternalRecurrence.InternalParse(largeBinaryProperty, persistablePropertyBag.TryGetProperty(InternalSchema.ItemId) as VersionedId, createTimeZone, persistablePropertyBag.ExTimeZone, CalendarItem.DefaultCodePage);
						}
						catch (RecurrenceCalendarTypeNotSupportedException ex)
						{
							ExTraceGlobals.RecurrenceTracer.TraceDebug<CalendarType>((long)this.GetHashCode(), "RecurrenceBlobConstraint::Validate. Not supported calendar type found. CalendarType:{0}", ex.CalendarType);
						}
						catch (RecurrenceFormatException)
						{
							return new StoreObjectValidationError(context, InternalSchema.AppointmentRecurrenceBlob, largeBinaryProperty, this);
						}
					}
				}
			}
			return null;
		}
	}
}
