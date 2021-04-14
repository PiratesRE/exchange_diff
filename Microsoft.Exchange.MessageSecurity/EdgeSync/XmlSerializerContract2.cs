using System;
using System.Collections;
using System.Xml.Serialization;

namespace Microsoft.Exchange.MessageSecurity.EdgeSync
{
	internal class XmlSerializerContract2 : XmlSerializerImplementation
	{
		public override XmlSerializationReader Reader
		{
			get
			{
				return new XmlSerializationReaderEdgeSyncCredential();
			}
		}

		public override XmlSerializationWriter Writer
		{
			get
			{
				return new XmlSerializationWriterEdgeSyncCredential();
			}
		}

		public override Hashtable ReadMethods
		{
			get
			{
				if (this.readMethods == null)
				{
					Hashtable hashtable = new Hashtable();
					hashtable["Microsoft.Exchange.MessageSecurity.EdgeSync.EdgeSyncCredential::"] = "Read3_EdgeSyncCredential";
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
					hashtable["Microsoft.Exchange.MessageSecurity.EdgeSync.EdgeSyncCredential::"] = "Write3_EdgeSyncCredential";
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
					hashtable.Add("Microsoft.Exchange.MessageSecurity.EdgeSync.EdgeSyncCredential::", new EdgeSyncCredentialSerializer());
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
			return type == typeof(EdgeSyncCredential);
		}

		public override XmlSerializer GetSerializer(Type type)
		{
			if (type == typeof(EdgeSyncCredential))
			{
				return new EdgeSyncCredentialSerializer();
			}
			return null;
		}

		private Hashtable readMethods;

		private Hashtable writeMethods;

		private Hashtable typedSerializers;
	}
}
