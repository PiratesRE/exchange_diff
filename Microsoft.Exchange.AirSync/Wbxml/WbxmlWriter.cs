using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.Exchange.AirSync.Wbxml
{
	internal class WbxmlWriter : WbxmlBase
	{
		public WbxmlWriter(Stream stream)
		{
			if (stream == null)
			{
				throw new WbxmlException("WbxmlWriter cannot take a null stream parameter");
			}
			this.stream = stream;
		}

		public void WriteXmlDocument(XmlDocument doc)
		{
			if (doc == null)
			{
				throw new WbxmlException("WbxmlWriter cannot take a null XmlDocument parameter");
			}
			this.WriteHeader();
			this.WriteXmlElement(doc.DocumentElement);
		}

		public void WriteXmlDocumentFromElement(XmlElement elem)
		{
			if (elem == null)
			{
				throw new WbxmlException("WbxmlWriter cannot take a null XmlElement parameter");
			}
			this.WriteHeader();
			this.WriteXmlElement(elem);
		}

		private static bool ElementHasContent(XmlElement elem)
		{
			foreach (object obj in elem.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.NodeType == XmlNodeType.Element || (xmlNode.NodeType == XmlNodeType.Text && !string.IsNullOrEmpty(xmlNode.Value)) || (xmlNode.NodeType == XmlNodeType.CDATA && !string.IsNullOrEmpty(xmlNode.Value)) || xmlNode.NodeType == XmlNodeType.Document)
				{
					return true;
				}
			}
			return false;
		}

		private void WriteEndTag()
		{
			this.stream.WriteByte(1);
		}

		private void WriteHeader()
		{
			this.currentTagPage = 0;
			this.WriteMultiByteInteger(3);
			this.WriteMultiByteInteger(1);
			this.WriteMultiByteInteger(106);
			this.WriteMultiByteInteger(0);
		}

		private void WriteMultiByteInteger(int value)
		{
			if (value > 127)
			{
				this.WriteMultiByteIntegerRecursive(value >> 7);
			}
			this.stream.WriteByte((byte)(value & 127));
		}

		private void WriteMultiByteIntegerRecursive(int value)
		{
			if (value > 127)
			{
				this.WriteMultiByteIntegerRecursive(value >> 7);
			}
			this.stream.WriteByte((byte)((value & 127) | 128));
		}

		private void WriteOpaqueString(string str)
		{
			this.stream.WriteByte(195);
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			this.WriteMultiByteInteger(bytes.Length);
			this.stream.Write(bytes, 0, bytes.Length);
		}

		private void WriteByteArray(byte[] data)
		{
			this.stream.WriteByte(195);
			this.WriteMultiByteInteger(data.Length);
			this.stream.Write(data, 0, data.Length);
		}

		private void WriteString(string str)
		{
			this.stream.WriteByte(3);
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			this.stream.Write(bytes, 0, bytes.Length);
			this.stream.WriteByte(0);
		}

		private void WriteTag(int tag, bool hasContent, bool hasAttributes)
		{
			byte b = (byte)((tag & 65280) >> 8);
			byte b2 = (byte)(tag & 63);
			if (hasContent)
			{
				b2 += 64;
			}
			if (hasAttributes)
			{
				b2 += 128;
			}
			if (b != this.currentTagPage)
			{
				this.stream.WriteByte(0);
				this.stream.WriteByte(b);
				this.currentTagPage = b;
			}
			this.stream.WriteByte(b2);
		}

		private void WriteXmlElement(XmlElement elem)
		{
			int tag = WbxmlBase.Schema.GetTag(elem.NamespaceURI, elem.LocalName);
			bool flag = WbxmlWriter.ElementHasContent(elem);
			this.WriteTag(tag, flag, false);
			if (flag)
			{
				foreach (object obj in elem.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					switch (xmlNode.NodeType)
					{
					case XmlNodeType.Element:
					{
						AirSyncBlobXmlNode airSyncBlobXmlNode = xmlNode as AirSyncBlobXmlNode;
						if (airSyncBlobXmlNode != null)
						{
							int tag2 = WbxmlBase.Schema.GetTag(airSyncBlobXmlNode.NamespaceURI, airSyncBlobXmlNode.LocalName);
							this.WriteTag(tag2, true, false);
							if (airSyncBlobXmlNode.ByteArray != null)
							{
								this.WriteByteArray(airSyncBlobXmlNode.ByteArray);
							}
							if (airSyncBlobXmlNode.Stream != null)
							{
								switch (airSyncBlobXmlNode.OriginalNodeType)
								{
								case XmlNodeType.Text:
									this.stream.WriteByte(3);
									break;
								case XmlNodeType.CDATA:
									this.stream.WriteByte(195);
									this.WriteMultiByteInteger((int)airSyncBlobXmlNode.StreamDataSize);
									break;
								}
								airSyncBlobXmlNode.CopyStream(this.stream);
								if (airSyncBlobXmlNode.OriginalNodeType == XmlNodeType.Text)
								{
									this.stream.WriteByte(0);
								}
							}
							this.WriteEndTag();
						}
						else
						{
							this.WriteXmlElement((XmlElement)xmlNode);
						}
						break;
					}
					case XmlNodeType.Attribute:
						break;
					case XmlNodeType.Text:
						this.WriteString(xmlNode.Value);
						break;
					case XmlNodeType.CDATA:
						this.WriteOpaqueString(xmlNode.Value);
						break;
					default:
						throw new AirSyncPermanentException(false);
					}
				}
				this.WriteEndTag();
			}
		}

		private const byte WbxmlVersion = 3;

		private byte currentTagPage;

		private Stream stream;
	}
}
