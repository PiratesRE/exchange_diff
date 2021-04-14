using System;
using System.Collections;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.OOF
{
	internal class XmlSerializerContract : XmlSerializerImplementation
	{
		public override XmlSerializationReader Reader
		{
			get
			{
				return new XmlSerializationReaderUserOofSettingsSerializer();
			}
		}

		public override XmlSerializationWriter Writer
		{
			get
			{
				return new XmlSerializationWriterUserOofSettingsSerializer();
			}
		}

		public override Hashtable ReadMethods
		{
			get
			{
				if (this.readMethods == null)
				{
					Hashtable hashtable = new Hashtable();
					hashtable["Microsoft.Exchange.InfoWorker.Common.OOF.UserOofSettingsSerializer::UserOofSettings:True:"] = "Read8_UserOofSettings";
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
					hashtable["Microsoft.Exchange.InfoWorker.Common.OOF.UserOofSettingsSerializer::UserOofSettings:True:"] = "Write7_UserOofSettings";
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
					hashtable.Add("Microsoft.Exchange.InfoWorker.Common.OOF.UserOofSettingsSerializer::UserOofSettings:True:", new UserOofSettingsSerializerSerializer());
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
			return type == typeof(UserOofSettingsSerializer);
		}

		public override XmlSerializer GetSerializer(Type type)
		{
			if (type == typeof(UserOofSettingsSerializer))
			{
				return new UserOofSettingsSerializerSerializer();
			}
			return null;
		}

		private Hashtable readMethods;

		private Hashtable writeMethods;

		private Hashtable typedSerializers;
	}
}
