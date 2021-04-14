using System;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.WBXml
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class WBXmlWriter : WBXmlBase
	{
		internal WBXmlWriter(Stream stream)
		{
			if (stream == null)
			{
				throw new EasWBXmlTransientException("WBXmlWriter cannot take a null stream parameter");
			}
			this.stream = stream;
		}

		internal void WriteXmlDocument(XmlDocument doc)
		{
			if (doc == null)
			{
				throw new EasWBXmlTransientException("WBXmlWriter cannot take a null XmlDocument parameter");
			}
			this.WriteHeader();
			this.WriteXmlElement(doc.DocumentElement);
		}

		internal void WriteXmlDocumentFromElement(XmlElement elem)
		{
			if (elem == null)
			{
				throw new EasWBXmlTransientException("WBXmlWriter cannot take a null XmlElement parameter");
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
			int tag = WBXmlBase.Schema.GetTag(elem.NamespaceURI, elem.LocalName);
			bool flag = WBXmlWriter.ElementHasContent(elem);
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
						WBxmlBlobNode wbxmlBlobNode = xmlNode as WBxmlBlobNode;
						if (wbxmlBlobNode != null)
						{
							int tag2 = WBXmlBase.Schema.GetTag(wbxmlBlobNode.NamespaceURI, wbxmlBlobNode.LocalName);
							this.WriteTag(tag2, true, false);
							if (wbxmlBlobNode.ByteArray != null)
							{
								this.WriteByteArray(wbxmlBlobNode.ByteArray);
							}
							if (wbxmlBlobNode.Stream != null)
							{
								switch (wbxmlBlobNode.OriginalNodeType)
								{
								case XmlNodeType.Text:
									this.stream.WriteByte(3);
									break;
								case XmlNodeType.CDATA:
									this.stream.WriteByte(195);
									this.WriteMultiByteInteger((int)wbxmlBlobNode.StreamDataSize);
									break;
								}
								wbxmlBlobNode.CopyStream(this.stream);
								if (wbxmlBlobNode.OriginalNodeType == XmlNodeType.Text)
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
						throw new EasWBXmlTransientException(string.Format("WriteXmlElement encountered an invalid XmlNodeType of '{0}'", xmlNode.NodeType));
					}
				}
				this.WriteEndTag();
			}
		}

		private const byte WBXmlVersion = 3;

		private byte currentTagPage;

		private Stream stream;
	}
}
