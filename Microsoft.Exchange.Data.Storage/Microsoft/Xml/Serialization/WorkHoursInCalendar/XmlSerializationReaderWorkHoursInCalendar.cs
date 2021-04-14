using System;
using System.Collections;
using System.Reflection;
using System.Security;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Xml.Serialization.WorkHoursInCalendar
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class XmlSerializationReaderWorkHoursInCalendar : XmlSerializationReader
	{
		public object Read9_Root()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id1_Root || base.Reader.NamespaceURI != this.id2_WorkingHoursxsd)
				{
					throw base.CreateUnknownNodeException();
				}
				result = this.Read8_WorkHoursInCalendar(true, true);
			}
			else
			{
				base.UnknownNode(null, "WorkingHours.xsd:Root");
			}
			return result;
		}

		private WorkHoursInCalendar Read8_WorkHoursInCalendar(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id3_WorkHoursInCalendar || xmlQualifiedName.Namespace != this.id2_WorkingHoursxsd))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			WorkHoursInCalendar workHoursInCalendar;
			try
			{
				workHoursInCalendar = (WorkHoursInCalendar)Activator.CreateInstance(typeof(WorkHoursInCalendar), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, Array<object>.Empty, null);
			}
			catch (MissingMethodException)
			{
				throw base.CreateInaccessibleConstructorException("global::Microsoft.Exchange.Data.Storage.WorkHoursInCalendar");
			}
			catch (SecurityException)
			{
				throw base.CreateCtorHasSecurityException("global::Microsoft.Exchange.Data.Storage.WorkHoursInCalendar");
			}
			bool[] array = new bool[1];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(workHoursInCalendar);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return workHoursInCalendar;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id4_WorkHoursVersion1 && base.Reader.NamespaceURI == this.id2_WorkingHoursxsd)
					{
						workHoursInCalendar.WorkHoursVersion1 = this.Read7_WorkHoursVersion1(false, true);
						array[0] = true;
					}
					else
					{
						base.UnknownNode(workHoursInCalendar, "WorkingHours.xsd:WorkHoursVersion1");
					}
				}
				else
				{
					base.UnknownNode(workHoursInCalendar, "WorkingHours.xsd:WorkHoursVersion1");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return workHoursInCalendar;
		}

		private WorkHoursVersion1 Read7_WorkHoursVersion1(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id4_WorkHoursVersion1 || xmlQualifiedName.Namespace != this.id2_WorkingHoursxsd))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			WorkHoursVersion1 workHoursVersion = new WorkHoursVersion1();
			bool[] array = new bool[3];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(workHoursVersion);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return workHoursVersion;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id5_TimeZone && base.Reader.NamespaceURI == this.id2_WorkingHoursxsd)
					{
						workHoursVersion.TimeZone = this.Read4_WorkHoursTimeZone(false, true);
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id6_TimeSlot && base.Reader.NamespaceURI == this.id2_WorkingHoursxsd)
					{
						workHoursVersion.TimeSlot = this.Read5_TimeSlot(false, true);
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id7_WorkDays && base.Reader.NamespaceURI == this.id2_WorkingHoursxsd)
					{
						workHoursVersion.WorkDays = this.Read6_DaysOfWeek(base.Reader.ReadElementString());
						array[2] = true;
					}
					else
					{
						base.UnknownNode(workHoursVersion, "WorkingHours.xsd:TimeZone, WorkingHours.xsd:TimeSlot, WorkingHours.xsd:WorkDays");
					}
				}
				else
				{
					base.UnknownNode(workHoursVersion, "WorkingHours.xsd:TimeZone, WorkingHours.xsd:TimeSlot, WorkingHours.xsd:WorkDays");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return workHoursVersion;
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

		private DaysOfWeek Read6_DaysOfWeek(string s)
		{
			return (DaysOfWeek)XmlSerializationReader.ToEnum(s, this.DaysOfWeekValues, "global::Microsoft.Exchange.Data.DaysOfWeek");
		}

		private TimeSlot Read5_TimeSlot(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id6_TimeSlot || xmlQualifiedName.Namespace != this.id2_WorkingHoursxsd))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			TimeSlot timeSlot = new TimeSlot();
			bool[] array = new bool[2];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(timeSlot);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return timeSlot;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id8_Start && base.Reader.NamespaceURI == this.id2_WorkingHoursxsd)
					{
						timeSlot.Start = base.Reader.ReadElementString();
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id9_End && base.Reader.NamespaceURI == this.id2_WorkingHoursxsd)
					{
						timeSlot.End = base.Reader.ReadElementString();
						array[1] = true;
					}
					else
					{
						base.UnknownNode(timeSlot, "WorkingHours.xsd:Start, WorkingHours.xsd:End");
					}
				}
				else
				{
					base.UnknownNode(timeSlot, "WorkingHours.xsd:Start, WorkingHours.xsd:End");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return timeSlot;
		}

		private WorkHoursTimeZone Read4_WorkHoursTimeZone(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id10_WorkHoursTimeZone || xmlQualifiedName.Namespace != this.id2_WorkingHoursxsd))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			WorkHoursTimeZone workHoursTimeZone = new WorkHoursTimeZone();
			bool[] array = new bool[4];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(workHoursTimeZone);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return workHoursTimeZone;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id11_Bias && base.Reader.NamespaceURI == this.id2_WorkingHoursxsd)
					{
						workHoursTimeZone.Bias = XmlConvert.ToInt32(base.Reader.ReadElementString());
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id12_Standard && base.Reader.NamespaceURI == this.id2_WorkingHoursxsd)
					{
						workHoursTimeZone.Standard = this.Read3_ZoneTransition(false, true);
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id13_DaylightSavings && base.Reader.NamespaceURI == this.id2_WorkingHoursxsd)
					{
						workHoursTimeZone.DaylightSavings = this.Read3_ZoneTransition(false, true);
						array[2] = true;
					}
					else if (!array[3] && base.Reader.LocalName == this.id14_Name && base.Reader.NamespaceURI == this.id2_WorkingHoursxsd)
					{
						workHoursTimeZone.Name = base.Reader.ReadElementString();
						array[3] = true;
					}
					else
					{
						base.UnknownNode(workHoursTimeZone, "WorkingHours.xsd:Bias, WorkingHours.xsd:Standard, WorkingHours.xsd:DaylightSavings, WorkingHours.xsd:Name");
					}
				}
				else
				{
					base.UnknownNode(workHoursTimeZone, "WorkingHours.xsd:Bias, WorkingHours.xsd:Standard, WorkingHours.xsd:DaylightSavings, WorkingHours.xsd:Name");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return workHoursTimeZone;
		}

		private ZoneTransition Read3_ZoneTransition(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id15_ZoneTransition || xmlQualifiedName.Namespace != this.id2_WorkingHoursxsd))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			ZoneTransition zoneTransition = new ZoneTransition();
			bool[] array = new bool[2];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(zoneTransition);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return zoneTransition;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id11_Bias && base.Reader.NamespaceURI == this.id2_WorkingHoursxsd)
					{
						zoneTransition.Bias = XmlConvert.ToInt32(base.Reader.ReadElementString());
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id16_ChangeDate && base.Reader.NamespaceURI == this.id2_WorkingHoursxsd)
					{
						zoneTransition.ChangeDate = this.Read2_ChangeDate(false, true);
						array[1] = true;
					}
					else
					{
						base.UnknownNode(zoneTransition, "WorkingHours.xsd:Bias, WorkingHours.xsd:ChangeDate");
					}
				}
				else
				{
					base.UnknownNode(zoneTransition, "WorkingHours.xsd:Bias, WorkingHours.xsd:ChangeDate");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return zoneTransition;
		}

		private ChangeDate Read2_ChangeDate(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id16_ChangeDate || xmlQualifiedName.Namespace != this.id2_WorkingHoursxsd))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			ChangeDate changeDate = new ChangeDate();
			bool[] array = new bool[3];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(changeDate);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return changeDate;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id17_Time && base.Reader.NamespaceURI == this.id2_WorkingHoursxsd)
					{
						changeDate.Time = base.Reader.ReadElementString();
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id18_Date && base.Reader.NamespaceURI == this.id2_WorkingHoursxsd)
					{
						changeDate.Date = base.Reader.ReadElementString();
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id19_DayOfWeek && base.Reader.NamespaceURI == this.id2_WorkingHoursxsd)
					{
						changeDate.DayOfWeek = XmlConvert.ToInt16(base.Reader.ReadElementString());
						array[2] = true;
					}
					else
					{
						base.UnknownNode(changeDate, "WorkingHours.xsd:Time, WorkingHours.xsd:Date, WorkingHours.xsd:DayOfWeek");
					}
				}
				else
				{
					base.UnknownNode(changeDate, "WorkingHours.xsd:Time, WorkingHours.xsd:Date, WorkingHours.xsd:DayOfWeek");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return changeDate;
		}

		protected override void InitCallbacks()
		{
		}

		protected override void InitIDs()
		{
			this.id16_ChangeDate = base.Reader.NameTable.Add("ChangeDate");
			this.id2_WorkingHoursxsd = base.Reader.NameTable.Add("WorkingHours.xsd");
			this.id3_WorkHoursInCalendar = base.Reader.NameTable.Add("WorkHoursInCalendar");
			this.id18_Date = base.Reader.NameTable.Add("Date");
			this.id4_WorkHoursVersion1 = base.Reader.NameTable.Add("WorkHoursVersion1");
			this.id7_WorkDays = base.Reader.NameTable.Add("WorkDays");
			this.id8_Start = base.Reader.NameTable.Add("Start");
			this.id6_TimeSlot = base.Reader.NameTable.Add("TimeSlot");
			this.id15_ZoneTransition = base.Reader.NameTable.Add("ZoneTransition");
			this.id9_End = base.Reader.NameTable.Add("End");
			this.id10_WorkHoursTimeZone = base.Reader.NameTable.Add("WorkHoursTimeZone");
			this.id19_DayOfWeek = base.Reader.NameTable.Add("DayOfWeek");
			this.id14_Name = base.Reader.NameTable.Add("Name");
			this.id12_Standard = base.Reader.NameTable.Add("Standard");
			this.id13_DaylightSavings = base.Reader.NameTable.Add("DaylightSavings");
			this.id11_Bias = base.Reader.NameTable.Add("Bias");
			this.id17_Time = base.Reader.NameTable.Add("Time");
			this.id5_TimeZone = base.Reader.NameTable.Add("TimeZone");
			this.id1_Root = base.Reader.NameTable.Add("Root");
		}

		private Hashtable _DaysOfWeekValues;

		private string id16_ChangeDate;

		private string id2_WorkingHoursxsd;

		private string id3_WorkHoursInCalendar;

		private string id18_Date;

		private string id4_WorkHoursVersion1;

		private string id7_WorkDays;

		private string id8_Start;

		private string id6_TimeSlot;

		private string id15_ZoneTransition;

		private string id9_End;

		private string id10_WorkHoursTimeZone;

		private string id19_DayOfWeek;

		private string id14_Name;

		private string id12_Standard;

		private string id13_DaylightSavings;

		private string id11_Bias;

		private string id17_Time;

		private string id5_TimeZone;

		private string id1_Root;
	}
}
