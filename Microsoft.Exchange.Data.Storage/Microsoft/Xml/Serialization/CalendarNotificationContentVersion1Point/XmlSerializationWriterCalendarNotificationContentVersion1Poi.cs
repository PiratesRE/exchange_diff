using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.VersionedXml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Xml.Serialization.CalendarNotificationContentVersion1Point0
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class XmlSerializationWriterCalendarNotificationContentVersion1Point0 : XmlSerializationWriter
	{
		public void Write7_CalendarNotificationContent(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteNullTagLiteral("CalendarNotificationContent", "");
				return;
			}
			base.TopLevelElement();
			this.Write6_Item("CalendarNotificationContent", "", (CalendarNotificationContentVersion1Point0)o, true, false);
		}

		private void Write6_Item(string n, string ns, CalendarNotificationContentVersion1Point0 o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(CalendarNotificationContentVersion1Point0)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("CalendarNotificationContentVersion1Point0", "");
			}
			base.WriteAttribute("Version", "", o.Version);
			base.WriteElementString("CalNotifType", "", this.Write4_CalendarNotificationType(o.CalNotifType));
			base.WriteElementString("CalNotifTypeDesc", "", o.CalNotifTypeDesc);
			List<CalendarEvent> calEvents = o.CalEvents;
			if (calEvents != null)
			{
				for (int i = 0; i < ((ICollection)calEvents).Count; i++)
				{
					this.Write5_CalendarEvent("CalEvent", "", calEvents[i], false, false);
				}
			}
			base.WriteEndElement(o);
		}

		private void Write5_CalendarEvent(string n, string ns, CalendarEvent o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(CalendarEvent)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("CalendarEvent", "");
			}
			base.WriteElementString("DayOfWeekOfStartTime", "", o.DayOfWeekOfStartTime);
			base.WriteElementString("DateOfStartTime", "", o.DateOfStartTime);
			base.WriteElementString("TimeOfStartTime", "", o.TimeOfStartTime);
			base.WriteElementString("DayOfWeekOfEndTime", "", o.DayOfWeekOfEndTime);
			base.WriteElementString("DateOfEndTime", "", o.DateOfEndTime);
			base.WriteElementString("TimeOfEndTime", "", o.TimeOfEndTime);
			base.WriteElementString("Subject", "", o.Subject);
			base.WriteElementString("Location", "", o.Location);
			base.WriteEndElement(o);
		}

		private string Write4_CalendarNotificationType(CalendarNotificationType v)
		{
			string result;
			switch (v)
			{
			case CalendarNotificationType.Uninteresting:
				result = "Uninteresting";
				break;
			case CalendarNotificationType.Summary:
				result = "Summary";
				break;
			case CalendarNotificationType.Reminder:
				result = "Reminder";
				break;
			case CalendarNotificationType.NewUpdate:
				result = "NewUpdate";
				break;
			case CalendarNotificationType.ChangedUpdate:
				result = "ChangedUpdate";
				break;
			case CalendarNotificationType.DeletedUpdate:
				result = "DeletedUpdate";
				break;
			default:
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Data.Storage.Management.CalendarNotificationType");
			}
			return result;
		}

		protected override void InitCallbacks()
		{
		}
	}
}
