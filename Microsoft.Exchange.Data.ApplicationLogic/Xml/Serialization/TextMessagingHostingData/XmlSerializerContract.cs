using System;
using System.Collections;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.ApplicationLogic.TextMessaging;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Xml.Serialization.TextMessagingHostingData
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class XmlSerializerContract : XmlSerializerImplementation
	{
		public override XmlSerializationReader Reader
		{
			get
			{
				return new XmlSerializationReaderTextMessagingHostingData();
			}
		}

		public override XmlSerializationWriter Writer
		{
			get
			{
				return new XmlSerializationWriterTextMessagingHostingData();
			}
		}

		public override Hashtable ReadMethods
		{
			get
			{
				if (this.readMethods == null)
				{
					Hashtable hashtable = new Hashtable();
					hashtable["Microsoft.Exchange.Data.ApplicationLogic.TextMessaging.TextMessagingHostingData:::False:"] = "Read20_TextMessagingHostingData";
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
					hashtable["Microsoft.Exchange.Data.ApplicationLogic.TextMessaging.TextMessagingHostingData:::False:"] = "Write20_TextMessagingHostingData";
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
					hashtable.Add("Microsoft.Exchange.Data.ApplicationLogic.TextMessaging.TextMessagingHostingData:::False:", new TextMessagingHostingDataSerializer());
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
			return type == typeof(TextMessagingHostingData);
		}

		public override XmlSerializer GetSerializer(Type type)
		{
			if (type == typeof(TextMessagingHostingData))
			{
				return new TextMessagingHostingDataSerializer();
			}
			return null;
		}

		private Hashtable readMethods;

		private Hashtable writeMethods;

		private Hashtable typedSerializers;
	}
}
