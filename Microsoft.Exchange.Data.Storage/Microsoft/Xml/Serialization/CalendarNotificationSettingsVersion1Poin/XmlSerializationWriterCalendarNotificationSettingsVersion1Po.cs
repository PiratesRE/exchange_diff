using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.VersionedXml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Xml.Serialization.CalendarNotificationSettingsVersion1Point0
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class XmlSerializationWriterCalendarNotificationSettingsVersion1Point0 : XmlSerializationWriter
	{
		public void Write16_CalendarNotificationSettings(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteNullTagLiteral("CalendarNotificationSettings", "");
				return;
			}
			base.TopLevelElement();
			this.Write15_Item("CalendarNotificationSettings", "", (CalendarNotificationSettingsVersion1Point0)o, true, false);
		}

		private void Write15_Item(string n, string ns, CalendarNotificationSettingsVersion1Point0 o, bool isNullable, bool needType)
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
				if (!(type == typeof(CalendarNotificationSettingsVersion1Point0)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("CalendarNotificationSettingsVersion1Point0", "");
			}
			base.WriteAttribute("Version", "", o.Version);
			this.Write7_TimeSlotMonitoringSettings("UpdateSettings", "", o.UpdateSettings, false, false);
			this.Write7_TimeSlotMonitoringSettings("ReminderSettings", "", o.ReminderSettings, false, false);
			this.Write12_TimePointScaningSettings("SummarySettings", "", o.SummarySettings, false, false);
			List<Emitter> emitters = o.Emitters;
			if (emitters != null)
			{
				for (int i = 0; i < ((ICollection)emitters).Count; i++)
				{
					this.Write14_Emitter("Emitter", "", emitters[i], false, false);
				}
			}
			base.WriteEndElement(o);
		}

		private void Write14_Emitter(string n, string ns, Emitter o, bool isNullable, bool needType)
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
				if (!(type == typeof(Emitter)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("Emitter", "");
			}
			base.WriteElementString("Type", "", this.Write13_EmitterType(o.Type));
			base.WriteElementStringRaw("Priority", "", XmlConvert.ToString(o.Priority));
			base.WriteElementStringRaw("Exclusive", "", XmlConvert.ToString(o.Exclusive));
			List<E164Number> phoneNumbers = o.PhoneNumbers;
			if (phoneNumbers != null)
			{
				for (int i = 0; i < ((ICollection)phoneNumbers).Count; i++)
				{
					base.WriteSerializable(phoneNumbers[i], "PhoneNumber", "", false, true);
				}
			}
			base.WriteEndElement(o);
		}

		private string Write13_EmitterType(EmitterType v)
		{
			string result;
			switch (v)
			{
			case EmitterType.Unknown:
				result = "Unknown";
				break;
			case EmitterType.TextMessaging:
				result = "TextMessaging";
				break;
			default:
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Data.Storage.VersionedXml.EmitterType");
			}
			return result;
		}

		private void Write12_TimePointScaningSettings(string n, string ns, TimePointScaningSettings o, bool isNullable, bool needType)
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
				if (!(type == typeof(TimePointScaningSettings)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("TimePointScaningSettings", "");
			}
			base.WriteElementStringRaw("Enabled", "", XmlConvert.ToString(o.Enabled));
			base.WriteElementStringRaw("NotifyingTimeInDay", "", XmlSerializationWriter.FromDateTime(o.NotifyingTimeInDay));
			this.Write6_Duration("Duration", "", o.Duration, false, false);
			this.Write11_Recurrence("Recurrence", "", o.Recurrence, false, false);
			base.WriteEndElement(o);
		}

		private void Write11_Recurrence(string n, string ns, Recurrence o, bool isNullable, bool needType)
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
				if (!(type == typeof(Recurrence)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("Recurrence", "");
			}
			base.WriteElementString("Type", "", this.Write8_RecurrenceType(o.Type));
			base.WriteElementStringRaw("Interval", "", XmlConvert.ToString(o.Interval));
			base.WriteElementStringRaw("NthDayInMonth", "", XmlConvert.ToString(o.NthDayInMonth));
			base.WriteElementString("DaysOfWeek", "", this.Write9_DaysOfWeek(o.DaysOfWeek));
			base.WriteElementString("WeekOrderInMonth", "", this.Write10_WeekOrderInMonth(o.WeekOrderInMonth));
			base.WriteElementStringRaw("MonthOrder", "", XmlConvert.ToString(o.MonthOrder));
			base.WriteEndElement(o);
		}

		private string Write10_WeekOrderInMonth(WeekOrderInMonth v)
		{
			string result;
			switch (v)
			{
			case WeekOrderInMonth.Last:
				result = "Last";
				break;
			case WeekOrderInMonth.None:
				result = "None";
				break;
			case WeekOrderInMonth.First:
				result = "First";
				break;
			case WeekOrderInMonth.Second:
				result = "Second";
				break;
			case WeekOrderInMonth.Third:
				result = "Third";
				break;
			case WeekOrderInMonth.Fourth:
				result = "Fourth";
				break;
			default:
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Data.Storage.VersionedXml.WeekOrderInMonth");
			}
			return result;
		}

		private string Write9_DaysOfWeek(DaysOfWeek v)
		{
			if (v <= DaysOfWeek.Thursday)
			{
				switch (v)
				{
				case DaysOfWeek.None:
					return "None";
				case DaysOfWeek.Sunday:
					return "Sunday";
				case DaysOfWeek.Monday:
					return "Monday";
				case DaysOfWeek.Sunday | DaysOfWeek.Monday:
				case DaysOfWeek.Sunday | DaysOfWeek.Tuesday:
				case DaysOfWeek.Monday | DaysOfWeek.Tuesday:
				case DaysOfWeek.Sunday | DaysOfWeek.Monday | DaysOfWeek.Tuesday:
					break;
				case DaysOfWeek.Tuesday:
					return "Tuesday";
				case DaysOfWeek.Wednesday:
					return "Wednesday";
				default:
					if (v == DaysOfWeek.Thursday)
					{
						return "Thursday";
					}
					break;
				}
			}
			else
			{
				if (v == DaysOfWeek.Friday)
				{
					return "Friday";
				}
				switch (v)
				{
				case DaysOfWeek.Weekdays:
					return "Weekdays";
				case DaysOfWeek.Sunday | DaysOfWeek.Monday | DaysOfWeek.Tuesday | DaysOfWeek.Wednesday | DaysOfWeek.Thursday | DaysOfWeek.Friday:
					break;
				case DaysOfWeek.Saturday:
					return "Saturday";
				case DaysOfWeek.WeekendDays:
					return "WeekendDays";
				default:
					if (v == DaysOfWeek.AllDays)
					{
						return "AllDays";
					}
					break;
				}
			}
			return XmlSerializationWriter.FromEnum((long)v, new string[]
			{
				"None",
				"Sunday",
				"Monday",
				"Tuesday",
				"Wednesday",
				"Thursday",
				"Friday",
				"Saturday",
				"Weekdays",
				"WeekendDays",
				"AllDays"
			}, new long[]
			{
				0L,
				1L,
				2L,
				4L,
				8L,
				16L,
				32L,
				64L,
				62L,
				65L,
				127L
			}, "Microsoft.Exchange.Data.Storage.VersionedXml.DaysOfWeek");
		}

		private string Write8_RecurrenceType(RecurrenceType v)
		{
			string result;
			switch (v)
			{
			case RecurrenceType.Unknown:
				result = "Unknown";
				break;
			case RecurrenceType.Daily:
				result = "Daily";
				break;
			case RecurrenceType.Weekly:
				result = "Weekly";
				break;
			case RecurrenceType.Monthly:
				result = "Monthly";
				break;
			case RecurrenceType.MonthlyTh:
				result = "MonthlyTh";
				break;
			case RecurrenceType.Yearly:
				result = "Yearly";
				break;
			case RecurrenceType.YearlyTh:
				result = "YearlyTh";
				break;
			default:
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Data.Storage.VersionedXml.RecurrenceType");
			}
			return result;
		}

		private void Write6_Duration(string n, string ns, Duration o, bool isNullable, bool needType)
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
				if (!(type == typeof(Duration)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("Duration", "");
			}
			base.WriteElementString("Type", "", this.Write5_DurationType(o.Type));
			base.WriteElementStringRaw("Interval", "", XmlConvert.ToString(o.Interval));
			base.WriteElementStringRaw("UseWorkHoursTimeSlot", "", XmlConvert.ToString(o.UseWorkHoursTimeSlot));
			base.WriteElementStringRaw("StartTimeInDay", "", XmlSerializationWriter.FromDateTime(o.StartTimeInDay));
			base.WriteElementStringRaw("EndTimeInDay", "", XmlSerializationWriter.FromDateTime(o.EndTimeInDay));
			base.WriteElementStringRaw("NonWorkHoursExcluded", "", XmlConvert.ToString(o.NonWorkHoursExcluded));
			base.WriteEndElement(o);
		}

		private string Write5_DurationType(DurationType v)
		{
			string result;
			switch (v)
			{
			case DurationType.Unknown:
				result = "Unknown";
				break;
			case DurationType.Days:
				result = "Days";
				break;
			case DurationType.Weeks:
				result = "Weeks";
				break;
			case DurationType.Months:
				result = "Months";
				break;
			case DurationType.Years:
				result = "Years";
				break;
			default:
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Data.Storage.VersionedXml.DurationType");
			}
			return result;
		}

		private void Write7_TimeSlotMonitoringSettings(string n, string ns, TimeSlotMonitoringSettings o, bool isNullable, bool needType)
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
				if (!(type == typeof(TimeSlotMonitoringSettings)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("TimeSlotMonitoringSettings", "");
			}
			base.WriteElementStringRaw("Enabled", "", XmlConvert.ToString(o.Enabled));
			base.WriteElementStringRaw("NotifyInWorkHoursTimeSlot", "", XmlConvert.ToString(o.NotifyInWorkHoursTimeSlot));
			base.WriteElementStringRaw("NotifyingStartTimeInDay", "", XmlSerializationWriter.FromDateTime(o.NotifyingStartTimeInDay));
			base.WriteElementStringRaw("NotifyingEndTimeInDay", "", XmlSerializationWriter.FromDateTime(o.NotifyingEndTimeInDay));
			this.Write6_Duration("Duration", "", o.Duration, false, false);
			base.WriteEndElement(o);
		}

		protected override void InitCallbacks()
		{
		}
	}
}
