using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Entity
{
	[Serializable]
	internal class EntityAttendeesProperty : EntityProperty, IExtendedAttendeesProperty, IMultivaluedProperty<ExtendedAttendeeData>, IProperty, IEnumerable<ExtendedAttendeeData>, IEnumerable
	{
		public EntityAttendeesProperty() : base(SchematizedObject<EventSchema>.SchemaInstance.AttendeesProperty, PropertyType.ReadWrite, false)
		{
		}

		public int Count
		{
			get
			{
				if (this.values == null)
				{
					return 0;
				}
				return this.values.Count;
			}
		}

		public override void Bind(IItem item)
		{
			base.Bind(item);
			this.values = base.CalendaringEvent.Attendees;
		}

		public override void Unbind()
		{
			try
			{
				this.values = null;
			}
			finally
			{
				base.Unbind();
			}
		}

		public IEnumerator<ExtendedAttendeeData> GetEnumerator()
		{
			if (this.values != null)
			{
				foreach (Attendee attendee in this.values)
				{
					bool sendExtendedData = base.CalendaringEvent.IsOrganizer;
					ResponseStatus status = attendee.Status;
					ResponseType response = (status == null) ? ResponseType.None : status.Response;
					if (response != ResponseType.Organizer)
					{
						yield return new ExtendedAttendeeData(attendee.EmailAddress, attendee.Name, (int)response, (int)attendee.Type, sendExtendedData);
					}
				}
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public override void CopyFrom(IProperty srcProperty)
		{
			IExtendedAttendeesProperty extendedAttendeesProperty = srcProperty as IExtendedAttendeesProperty;
			if (extendedAttendeesProperty != null)
			{
				List<Attendee> list = new List<Attendee>(extendedAttendeesProperty.Count);
				foreach (ExtendedAttendeeData extendedAttendeeData in extendedAttendeesProperty)
				{
					list.Add(new Attendee
					{
						EmailAddress = extendedAttendeeData.EmailAddress,
						Name = extendedAttendeeData.DisplayName,
						Type = (AttendeeType)extendedAttendeeData.Type
					});
				}
				base.CalendaringEvent.Attendees = list;
			}
		}

		private IList<Attendee> values;
	}
}
