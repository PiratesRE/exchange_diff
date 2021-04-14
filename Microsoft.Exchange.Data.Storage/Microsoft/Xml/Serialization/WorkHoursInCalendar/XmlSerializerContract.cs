using System;
using System.Collections;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Xml.Serialization.WorkHoursInCalendar
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class XmlSerializerContract : XmlSerializerImplementation
	{
		public override XmlSerializationReader Reader
		{
			get
			{
				return new XmlSerializationReaderWorkHoursInCalendar();
			}
		}

		public override XmlSerializationWriter Writer
		{
			get
			{
				return new XmlSerializationWriterWorkHoursInCalendar();
			}
		}

		public override Hashtable ReadMethods
		{
			get
			{
				if (this.readMethods == null)
				{
					Hashtable hashtable = new Hashtable();
					hashtable["Microsoft.Exchange.Data.Storage.WorkHoursInCalendar:WorkingHours.xsd:Root:True:"] = "Read9_Root";
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
					hashtable["Microsoft.Exchange.Data.Storage.WorkHoursInCalendar:WorkingHours.xsd:Root:True:"] = "Write9_Root";
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
					hashtable.Add("Microsoft.Exchange.Data.Storage.WorkHoursInCalendar:WorkingHours.xsd:Root:True:", new WorkHoursInCalendarSerializer());
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
			return type == typeof(WorkHoursInCalendar);
		}

		public override XmlSerializer GetSerializer(Type type)
		{
			if (type == typeof(WorkHoursInCalendar))
			{
				return new WorkHoursInCalendarSerializer();
			}
			return null;
		}

		private Hashtable readMethods;

		private Hashtable writeMethods;

		private Hashtable typedSerializers;
	}
}
