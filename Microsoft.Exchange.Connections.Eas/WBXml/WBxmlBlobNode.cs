using System;
using System.IO;
using System.Xml;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.WBXml
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class WBxmlBlobNode : XmlElement
	{
		internal WBxmlBlobNode(string prefix, string localName, string namespaceUri, XmlDocument doc) : base(prefix, localName, namespaceUri, doc)
		{
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

		internal StreamAccessor Stream
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

		internal byte[] ByteArray
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

		internal long StreamDataSize
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

		internal XmlNodeType OriginalNodeType
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

		public override int GetHashCode()
		{
			if (this.byteArray != null)
			{
				return (int)WBXmlStreamHelper.UpdateCrc32(0U, this.byteArray, 0, this.byteArray.Length);
			}
			return base.GetHashCode();
		}

		internal void CopyStream(Stream outputStream)
		{
			if (this.stream == null)
			{
				return;
			}
			if (this.stream.CanSeek)
			{
				this.stream.Seek(0L, SeekOrigin.Begin);
			}
			WBXmlStreamHelper.CopyStream(this.stream, outputStream, (int)this.StreamDataSize);
		}

		private StreamAccessor stream;

		private long streamDataSize = -1L;

		private byte[] byteArray;

		private XmlNodeType originalNodeType = XmlNodeType.CDATA;
	}
}
