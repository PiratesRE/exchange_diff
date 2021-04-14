using System;
using System.Collections;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage.VersionedXml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Xml.Serialization.CalendarNotificationContentVersion1Point0
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class XmlSerializerContract : XmlSerializerImplementation
	{
		public override XmlSerializationReader Reader
		{
			get
			{
				return new XmlSerializationReaderCalendarNotificationContentVersion1Point0();
			}
		}

		public override XmlSerializationWriter Writer
		{
			get
			{
				return new XmlSerializationWriterCalendarNotificationContentVersion1Point0();
			}
		}

		public override Hashtable ReadMethods
		{
			get
			{
				if (this.readMethods == null)
				{
					Hashtable hashtable = new Hashtable();
					hashtable["Microsoft.Exchange.Data.Storage.VersionedXml.CalendarNotificationContentVersion1Point0::CalendarNotificationContent:True:"] = "Read7_CalendarNotificationContent";
					if (this.readMethods == null)
					{
						this.readMethods = hashtable;
					}
				}
				return this.readMethods;
			}
		}

		public override Hashtable WriteMethods
		{
			get
			{
				if (this.writeMethods == null)
				{
					Hashtable hashtable = new Hashtable();
					hashtable["Microsoft.Exchange.Data.Storage.VersionedXml.CalendarNotificationContentVersion1Point0::CalendarNotificationContent:True:"] = "Write7_CalendarNotificationContent";
					if (this.writeMethods == null)
					{
						this.writeMethods = hashtable;
					}
				}
				return this.writeMethods;
			}
		}

		public override Hashtable TypedSerializers
		{
			get
			{
				if (this.typedSerializers == null)
				{
					Hashtable hashtable = new Hashtable();
					hashtable.Add("Microsoft.Exchange.Data.Storage.VersionedXml.CalendarNotificationContentVersion1Point0::CalendarNotificationContent:True:", new CalendarNotificationContentVersion1Point0Serializer());
					if (this.typedSerializers == null)
					{
						this.typedSerializers = hashtable;
					}
				}
				return this.typedSerializers;
			}
		}

		public override bool CanSerialize(Type type)
		{
			return type == typeof(CalendarNotificationContentVersion1Point0);
		}

		public override XmlSerializer GetSerializer(Type type)
		{
			if (type == typeof(CalendarNotificationContentVersion1Point0))
			{
				return new CalendarNotificationContentVersion1Point0Serializer();
			}
			return null;
		}

		private Hashtable readMethods;

		private Hashtable writeMethods;

		private Hashtable typedSerializers;
	}
}
