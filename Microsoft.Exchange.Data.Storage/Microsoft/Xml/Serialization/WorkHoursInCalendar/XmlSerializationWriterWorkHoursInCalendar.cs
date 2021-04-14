using System;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Xml.Serialization.WorkHoursInCalendar
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class XmlSerializationWriterWorkHoursInCalendar : XmlSerializationWriter
	{
		public void Write9_Root(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteNullTagLiteral("Root", "WorkingHours.xsd");
				return;
			}
			base.TopLevelElement();
			this.Write8_WorkHoursInCalendar("Root", "WorkingHours.xsd", (WorkHoursInCalendar)o, true, false);
		}

		private void Write8_WorkHoursInCalendar(string n, string ns, WorkHoursInCalendar o, bool isNullable, bool needType)
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
				if (!(type == typeof(WorkHoursInCalendar)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("WorkHoursInCalendar", "WorkingHours.xsd");
			}
			this.Write7_WorkHoursVersion1("WorkHoursVersion1", "WorkingHours.xsd", o.WorkHoursVersion1, false, false);
			base.WriteEndElement(o);
		}

		private void Write7_WorkHoursVersion1(string n, string ns, WorkHoursVersion1 o, bool isNullable, bool needType)
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
				if (!(type == typeof(WorkHoursVersion1)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("WorkHoursVersion1", "WorkingHours.xsd");
			}
			this.Write4_WorkHoursTimeZone("TimeZone", "WorkingHours.xsd", o.TimeZone, false, false);
			this.Write5_TimeSlot("TimeSlot", "WorkingHours.xsd", o.TimeSlot, false, false);
			base.WriteElementString("WorkDays", "WorkingHours.xsd", this.Write6_DaysOfWeek(o.WorkDays));
			base.WriteEndElement(o);
		}

		private string Write6_DaysOfWeek(DaysOfWeek v)
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
			}, "Microsoft.Exchange.Data.DaysOfWeek");
		}

		private void Write5_TimeSlot(string n, string ns, TimeSlot o, bool isNullable, bool needType)
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
				if (!(type == typeof(TimeSlot)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("TimeSlot", "WorkingHours.xsd");
			}
			base.WriteElementString("Start", "WorkingHours.xsd", o.Start);
			base.WriteElementString("End", "WorkingHours.xsd", o.End);
			base.WriteEndElement(o);
		}

		private void Write4_WorkHoursTimeZone(string n, string ns, WorkHoursTimeZone o, bool isNullable, bool needType)
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
				if (!(type == typeof(WorkHoursTimeZone)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("WorkHoursTimeZone", "WorkingHours.xsd");
			}
			base.WriteElementStringRaw("Bias", "WorkingHours.xsd", XmlConvert.ToString(o.Bias));
			this.Write3_ZoneTransition("Standard", "WorkingHours.xsd", o.Standard, false, false);
			this.Write3_ZoneTransition("DaylightSavings", "WorkingHours.xsd", o.DaylightSavings, false, false);
			base.WriteElementString("Name", "WorkingHours.xsd", o.Name);
			base.WriteEndElement(o);
		}

		private void Write3_ZoneTransition(string n, string ns, ZoneTransition o, bool isNullable, bool needType)
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
				if (!(type == typeof(ZoneTransition)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("ZoneTransition", "WorkingHours.xsd");
			}
			base.WriteElementStringRaw("Bias", "WorkingHours.xsd", XmlConvert.ToString(o.Bias));
			this.Write2_ChangeDate("ChangeDate", "WorkingHours.xsd", o.ChangeDate, false, false);
			base.WriteEndElement(o);
		}

		private void Write2_ChangeDate(string n, string ns, ChangeDate o, bool isNullable, bool needType)
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
				if (!(type == typeof(ChangeDate)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("ChangeDate", "WorkingHours.xsd");
			}
			base.WriteElementString("Time", "WorkingHours.xsd", o.Time);
			base.WriteElementString("Date", "WorkingHours.xsd", o.Date);
			base.WriteElementStringRaw("DayOfWeek", "WorkingHours.xsd", XmlConvert.ToString(o.DayOfWeek));
			base.WriteEndElement(o);
		}

		protected override void InitCallbacks()
		{
		}
	}
}
