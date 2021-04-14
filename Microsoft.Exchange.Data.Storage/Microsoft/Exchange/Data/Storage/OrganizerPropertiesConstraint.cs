using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OrganizerPropertiesConstraint : StoreObjectConstraint
	{
		internal OrganizerPropertiesConstraint() : base(new PropertyDefinition[]
		{
			InternalSchema.AppointmentStateInternal
		})
		{
		}

		internal override StoreObjectValidationError Validate(ValidationContext context, IValidatablePropertyBag validatablePropertyBag)
		{
			string text = validatablePropertyBag.TryGetProperty(InternalSchema.ItemClass) as string;
			if (!string.IsNullOrEmpty(text) && (ObjectClass.IsCalendarItem(text) || ObjectClass.IsRecurrenceException(text) || ObjectClass.IsMeetingMessage(text)) && validatablePropertyBag.IsPropertyDirty(InternalSchema.AppointmentStateInternal))
			{
				object obj = validatablePropertyBag.TryGetProperty(InternalSchema.AppointmentStateInternal);
				if (obj is int)
				{
					AppointmentStateFlags appointmentStateFlags = (AppointmentStateFlags)obj;
					if (EnumValidator<AppointmentStateFlags>.IsValidValue(appointmentStateFlags))
					{
						PropertyValueTrackingData originalPropertyInformation = validatablePropertyBag.GetOriginalPropertyInformation(InternalSchema.AppointmentStateInternal);
						if (originalPropertyInformation.PropertyValueState == PropertyTrackingInformation.Modified && originalPropertyInformation.OriginalPropertyValue != null && !PropertyError.IsPropertyNotFound(originalPropertyInformation.OriginalPropertyValue))
						{
							AppointmentStateFlags appointmentStateFlags2 = (AppointmentStateFlags)originalPropertyInformation.OriginalPropertyValue;
							bool flag = (appointmentStateFlags2 & AppointmentStateFlags.Received) == AppointmentStateFlags.None;
							bool flag2 = (appointmentStateFlags & AppointmentStateFlags.Received) == AppointmentStateFlags.None;
							if (flag != flag2)
							{
								return new StoreObjectValidationError(context, InternalSchema.AppointmentStateInternal, obj, this);
							}
						}
					}
				}
			}
			return null;
		}
	}
}
