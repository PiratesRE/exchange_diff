using System;
using System.Collections;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage.VersionedXml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Xml.Serialization.CalendarNotificationSettingsVersion1Point0
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class XmlSerializerContract : XmlSerializerImplementation
	{
		public override XmlSerializationReader Reader
		{
			get
			{
				return new XmlSerializationReaderCalendarNotificationSettingsVersion1Point0();
			}
		}

		public override XmlSerializationWriter Writer
		{
			get
			{
				return new XmlSerializationWriterCalendarNotificationSettingsVersion1Point0();
			}
		}

		public override Hashtable ReadMethods
		{
			get
			{
				if (this.readMethods == null)
				{
					Hashtable hashtable = new Hashtable();
					hashtable["Microsoft.Exchange.Data.Storage.VersionedXml.CalendarNotificationSettingsVersion1Point0::CalendarNotificationSettings:True:"] = "Read16_CalendarNotificationSettings";
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
					hashtable["Microsoft.Exchange.Data.Storage.VersionedXml.CalendarNotificationSettingsVersion1Point0::CalendarNotificationSettings:True:"] = "Write16_CalendarNotificationSettings";
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
					hashtable.Add("Microsoft.Exchange.Data.Storage.VersionedXml.CalendarNotificationSettingsVersion1Point0::CalendarNotificationSettings:True:", new CalendarNotificationSettingsVersion1Point0Serializer());
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
			return type == typeof(CalendarNotificationSettingsVersion1Point0);
		}

		public override XmlSerializer GetSerializer(Type type)
		{
			if (type == typeof(CalendarNotificationSettingsVersion1Point0))
			{
				return new CalendarNotificationSettingsVersion1Point0Serializer();
			}
			return null;
		}

		private Hashtable readMethods;

		private Hashtable writeMethods;

		private Hashtable typedSerializers;
	}
}
