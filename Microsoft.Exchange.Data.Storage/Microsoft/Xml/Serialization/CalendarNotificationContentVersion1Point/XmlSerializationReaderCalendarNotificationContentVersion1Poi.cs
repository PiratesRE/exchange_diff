using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.VersionedXml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Xml.Serialization.CalendarNotificationContentVersion1Point0
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class XmlSerializationReaderCalendarNotificationContentVersion1Point0 : XmlSerializationReader
	{
		public object Read7_CalendarNotificationContent()
		{
			object result = null;
			base.Reader.MoveToContent();
			if (base.Reader.NodeType == XmlNodeType.Element)
			{
				if (base.Reader.LocalName != this.id1_CalendarNotificationContent || base.Reader.NamespaceURI != this.id2_Item)
				{
					throw base.CreateUnknownNodeException();
				}
				result = this.Read6_Item(true, true);
			}
			else
			{
				base.UnknownNode(null, ":CalendarNotificationContent");
			}
			return result;
		}

		private CalendarNotificationContentVersion1Point0 Read6_Item(bool isNullable, bool checkType)
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
			CalendarNotificationContentVersion1Point0 calendarNotificationContentVersion1Point = new CalendarNotificationContentVersion1Point0();
			if (calendarNotificationContentVersion1Point.CalEvents == null)
			{
				calendarNotificationContentVersion1Point.CalEvents = new List<CalendarEvent>();
			}
			List<CalendarEvent> calEvents = calendarNotificationContentVersion1Point.CalEvents;
			bool[] array = new bool[4];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!array[0] && base.Reader.LocalName == this.id4_Version && base.Reader.NamespaceURI == this.id2_Item)
				{
					calendarNotificationContentVersion1Point.Version = base.Reader.Value;
					array[0] = true;
				}
				else if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(calendarNotificationContentVersion1Point, ":Version");
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return calendarNotificationContentVersion1Point;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[1] && base.Reader.LocalName == this.id5_CalNotifType && base.Reader.NamespaceURI == this.id2_Item)
					{
						calendarNotificationContentVersion1Point.CalNotifType = this.Read4_CalendarNotificationType(base.Reader.ReadElementString());
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id6_CalNotifTypeDesc && base.Reader.NamespaceURI == this.id2_Item)
					{
						calendarNotificationContentVersion1Point.CalNotifTypeDesc = base.Reader.ReadElementString();
						array[2] = true;
					}
					else if (base.Reader.LocalName == this.id7_CalEvent && base.Reader.NamespaceURI == this.id2_Item)
					{
						if (calEvents == null)
						{
							base.Reader.Skip();
						}
						else
						{
							calEvents.Add(this.Read5_CalendarEvent(false, true));
						}
					}
					else
					{
						base.UnknownNode(calendarNotificationContentVersion1Point, ":CalNotifType, :CalNotifTypeDesc, :CalEvent");
					}
				}
				else
				{
					base.UnknownNode(calendarNotificationContentVersion1Point, ":CalNotifType, :CalNotifTypeDesc, :CalEvent");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return calendarNotificationContentVersion1Point;
		}

		private CalendarEvent Read5_CalendarEvent(bool isNullable, bool checkType)
		{
			XmlQualifiedName xmlQualifiedName = checkType ? base.GetXsiType() : null;
			bool flag = false;
			if (isNullable)
			{
				flag = base.ReadNull();
			}
			if (checkType && !(xmlQualifiedName == null) && (xmlQualifiedName.Name != this.id8_CalendarEvent || xmlQualifiedName.Namespace != this.id2_Item))
			{
				throw base.CreateUnknownTypeException(xmlQualifiedName);
			}
			if (flag)
			{
				return null;
			}
			CalendarEvent calendarEvent = new CalendarEvent();
			bool[] array = new bool[8];
			while (base.Reader.MoveToNextAttribute())
			{
				if (!base.IsXmlnsAttribute(base.Reader.Name))
				{
					base.UnknownNode(calendarEvent);
				}
			}
			base.Reader.MoveToElement();
			if (base.Reader.IsEmptyElement)
			{
				base.Reader.Skip();
				return calendarEvent;
			}
			base.Reader.ReadStartElement();
			base.Reader.MoveToContent();
			int num = 0;
			int readerCount = base.ReaderCount;
			while (base.Reader.NodeType != XmlNodeType.EndElement && base.Reader.NodeType != XmlNodeType.None)
			{
				if (base.Reader.NodeType == XmlNodeType.Element)
				{
					if (!array[0] && base.Reader.LocalName == this.id9_DayOfWeekOfStartTime && base.Reader.NamespaceURI == this.id2_Item)
					{
						calendarEvent.DayOfWeekOfStartTime = base.Reader.ReadElementString();
						array[0] = true;
					}
					else if (!array[1] && base.Reader.LocalName == this.id10_DateOfStartTime && base.Reader.NamespaceURI == this.id2_Item)
					{
						calendarEvent.DateOfStartTime = base.Reader.ReadElementString();
						array[1] = true;
					}
					else if (!array[2] && base.Reader.LocalName == this.id11_TimeOfStartTime && base.Reader.NamespaceURI == this.id2_Item)
					{
						calendarEvent.TimeOfStartTime = base.Reader.ReadElementString();
						array[2] = true;
					}
					else if (!array[3] && base.Reader.LocalName == this.id12_DayOfWeekOfEndTime && base.Reader.NamespaceURI == this.id2_Item)
					{
						calendarEvent.DayOfWeekOfEndTime = base.Reader.ReadElementString();
						array[3] = true;
					}
					else if (!array[4] && base.Reader.LocalName == this.id13_DateOfEndTime && base.Reader.NamespaceURI == this.id2_Item)
					{
						calendarEvent.DateOfEndTime = base.Reader.ReadElementString();
						array[4] = true;
					}
					else if (!array[5] && base.Reader.LocalName == this.id14_TimeOfEndTime && base.Reader.NamespaceURI == this.id2_Item)
					{
						calendarEvent.TimeOfEndTime = base.Reader.ReadElementString();
						array[5] = true;
					}
					else if (!array[6] && base.Reader.LocalName == this.id15_Subject && base.Reader.NamespaceURI == this.id2_Item)
					{
						calendarEvent.Subject = base.Reader.ReadElementString();
						array[6] = true;
					}
					else if (!array[7] && base.Reader.LocalName == this.id16_Location && base.Reader.NamespaceURI == this.id2_Item)
					{
						calendarEvent.Location = base.Reader.ReadElementString();
						array[7] = true;
					}
					else
					{
						base.UnknownNode(calendarEvent, ":DayOfWeekOfStartTime, :DateOfStartTime, :TimeOfStartTime, :DayOfWeekOfEndTime, :DateOfEndTime, :TimeOfEndTime, :Subject, :Location");
					}
				}
				else
				{
					base.UnknownNode(calendarEvent, ":DayOfWeekOfStartTime, :DateOfStartTime, :TimeOfStartTime, :DayOfWeekOfEndTime, :DateOfEndTime, :TimeOfEndTime, :Subject, :Location");
				}
				base.Reader.MoveToContent();
				base.CheckReaderCount(ref num, ref readerCount);
			}
			base.ReadEndElement();
			return calendarEvent;
		}

		private CalendarNotificationType Read4_CalendarNotificationType(string s)
		{
			switch (s)
			{
			case "Uninteresting":
				return CalendarNotificationType.Uninteresting;
			case "Summary":
				return CalendarNotificationType.Summary;
			case "Reminder":
				return CalendarNotificationType.Reminder;
			case "NewUpdate":
				return CalendarNotificationType.NewUpdate;
			case "ChangedUpdate":
				return CalendarNotificationType.ChangedUpdate;
			case "DeletedUpdate":
				return CalendarNotificationType.DeletedUpdate;
			}
			throw base.CreateUnknownConstantException(s, typeof(CalendarNotificationType));
		}

		protected override void InitCallbacks()
		{
		}

		protected override void InitIDs()
		{
			this.id3_Item = base.Reader.NameTable.Add("CalendarNotificationContentVersion1Point0");
			this.id14_TimeOfEndTime = base.Reader.NameTable.Add("TimeOfEndTime");
			this.id16_Location = base.Reader.NameTable.Add("Location");
			this.id11_TimeOfStartTime = base.Reader.NameTable.Add("TimeOfStartTime");
			this.id2_Item = base.Reader.NameTable.Add("");
			this.id9_DayOfWeekOfStartTime = base.Reader.NameTable.Add("DayOfWeekOfStartTime");
			this.id5_CalNotifType = base.Reader.NameTable.Add("CalNotifType");
			this.id12_DayOfWeekOfEndTime = base.Reader.NameTable.Add("DayOfWeekOfEndTime");
			this.id13_DateOfEndTime = base.Reader.NameTable.Add("DateOfEndTime");
			this.id1_CalendarNotificationContent = base.Reader.NameTable.Add("CalendarNotificationContent");
			this.id15_Subject = base.Reader.NameTable.Add("Subject");
			this.id6_CalNotifTypeDesc = base.Reader.NameTable.Add("CalNotifTypeDesc");
			this.id4_Version = base.Reader.NameTable.Add("Version");
			this.id7_CalEvent = base.Reader.NameTable.Add("CalEvent");
			this.id10_DateOfStartTime = base.Reader.NameTable.Add("DateOfStartTime");
			this.id8_CalendarEvent = base.Reader.NameTable.Add("CalendarEvent");
		}

		private string id3_Item;

		private string id14_TimeOfEndTime;

		private string id16_Location;

		private string id11_TimeOfStartTime;

		private string id2_Item;

		private string id9_DayOfWeekOfStartTime;

		private string id5_CalNotifType;

		private string id12_DayOfWeekOfEndTime;

		private string id13_DateOfEndTime;

		private string id1_CalendarNotificationContent;

		private string id15_Subject;

		private string id6_CalNotifTypeDesc;

		private string id4_Version;

		private string id7_CalEvent;

		private string id10_DateOfStartTime;

		private string id8_CalendarEvent;
	}
}
