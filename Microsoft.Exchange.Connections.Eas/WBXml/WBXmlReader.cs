using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Connections.Eas.WBXml
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class WBXmlReader : WBXmlBase, IDisposeTrackable, IDisposable
	{
		internal WBXmlReader(Stream stream) : this(stream, false)
		{
		}

		internal WBXmlReader(Stream stream, bool parseBlobAsByteArray)
		{
			this.parseBlobAsByteArray = parseBlobAsByteArray;
			if (stream == null)
			{
				throw new EasWBXmlTransientException("WBXmlReader cannot accept a null stream");
			}
			this.underlyingStream = stream;
			this.stream = new PooledBufferedStream(stream, WBXmlReader.readerPool, false);
			this.disposeTracker = this.GetDisposeTracker();
		}

		private static XmlDocument ErrorDocument
		{
			get
			{
				if (WBXmlReader.errorDocument == null)
				{
					XmlDocument xmlDocument = new SafeXmlDocument();
					xmlDocument.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?><WBXmlConversionError></WBXmlConversionError>");
					WBXmlReader.errorDocument = xmlDocument;
				}
				return WBXmlReader.errorDocument;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<WBXmlReader>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		internal XmlDocument ReadXmlDocument()
		{
			this.ReadHeader();
			XmlDocument result;
			try
			{
				bool flag;
				bool flag2;
				int tag = this.ReadTag(out flag, out flag2);
				string name = WBXmlBase.Schema.GetName(tag);
				string nameSpace = WBXmlBase.Schema.GetNameSpace(tag);
				if (name == null || nameSpace == null)
				{
					result = WBXmlReader.ErrorDocument;
				}
				else
				{
					XmlDocument xmlDocument = new SafeXmlDocument();
					bool flag3 = WBXmlBase.Schema.IsTagSecure(tag);
					bool flag4 = WBXmlBase.Schema.IsTagAnOpaqueBlob(tag);
					XmlElement xmlElement;
					if (flag3)
					{
						xmlElement = new WBXmlSecureStringNode(null, name, nameSpace, xmlDocument);
					}
					else if (flag4)
					{
						xmlElement = new WBxmlBlobNode(null, name, nameSpace, xmlDocument);
					}
					else
					{
						xmlElement = xmlDocument.CreateElement(name, nameSpace);
					}
					xmlDocument.AppendChild(xmlElement);
					if (flag)
					{
						this.SkipAttributes();
					}
					if (flag2 && !this.FillXmlElement(xmlElement, 0, flag3, flag4))
					{
						result = WBXmlReader.ErrorDocument;
					}
					else
					{
						result = xmlDocument;
					}
				}
			}
			catch (IndexOutOfRangeException innerException)
			{
				throw new EasWBXmlTransientException("Invalid WBXML code/codepage from client", innerException);
			}
			catch (ArgumentOutOfRangeException innerException2)
			{
				throw new EasWBXmlTransientException("Invalid WBXML code from client", innerException2);
			}
			return result;
		}

		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.underlyingStream = null;
				if (this.stream != null)
				{
					this.stream.Dispose();
					this.stream = null;
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
			}
		}

		private static SecureString ConvertToSecureString(byte[] buffer)
		{
			if (buffer.Length > WBXmlReader.maxSecureStringLength)
			{
				throw new EasWBXmlTransientException("Buffer is too long");
			}
			GCHandle gchandle = default(GCHandle);
			SecureString result;
			try
			{
				char[] chars = Encoding.UTF8.GetChars(buffer);
				gchandle = GCHandle.Alloc(chars, GCHandleType.Pinned);
				Array.Clear(buffer, 0, buffer.Length);
				SecureString secureString = chars.ConvertToSecureString();
				Array.Clear(chars, 0, chars.Length);
				result = secureString;
			}
			finally
			{
				if (gchandle.IsAllocated)
				{
					gchandle.Free();
				}
			}
			return result;
		}

		private bool FillXmlElement(XmlElement elem, int depth, bool elemIsSecureData, bool elemIsBlobData)
		{
			if (depth > 20)
			{
				throw new EasWBXmlTransientException("Document nested too deep");
			}
			for (;;)
			{
				bool flag;
				bool flag2;
				int num = this.ReadTag(out flag, out flag2);
				if (num == 1)
				{
					return true;
				}
				if (num == 3)
				{
					if (elemIsSecureData)
					{
						WBXmlSecureStringNode wbxmlSecureStringNode = (WBXmlSecureStringNode)elem;
						wbxmlSecureStringNode.SecureData = this.ReadInlineSecureString();
					}
					else
					{
						string innerText = this.ReadInlineString();
						elem.InnerText = innerText;
					}
				}
				else if (num == 195)
				{
					if (elemIsSecureData)
					{
						WBXmlSecureStringNode wbxmlSecureStringNode2 = (WBXmlSecureStringNode)elem;
						wbxmlSecureStringNode2.SecureData = this.ReadOpaqueSecureString();
					}
					else if (elemIsBlobData)
					{
						WBxmlBlobNode wbxmlBlobNode = elem as WBxmlBlobNode;
						int num2 = this.ReadMultiByteInteger();
						if (this.stream.Position + (long)num2 > this.stream.Length)
						{
							break;
						}
						if (this.parseBlobAsByteArray)
						{
							wbxmlBlobNode.ByteArray = this.ReadBytes(num2);
						}
						else
						{
							wbxmlBlobNode.Stream = new WBXmlSubStream(this.underlyingStream, this.stream.Position, (long)num2);
							this.stream.Seek((long)num2, SeekOrigin.Current);
						}
					}
					else
					{
						string innerText2 = this.ReadOpaqueString();
						elem.InnerText = innerText2;
					}
				}
				if ((num & 63) >= 5)
				{
					if (flag)
					{
						this.SkipAttributes();
					}
					string name = WBXmlBase.Schema.GetName(num);
					string nameSpace = WBXmlBase.Schema.GetNameSpace(num);
					if (name == null || nameSpace == null)
					{
						return false;
					}
					bool flag3 = WBXmlBase.Schema.IsTagSecure(num);
					bool flag4 = WBXmlBase.Schema.IsTagAnOpaqueBlob(num);
					XmlElement xmlElement;
					if (flag3)
					{
						xmlElement = new WBXmlSecureStringNode(null, name, nameSpace, elem.OwnerDocument);
					}
					else if (flag4)
					{
						xmlElement = new WBxmlBlobNode(null, name, nameSpace, elem.OwnerDocument);
					}
					else
					{
						xmlElement = elem.OwnerDocument.CreateElement(name, nameSpace);
					}
					if (flag2 && !this.FillXmlElement(xmlElement, depth + 1, flag3, flag4))
					{
						return false;
					}
					elem.AppendChild(xmlElement);
				}
			}
			return false;
		}

		private byte ReadByte()
		{
			int num = this.stream.ReadByte();
			if (num == -1)
			{
				Exception innerException = new EndOfStreamException();
				throw new EasWBXmlTransientException("End of stream reached by ReadByte before parsing was complete", innerException);
			}
			return (byte)num;
		}

		private void ReadBytes(byte[] answer)
		{
			int num = answer.Length;
			int i;
			int num2;
			for (i = 0; i < num; i += num2)
			{
				num2 = this.stream.Read(answer, i, num - i);
				if (num2 == 0)
				{
					break;
				}
			}
			if (i != num)
			{
				Exception innerException = new EndOfStreamException();
				throw new EasWBXmlTransientException("End of stream reached by ReadBytes before parsing was complete", innerException);
			}
		}

		private byte[] ReadBytes(int count)
		{
			byte[] array = new byte[count];
			this.ReadBytes(array);
			return array;
		}

		private void SkipBytes(int count)
		{
			if (this.stream.CanSeek)
			{
				this.stream.Seek((long)count, SeekOrigin.Current);
				return;
			}
			for (int i = 0; i < count; i++)
			{
				this.ReadByte();
			}
		}

		private void ReadHeader()
		{
			int num = (int)this.ReadByte();
			if (num != 3)
			{
				throw new EasWBXmlTransientException("Unsupported WBXml version");
			}
			int num2 = this.ReadMultiByteInteger();
			if (num2 != 1)
			{
				throw new EasWBXmlTransientException("Unsupported PublicID: " + num2);
			}
			int num3 = this.ReadMultiByteInteger();
			if (num3 != 106)
			{
				throw new EasWBXmlTransientException("Unsupported charset: 0x" + num3.ToString("X", CultureInfo.InvariantCulture));
			}
			int num4 = this.ReadMultiByteInteger();
			if (num4 > 0)
			{
				throw new EasWBXmlTransientException(string.Format("stringTableLength {0} > 0 ", num4));
			}
		}

		private string ReadInlineString()
		{
			long position = this.stream.Position;
			int num = 0;
			int num2 = 0;
			byte[] array = null;
			string @string;
			try
			{
				byte[] array2;
				array = (array2 = WBXmlReader.readerPool.Acquire());
				for (;;)
				{
					int num3 = this.stream.Read(array2, num, array2.Length - num);
					if (num3 == 0)
					{
						break;
					}
					num2 += num3;
					while (num < num2 && array2[num] != 0)
					{
						num++;
					}
					if (num < num2)
					{
						goto Block_5;
					}
					Array.Resize<byte>(ref array2, array2.Length * 2);
				}
				Exception innerException = new EndOfStreamException();
				throw new EasWBXmlTransientException("End of stream reached by ReadInlineString before parsing was complete", innerException);
				Block_5:
				this.stream.Seek(position + (long)num + 1L, SeekOrigin.Begin);
				@string = Encoding.UTF8.GetString(array2, 0, num);
			}
			finally
			{
				if (array != null)
				{
					WBXmlReader.readerPool.Release(array);
				}
			}
			return @string;
		}

		private SecureString ReadInlineSecureString()
		{
			long position = this.stream.Position;
			long num = position;
			while (this.ReadByte() != 0)
			{
				num += 1L;
			}
			this.stream.Position = position;
			GCHandle gchandle = default(GCHandle);
			SecureString result;
			try
			{
				int num2 = (int)(num - position) + 1;
				if (num2 > WBXmlReader.maxSecureStringLength)
				{
					throw new EasWBXmlTransientException("Inline secure string is too long");
				}
				byte[] array = new byte[num2];
				gchandle = GCHandle.Alloc(array, GCHandleType.Pinned);
				this.ReadBytes(array);
				result = WBXmlReader.ConvertToSecureString(array);
			}
			finally
			{
				if (gchandle.IsAllocated)
				{
					gchandle.Free();
				}
			}
			return result;
		}

		private int ReadMultiByteInteger()
		{
			uint num = 0U;
			byte b = this.ReadByte();
			int i = 1;
			while (i <= 5)
			{
				num = (num << 7) + (uint)(b & 127);
				if ((b & 128) != 0)
				{
					b = this.ReadByte();
					i++;
				}
				else
				{
					if (num > 2147483647U)
					{
						throw new EasWBXmlTransientException("Multi-byte integer is too large");
					}
					return (int)num;
				}
			}
			throw new EasWBXmlTransientException("Multi-byte integer is too large");
		}

		private string ReadOpaqueString()
		{
			int count = this.ReadMultiByteInteger();
			byte[] bytes = this.ReadBytes(count);
			return Encoding.UTF8.GetString(bytes);
		}

		private SecureString ReadOpaqueSecureString()
		{
			GCHandle gchandle = default(GCHandle);
			SecureString result;
			try
			{
				int num = this.ReadMultiByteInteger();
				if (num > WBXmlReader.maxSecureStringLength)
				{
					throw new EasWBXmlTransientException("Opaque secure string is too long");
				}
				byte[] array = new byte[num];
				gchandle = GCHandle.Alloc(array, GCHandleType.Pinned);
				this.ReadBytes(array);
				result = WBXmlReader.ConvertToSecureString(array);
			}
			finally
			{
				if (gchandle.IsAllocated)
				{
					gchandle.Free();
				}
			}
			return result;
		}

		private int ReadTag(out bool hasAttributes, out bool hasContent)
		{
			byte b = this.ReadByte();
			int num = 0;
			while (b == 0)
			{
				this.currentTagPage = this.ReadByte();
				if (num++ > 5)
				{
					throw new EasWBXmlTransientException("Multiple consecutive page change codes not allowed");
				}
				b = this.ReadByte();
			}
			if ((b & 63) < 5)
			{
				hasAttributes = false;
				hasContent = ((b & 64) != 0);
				return (int)b;
			}
			hasAttributes = ((b & 128) != 0);
			hasContent = ((b & 64) != 0);
			b &= 63;
			return (int)b | (int)this.currentTagPage << 8;
		}

		private void SkipAttributes()
		{
			byte b = this.ReadByte();
			while (b != 1)
			{
				if (b == 0)
				{
					this.ReadByte();
				}
				else if (b == 4)
				{
					this.ReadMultiByteInteger();
				}
				else if (b == 3)
				{
					for (b = this.ReadByte(); b != 0; b = this.ReadByte())
					{
					}
				}
				else if (b == 131)
				{
					this.ReadMultiByteInteger();
				}
				else if (b == 195)
				{
					int count = this.ReadMultiByteInteger();
					this.SkipBytes(count);
				}
				else
				{
					if ((b & 63) < 5)
					{
						throw new EasWBXmlTransientException("Unsupported control code");
					}
					throw new EasWBXmlTransientException("Invalid WBXML - Attributes (potentially malicious)");
				}
			}
		}

		private static XmlDocument errorDocument;

		private static BufferPool readerPool = new BufferPool(1024, true);

		private static int maxSecureStringLength = 256;

		private readonly bool parseBlobAsByteArray;

		private readonly DisposeTracker disposeTracker;

		private byte currentTagPage;

		private Stream stream;

		private Stream underlyingStream;
	}
}
