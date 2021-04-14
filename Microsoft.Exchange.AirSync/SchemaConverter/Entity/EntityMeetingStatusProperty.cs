using System;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Calendaring;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Entity
{
	[Serializable]
	internal class EntityMeetingStatusProperty : EntityProperty, IIntegerProperty, IProperty
	{
		public EntityMeetingStatusProperty() : base(PropertyType.ReadOnly, false)
		{
		}

		public virtual int IntegerData
		{
			get
			{
				AppointmentStateFlags appointmentStateFlags = AppointmentStateFlags.None;
				if (base.CalendaringEvent.IsCancelled)
				{
					appointmentStateFlags |= AppointmentStateFlags.Cancelled;
				}
				if (base.CalendaringEvent.HasAttendees)
				{
					appointmentStateFlags |= AppointmentStateFlags.Meeting;
				}
				if (((IEventInternal)base.CalendaringEvent).IsReceived)
				{
					appointmentStateFlags |= AppointmentStateFlags.Received;
				}
				return (int)appointmentStateFlags;
			}
		}

		public override void CopyFrom(IProperty srcProperty)
		{
			throw new InvalidOperationException("CopyFrom should not be called on a readonly EntityMeetingStatusProperty.");
		}
	}
}
