using System;
using System.IO;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync.Wbxml
{
	internal class AirSyncBlobXmlNode : XmlElement
	{
		public AirSyncBlobXmlNode(string prefix, string localName, string namespaceURI, XmlDocument doc) : base(prefix, localName, namespaceURI, doc)
		{
		}

		public Stream Stream
		{
			get
			{
				return this.stream;
			}
			set
			{
				if (this.byteArray != null)
				{
					throw new InvalidOperationException("byteArray has already been assigned");
				}
				this.stream = value;
			}
		}

		public byte[] ByteArray
		{
			get
			{
				return this.byteArray;
			}
			set
			{
				if (this.stream != null)
				{
					throw new InvalidOperationException("stream has already been assigned");
				}
				this.byteArray = value;
			}
		}

		public long StreamDataSize
		{
			get
			{
				if (this.streamDataSize < 0L)
				{
					return this.Stream.Length;
				}
				return this.streamDataSize;
			}
			set
			{
				this.streamDataSize = value;
			}
		}

		public XmlNodeType OriginalNodeType
		{
			get
			{
				return this.originalNodeType;
			}
			set
			{
				this.originalNodeType = value;
			}
		}

		public override string InnerText
		{
			get
			{
				return "*** blob ***";
			}
			set
			{
				throw new NotImplementedException("This kind of node should contain a blob, not a string");
			}
		}

		public void CopyStream(Stream outputStream)
		{
			if (this.stream == null)
			{
				return;
			}
			if (this.stream.CanSeek)
			{
				this.stream.Seek(0L, SeekOrigin.Begin);
			}
			else
			{
				AirSyncDiagnostics.TraceError<Type>(ExTraceGlobals.ConversionTracer, this, "this.streamis of {0} type.", this.stream.GetType());
			}
			AirSyncStream airSyncStream = this.stream as AirSyncStream;
			if (airSyncStream != null)
			{
				airSyncStream.CopyStream(outputStream, (int)this.StreamDataSize);
				return;
			}
			StreamHelper.CopyStream(this.stream, outputStream, (int)this.StreamDataSize);
		}

		public override int GetHashCode()
		{
			AirSyncStream airSyncStream = this.stream as AirSyncStream;
			if (airSyncStream != null)
			{
				return airSyncStream.StreamHash;
			}
			if (this.byteArray != null)
			{
				return (int)StreamHelper.UpdateCrc32(0U, this.byteArray, 0, this.byteArray.Length);
			}
			return base.GetHashCode();
		}

		private Stream stream;

		private long streamDataSize = -1L;

		private byte[] byteArray;

		private XmlNodeType originalNodeType = XmlNodeType.CDATA;
	}
}
