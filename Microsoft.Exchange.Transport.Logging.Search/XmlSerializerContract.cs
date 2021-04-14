using System;
using System.Collections;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	internal class XmlSerializerContract : XmlSerializerImplementation
	{
		public override XmlSerializationReader Reader
		{
			get
			{
				return new XmlSerializationReaderLogQuery();
			}
		}

		public override XmlSerializationWriter Writer
		{
			get
			{
				return new XmlSerializationWriterLogQuery();
			}
		}

		public override Hashtable ReadMethods
		{
			get
			{
				if (this.readMethods == null)
				{
					Hashtable hashtable = new Hashtable();
					hashtable["Microsoft.Exchange.Transport.Logging.Search.LogQuery::"] = "Read28_LogQuery";
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
					hashtable["Microsoft.Exchange.Transport.Logging.Search.LogQuery::"] = "Write28_LogQuery";
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
					hashtable.Add("Microsoft.Exchange.Transport.Logging.Search.LogQuery::", new LogQuerySerializer());
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
			return type == typeof(LogQuery);
		}

		public override XmlSerializer GetSerializer(Type type)
		{
			if (type == typeof(LogQuery))
			{
				return new LogQuerySerializer();
			}
			return null;
		}

		private Hashtable readMethods;

		private Hashtable writeMethods;

		private Hashtable typedSerializers;
	}
}
