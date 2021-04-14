using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.VersionedXml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Xml.Serialization.CalendarNotificationSettingsVersion1Point0
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class XmlSerializationReaderCalendarNotificationSettingsVersion1Point0 : XmlSerializationReader
	{
		public object Read16_CalendarNotificationSettings()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id1_CalendarNotificationSettings || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				result = this.Read15_Item(true, true);
			}
			else
			{
				base.UnknownNode(null, ":CalendarNotificationSettings");
			}
			return result;
		}

		private CalendarNotificationSettingsVersion1Point0 Read15_Item(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id3_Item || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			CalendarNotificationSettingsVersion1Point0 calendarNotificationSettingsVersion1Point = new CalendarNotificationSettingsVersion1Point0();
			if (calendarNotificationSettingsVersion1Point.Emitters == null)
			{
				calendarNotificationSettingsVersion1Point.Emitters = new List<Emitter>();
			}
			List<Emitter> emitters = calendarNotificationSettingsVersion1Point.Emitters;
			bool[] array = new bool[5];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!array[0] && base.Reader.LocalName == this.id4_Version && base.Reader.NamespaceURI == this.id2_Item)
				{
					calendarNotificationSettingsVersion1Point.Version = base.Reader.Value;
					array[0] = true;
				}
				else if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(calendarNotificationSettingsVersion1Point, ":Version");
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return calendarNotificationSettingsVersion1Point;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[1] && base.Reader.LocalName == this.id5_UpdateSettings && base.Reader.NamespaceURI == this.id2_Item)
					{
						calendarNotificationSettingsVersion1Point.UpdateSettings = this.Read7_TimeSlotMonitoringSettings(false, true);
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id6_ReminderSettings && base.Reader.NamespaceURI == this.id2_Item)
					{
						calendarNotificationSettingsVersion1Point.ReminderSettings = this.Read7_TimeSlotMonitoringSettings(false, true);
						array[2] = true;
					}
					else if (!array[3] && base.Reader.LocalName == this.id7_SummarySettings && base.Reader.NamespaceURI == this.id2_Item)
					{
						calendarNotificationSettingsVersion1Point.SummarySettings = this.Read12_TimePointScaningSettings(false, true);
						array[3] = true;
					}
					else if (base.Reader.LocalName == this.id8_Emitter && base.Reader.NamespaceURI == this.id2_Item)
					{
						if (emitters == null)
						{
							base.Reader.Skip();
						}
						else
						{
							emitters.Add(this.Read14_Emitter(false, true));
						}
					}
					else
					{
						base.UnknownNode(calendarNotificationSettingsVersion1Point, ":UpdateSettings, :ReminderSettings, :SummarySettings, :Emitter");
					}
				}
				else
				{
					base.UnknownNode(calendarNotificationSettingsVersion1Point, ":UpdateSettings, :ReminderSettings, :SummarySettings, :Emitter");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return calendarNotificationSettingsVersion1Point;
		}

		private Emitter Read14_Emitter(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id8_Emitter || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			Emitter emitter = new Emitter();
			if (emitter.PhoneNumbers == null)
			{
				emitter.PhoneNumbers = new List<E164Number>();
			}
			List<E164Number> phoneNumbers = emitter.PhoneNumbers;
			bool[] array = new bool[4];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(emitter);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return emitter;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id9_Type && base.Reader.NamespaceURI == this.id2_Item)
					{
						emitter.Type = this.Read13_EmitterType(base.Reader.ReadElementString());
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id10_Priority && base.Reader.NamespaceURI == this.id2_Item)
					{
						emitter.Priority = XmlConvert.ToInt32(base.Reader.ReadElementString());
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id11_Exclusive && base.Reader.NamespaceURI == this.id2_Item)
					{
						emitter.Exclusive = XmlConvert.ToBoolean(base.Reader.ReadElementString());
						array[2] = true;
					}
					else if (base.Reader.LocalName == this.id12_PhoneNumber && base.Reader.NamespaceURI == this.id2_Item)
					{
						phoneNumbers.Add((E164Number)base.ReadSerializable(new E164Number()));
					}
					else
					{
						base.UnknownNode(emitter, ":Type, :Priority, :Exclusive, :PhoneNumber");
					}
				}
				else
				{
					base.UnknownNode(emitter, ":Type, :Priority, :Exclusive, :PhoneNumber");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return emitter;
		}

		private EmitterType Read13_EmitterType(string s)
		{
			if (s != null)
			{
				if (s == "Unknown")
				{
					return EmitterType.Unknown;
				}
				if (s == "TextMessaging")
				{
					return EmitterType.TextMessaging;
				}
			}
			throw base.CreateUnknownConstantException(s, typeof(EmitterType));
		}

		private TimePointScaningSettings Read12_TimePointScaningSettings(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id13_TimePointScaningSettings || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			TimePointScaningSettings timePointScaningSettings = new TimePointScaningSettings();
			bool[] array = new bool[4];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(timePointScaningSettings);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return timePointScaningSettings;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id14_Enabled && base.Reader.NamespaceURI == this.id2_Item)
					{
						timePointScaningSettings.Enabled = XmlConvert.ToBoolean(base.Reader.ReadElementString());
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id15_NotifyingTimeInDay && base.Reader.NamespaceURI == this.id2_Item)
					{
						timePointScaningSettings.NotifyingTimeInDay = XmlSerializationReader.ToDateTime(base.Reader.ReadElementString());
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id16_Duration && base.Reader.NamespaceURI == this.id2_Item)
					{
						timePointScaningSettings.Duration = this.Read6_Duration(false, true);
						array[2] = true;
					}
					else if (!array[3] && base.Reader.LocalName == this.id17_Recurrence && base.Reader.NamespaceURI == this.id2_Item)
					{
						timePointScaningSettings.Recurrence = this.Read11_Recurrence(false, true);
						array[3] = true;
					}
					else
					{
						base.UnknownNode(timePointScaningSettings, ":Enabled, :NotifyingTimeInDay, :Duration, :Recurrence");
					}
				}
				else
				{
					base.UnknownNode(timePointScaningSettings, ":Enabled, :NotifyingTimeInDay, :Duration, :Recurrence");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return timePointScaningSettings;
		}

		private Recurrence Read11_Recurrence(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id17_Recurrence || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			Recurrence recurrence = new Recurrence();
			bool[] array = new bool[6];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(recurrence);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return recurrence;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id9_Type && base.Reader.NamespaceURI == this.id2_Item)
					{
						recurrence.Type = this.Read8_RecurrenceType(base.Reader.ReadElementString());
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id18_Interval && base.Reader.NamespaceURI == this.id2_Item)
					{
						recurrence.Interval = XmlConvert.ToUInt32(base.Reader.ReadElementString());
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id19_NthDayInMonth && base.Reader.NamespaceURI == this.id2_Item)
					{
						recurrence.NthDayInMonth = XmlConvert.ToUInt32(base.Reader.ReadElementString());
						array[2] = true;
					}
					else if (!array[3] && base.Reader.LocalName == this.id20_DaysOfWeek && base.Reader.NamespaceURI == this.id2_Item)
					{
						recurrence.DaysOfWeek = this.Read9_DaysOfWeek(base.Reader.ReadElementString());
						array[3] = true;
					}
					else if (!array[4] && base.Reader.LocalName == this.id21_WeekOrderInMonth && base.Reader.NamespaceURI == this.id2_Item)
					{
						recurrence.WeekOrderInMonth = this.Read10_WeekOrderInMonth(base.Reader.ReadElementString());
						array[4] = true;
					}
					else if (!array[5] && base.Reader.LocalName == this.id22_MonthOrder && base.Reader.NamespaceURI == this.id2_Item)
					{
						recurrence.MonthOrder = XmlConvert.ToUInt32(base.Reader.ReadElementString());
						array[5] = true;
					}
					else
					{
						base.UnknownNode(recurrence, ":Type, :Interval, :NthDayInMonth, :DaysOfWeek, :WeekOrderInMonth, :MonthOrder");
					}
				}
				else
				{
					base.UnknownNode(recurrence, ":Type, :Interval, :NthDayInMonth, :DaysOfWeek, :WeekOrderInMonth, :MonthOrder");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return recurrence;
		}

		private WeekOrderInMonth Read10_WeekOrderInMonth(string s)
		{
			switch (s)
			{
			case "None":
				return WeekOrderInMonth.None;
			case "First":
				return WeekOrderInMonth.First;
			case "Second":
				return WeekOrderInMonth.Second;
			case "Third":
				return WeekOrderInMonth.Third;
			case "Fourth":
				return WeekOrderInMonth.Fourth;
			case "Last":
				return WeekOrderInMonth.Last;
			}
			throw base.CreateUnknownConstantException(s, typeof(WeekOrderInMonth));
		}

		internal Hashtable DaysOfWeekValues
		{
			get
			{
				if (this._DaysOfWeekValues == null)
				{
					this._DaysOfWeekValues = new Hashtable
					{
						{
							"None",
							0L
						},
						{
							"Sunday",
							1L
						},
						{
							"Monday",
							2L
						},
						{
							"Tuesday",
							4L
						},
						{
							"Wednesday",
							8L
						},
						{
							"Thursday",
							16L
						},
						{
							"Friday",
							32L
						},
						{
							"Saturday",
							64L
						},
						{
							"Weekdays",
							62L
						},
						{
							"WeekendDays",
							65L
						},
						{
							"AllDays",
							127L
						}
					};
				}
				return this._DaysOfWeekValues;
			}
		}

		private DaysOfWeek Read9_DaysOfWeek(string s)
		{
			return (DaysOfWeek)XmlSerializationReader.ToEnum(s, this.DaysOfWeekValues, "global::Microsoft.Exchange.Data.Storage.VersionedXml.DaysOfWeek");
		}

		private RecurrenceType Read8_RecurrenceType(string s)
		{
			switch (s)
			{
			case "Unknown":
				return RecurrenceType.Unknown;
			case "Daily":
				return RecurrenceType.Daily;
			case "Weekly":
				return RecurrenceType.Weekly;
			case "Monthly":
				return RecurrenceType.Monthly;
			case "MonthlyTh":
				return RecurrenceType.MonthlyTh;
			case "Yearly":
				return RecurrenceType.Yearly;
			case "YearlyTh":
				return RecurrenceType.YearlyTh;
			}
			throw base.CreateUnknownConstantException(s, typeof(RecurrenceType));
		}

		private Duration Read6_Duration(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id16_Duration || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			Duration duration = new Duration();
			bool[] array = new bool[6];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(duration);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return duration;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id9_Type && base.Reader.NamespaceURI == this.id2_Item)
					{
						duration.Type = this.Read5_DurationType(base.Reader.ReadElementString());
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id18_Interval && base.Reader.NamespaceURI == this.id2_Item)
					{
						duration.Interval = XmlConvert.ToUInt32(base.Reader.ReadElementString());
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id23_UseWorkHoursTimeSlot && base.Reader.NamespaceURI == this.id2_Item)
					{
						duration.UseWorkHoursTimeSlot = XmlConvert.ToBoolean(base.Reader.ReadElementString());
						array[2] = true;
					}
					else if (!array[3] && base.Reader.LocalName == this.id24_StartTimeInDay && base.Reader.NamespaceURI == this.id2_Item)
					{
						duration.StartTimeInDay = XmlSerializationReader.ToDateTime(base.Reader.ReadElementString());
						array[3] = true;
					}
					else if (!array[4] && base.Reader.LocalName == this.id25_EndTimeInDay && base.Reader.NamespaceURI == this.id2_Item)
					{
						duration.EndTimeInDay = XmlSerializationReader.ToDateTime(base.Reader.ReadElementString());
						array[4] = true;
					}
					else if (!array[5] && base.Reader.LocalName == this.id26_NonWorkHoursExcluded && base.Reader.NamespaceURI == this.id2_Item)
					{
						duration.NonWorkHoursExcluded = XmlConvert.ToBoolean(base.Reader.ReadElementString());
						array[5] = true;
					}
					else
					{
						base.UnknownNode(duration, ":Type, :Interval, :UseWorkHoursTimeSlot, :StartTimeInDay, :EndTimeInDay, :NonWorkHoursExcluded");
					}
				}
				else
				{
					base.UnknownNode(duration, ":Type, :Interval, :UseWorkHoursTimeSlot, :StartTimeInDay, :EndTimeInDay, :NonWorkHoursExcluded");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return duration;
		}

		private DurationType Read5_DurationType(string s)
		{
			if (s != null)
			{
				if (s == "Unknown")
				{
					return DurationType.Unknown;
				}
				if (s == "Days")
				{
					return DurationType.Days;
				}
				if (s == "Weeks")
				{
					return DurationType.Weeks;
				}
				if (s == "Months")
				{
					return DurationType.Months;
				}
				if (s == "Years")
				{
					return DurationType.Years;
				}
			}
			throw base.CreateUnknownConstantException(s, typeof(DurationType));
		}

		private TimeSlotMonitoringSettings Read7_TimeSlotMonitoringSettings(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id27_TimeSlotMonitoringSettings || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			TimeSlotMonitoringSettings timeSlotMonitoringSettings = new TimeSlotMonitoringSettings();
			bool[] array = new bool[5];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(timeSlotMonitoringSettings);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return timeSlotMonitoringSettings;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id14_Enabled && base.Reader.NamespaceURI == this.id2_Item)
					{
						timeSlotMonitoringSettings.Enabled = XmlConvert.ToBoolean(base.Reader.ReadElementString());
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id28_NotifyInWorkHoursTimeSlot && base.Reader.NamespaceURI == this.id2_Item)
					{
						timeSlotMonitoringSettings.NotifyInWorkHoursTimeSlot = XmlConvert.ToBoolean(base.Reader.ReadElementString());
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id29_NotifyingStartTimeInDay && base.Reader.NamespaceURI == this.id2_Item)
					{
						timeSlotMonitoringSettings.NotifyingStartTimeInDay = XmlSerializationReader.ToDateTime(base.Reader.ReadElementString());
						array[2] = true;
					}
					else if (!array[3] && base.Reader.LocalName == this.id30_NotifyingEndTimeInDay && base.Reader.NamespaceURI == this.id2_Item)
					{
						timeSlotMonitoringSettings.NotifyingEndTimeInDay = XmlSerializationReader.ToDateTime(base.Reader.ReadElementString());
						array[3] = true;
					}
					else if (!array[4] && base.Reader.LocalName == this.id16_Duration && base.Reader.NamespaceURI == this.id2_Item)
					{
						timeSlotMonitoringSettings.Duration = this.Read6_Duration(false, true);
						array[4] = true;
					}
					else
					{
						base.UnknownNode(timeSlotMonitoringSettings, ":Enabled, :NotifyInWorkHoursTimeSlot, :NotifyingStartTimeInDay, :NotifyingEndTimeInDay, :Duration");
					}
				}
				else
				{
					base.UnknownNode(timeSlotMonitoringSettings, ":Enabled, :NotifyInWorkHoursTimeSlot, :NotifyingStartTimeInDay, :NotifyingEndTimeInDay, :Duration");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return timeSlotMonitoringSettings;
		}

		protected override void InitCallbacks()
		{
		}

		protected override void InitIDs()
		{
			this.id8_Emitter = base.Reader.NameTable.Add("Emitter");
			this.id18_Interval = base.Reader.NameTable.Add("Interval");
			this.id16_Duration = base.Reader.NameTable.Add("Duration");
			this.id14_Enabled = base.Reader.NameTable.Add("Enabled");
			this.id28_NotifyInWorkHoursTimeSlot = base.Reader.NameTable.Add("NotifyInWorkHoursTimeSlot");
			this.id4_Version = base.Reader.NameTable.Add("Version");
			this.id22_MonthOrder = base.Reader.NameTable.Add("MonthOrder");
			this.id3_Item = base.Reader.NameTable.Add("CalendarNotificationSettingsVersion1Point0");
			this.id7_SummarySettings = base.Reader.NameTable.Add("SummarySettings");
			this.id30_NotifyingEndTimeInDay = base.Reader.NameTable.Add("NotifyingEndTimeInDay");
			this.id21_WeekOrderInMonth = base.Reader.NameTable.Add("WeekOrderInMonth");
			this.id23_UseWorkHoursTimeSlot = base.Reader.NameTable.Add("UseWorkHoursTimeSlot");
			this.id6_ReminderSettings = base.Reader.NameTable.Add("ReminderSettings");
			this.id5_UpdateSettings = base.Reader.NameTable.Add("UpdateSettings");
			this.id9_Type = base.Reader.NameTable.Add("Type");
			this.id25_EndTimeInDay = base.Reader.NameTable.Add("EndTimeInDay");
			this.id20_DaysOfWeek = base.Reader.NameTable.Add("DaysOfWeek");
			this.id2_Item = base.Reader.NameTable.Add("");
			this.id19_NthDayInMonth = base.Reader.NameTable.Add("NthDayInMonth");
			this.id1_CalendarNotificationSettings = base.Reader.NameTable.Add("CalendarNotificationSettings");
			this.id17_Recurrence = base.Reader.NameTable.Add("Recurrence");
			this.id29_NotifyingStartTimeInDay = base.Reader.NameTable.Add("NotifyingStartTimeInDay");
			this.id15_NotifyingTimeInDay = base.Reader.NameTable.Add("NotifyingTimeInDay");
			this.id13_TimePointScaningSettings = base.Reader.NameTable.Add("TimePointScaningSettings");
			this.id26_NonWorkHoursExcluded = base.Reader.NameTable.Add("NonWorkHoursExcluded");
			this.id24_StartTimeInDay = base.Reader.NameTable.Add("StartTimeInDay");
			this.id10_Priority = base.Reader.NameTable.Add("Priority");
			this.id27_TimeSlotMonitoringSettings = base.Reader.NameTable.Add("TimeSlotMonitoringSettings");
			this.id12_PhoneNumber = base.Reader.NameTable.Add("PhoneNumber");
			this.id11_Exclusive = base.Reader.NameTable.Add("Exclusive");
		}

		private Hashtable _DaysOfWeekValues;

		private string id8_Emitter;

		private string id18_Interval;

		private string id16_Duration;

		private string id14_Enabled;

		private string id28_NotifyInWorkHoursTimeSlot;

		private string id4_Version;

		private string id22_MonthOrder;

		private string id3_Item;

		private string id7_SummarySettings;

		private string id30_NotifyingEndTimeInDay;

		private string id21_WeekOrderInMonth;

		private string id23_UseWorkHoursTimeSlot;

		private string id6_ReminderSettings;

		private string id5_UpdateSettings;

		private string id9_Type;

		private string id25_EndTimeInDay;

		private string id20_DaysOfWeek;

		private string id2_Item;

		private string id19_NthDayInMonth;

		private string id1_CalendarNotificationSettings;

		private string id17_Recurrence;

		private string id29_NotifyingStartTimeInDay;

		private string id15_NotifyingTimeInDay;

		private string id13_TimePointScaningSettings;

		private string id26_NonWorkHoursExcluded;

		private string id24_StartTimeInDay;

		private string id10_Priority;

		private string id27_TimeSlotMonitoringSettings;

		private string id12_PhoneNumber;

		private string id11_Exclusive;
	}
}
