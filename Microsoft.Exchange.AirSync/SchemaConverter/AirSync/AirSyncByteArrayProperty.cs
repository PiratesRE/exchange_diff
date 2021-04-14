using System;
using System.IO;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.AirSync.Wbxml;

namespace Microsoft.Exchange.AirSync.SchemaConverter.AirSync
{
	internal class AirSyncByteArrayProperty : AirSyncProperty, IByteArrayProperty, IProperty
	{
		public AirSyncByteArrayProperty(string xmlNodeNamespace, string airSyncTagName, bool requiresClientSupport) : base(xmlNodeNamespace, airSyncTagName, requiresClientSupport)
		{
		}

		public byte[] ByteArrayData
		{
			get
			{
				if (this.byteData == null)
				{
					AirSyncBlobXmlNode airSyncBlobXmlNode = (AirSyncBlobXmlNode)base.XmlNode;
					if (airSyncBlobXmlNode != null && airSyncBlobXmlNode.Stream != null && airSyncBlobXmlNode.Stream.CanSeek && airSyncBlobXmlNode.Stream.CanRead)
					{
						this.byteData = new byte[airSyncBlobXmlNode.Stream.Length];
						airSyncBlobXmlNode.Stream.Seek(0L, SeekOrigin.Begin);
						airSyncBlobXmlNode.Stream.Read(this.byteData, 0, (int)airSyncBlobXmlNode.Stream.Length);
					}
				}
				return this.byteData;
			}
		}

		public override void Unbind()
		{
			base.Unbind();
			this.byteData = null;
		}

		protected override void InternalCopyFrom(IProperty srcProperty)
		{
			IByteArrayProperty byteArrayProperty = srcProperty as IByteArrayProperty;
			if (byteArrayProperty == null)
			{
				throw new UnexpectedTypeException("IByteArrayProperty", srcProperty);
			}
			byte[] byteArrayData = byteArrayProperty.ByteArrayData;
			if (PropertyState.Modified == srcProperty.State && byteArrayData != null)
			{
				base.CreateAirSyncNode(base.AirSyncTagNames[0], byteArrayData);
			}
		}

		private byte[] byteData;
	}
}
