using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.AirSync.Wbxml
{
	internal class WbxmlReader : WbxmlBase, IDisposeTrackable, IDisposable
	{
		public WbxmlReader(Stream stream) : this(stream, false)
		{
		}

		public WbxmlReader(Stream stream, bool parseBlobAsByteArray)
		{
			this.parseBlobAsByteArray = parseBlobAsByteArray;
			if (stream == null)
			{
				throw new WbxmlException("WbxmlReader cannot accept a null stream");
			}
			this.underlyingStream = stream;
			this.stream = new PooledBufferedStream(stream, WbxmlReader.readerPool, false);
			this.disposeTracker = this.GetDisposeTracker();
		}

		private static XmlDocument ErrorDocument
		{
			get
			{
				if (WbxmlReader.errorDocument == null)
				{
					string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><WbxmlConversionError></WbxmlConversionError>";
					XmlDocument xmlDocument = new SafeXmlDocument();
					xmlDocument.LoadXml(xml);
					WbxmlReader.errorDocument = xmlDocument;
				}
				return WbxmlReader.errorDocument;
			}
		}

		public XmlDocument ReadXmlDocument()
		{
			this.ReadHeader();
			XmlDocument result;
			try
			{
				bool flag;
				bool flag2;
				int tag = this.ReadTag(out flag, out flag2);
				string name = WbxmlBase.Schema.GetName(tag);
				string nameSpace = WbxmlBase.Schema.GetNameSpace(tag);
				if (name == null || nameSpace == null)
				{
					result = WbxmlReader.ErrorDocument;
				}
				else
				{
					XmlDocument xmlDocument = new SafeXmlDocument();
					bool flag3 = WbxmlBase.Schema.IsTagSecure(tag);
					bool flag4 = WbxmlBase.Schema.IsTagAnOpaqueBlob(tag);
					XmlElement xmlElement;
					if (flag3)
					{
						xmlElement = new AirSyncSecureStringXmlNode(null, name, nameSpace, xmlDocument);
					}
					else if (flag4)
					{
						xmlElement = new AirSyncBlobXmlNode(null, name, nameSpace, xmlDocument);
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
						result = WbxmlReader.ErrorDocument;
					}
					else
					{
						result = xmlDocument;
					}
				}
			}
			catch (IndexOutOfRangeException innerException)
			{
				throw new WbxmlException("Invalid WBXML code/codepage from client", innerException);
			}
			catch (NotImplementedException innerException2)
			{
				throw new WbxmlException("Invalid WBXML data from client (see inner exception).", innerException2);
			}
			catch (ArgumentOutOfRangeException innerException3)
			{
				throw new WbxmlException("Invalid WBXML code from client", innerException3);
			}
			return result;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<WbxmlReader>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
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
			if (buffer.Length > WbxmlReader.maxSecureStringLength)
			{
				throw new WbxmlException("Buffer is too long");
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
				throw new WbxmlException("Document nested too deep");
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
						AirSyncSecureStringXmlNode airSyncSecureStringXmlNode = (AirSyncSecureStringXmlNode)elem;
						airSyncSecureStringXmlNode.SecureData = this.ReadInlineSecureString();
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
						AirSyncSecureStringXmlNode airSyncSecureStringXmlNode2 = (AirSyncSecureStringXmlNode)elem;
						airSyncSecureStringXmlNode2.SecureData = this.ReadOpaqueSecureString();
					}
					else if (elemIsBlobData)
					{
						AirSyncBlobXmlNode airSyncBlobXmlNode = elem as AirSyncBlobXmlNode;
						int num2 = this.ReadMultiByteInteger();
						if (this.stream.Position + (long)num2 > this.stream.Length)
						{
							break;
						}
						if (this.parseBlobAsByteArray)
						{
							airSyncBlobXmlNode.ByteArray = this.ReadBytes(num2);
						}
						else
						{
							airSyncBlobXmlNode.Stream = new SubStream(this.underlyingStream, this.stream.Position, (long)num2);
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
					string name = WbxmlBase.Schema.GetName(num);
					string nameSpace = WbxmlBase.Schema.GetNameSpace(num);
					if (name == null || nameSpace == null)
					{
						return false;
					}
					bool flag3 = WbxmlBase.Schema.IsTagSecure(num);
					bool flag4 = WbxmlBase.Schema.IsTagAnOpaqueBlob(num);
					XmlElement xmlElement;
					if (flag3)
					{
						xmlElement = new AirSyncSecureStringXmlNode(null, name, nameSpace, elem.OwnerDocument);
					}
					else if (flag4)
					{
						xmlElement = new AirSyncBlobXmlNode(null, name, nameSpace, elem.OwnerDocument);
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
				throw new WbxmlException("End of stream reached by ReadByte before parsing was complete", innerException);
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
				throw new WbxmlException("End of stream reached by ReadBytes before parsing was complete", innerException);
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
				throw new WbxmlException("Unsupported Wbxml version");
			}
			int num2 = this.ReadMultiByteInteger();
			if (num2 != 1)
			{
				throw new WbxmlException("Unsupported PublicID: " + num2);
			}
			int num3 = this.ReadMultiByteInteger();
			if (num3 != 106)
			{
				throw new WbxmlException("Unsupported charset: 0x" + num3.ToString("X", CultureInfo.InvariantCulture));
			}
			int num4 = this.ReadMultiByteInteger();
			if (num4 > 0)
			{
				throw new WbxmlException(string.Format("stringTableLength {0} > 0 ", num4));
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
				array = (array2 = WbxmlReader.readerPool.Acquire());
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
				throw new WbxmlException("End of stream reached by ReadInlineString before parsing was complete", innerException);
				Block_5:
				this.stream.Seek(position + (long)num + 1L, SeekOrigin.Begin);
				@string = Encoding.UTF8.GetString(array2, 0, num);
			}
			finally
			{
				if (array != null)
				{
					WbxmlReader.readerPool.Release(array);
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
				if (num2 > WbxmlReader.maxSecureStringLength)
				{
					throw new WbxmlException("Inline secure string is too long");
				}
				byte[] array = new byte[num2];
				gchandle = GCHandle.Alloc(array, GCHandleType.Pinned);
				this.ReadBytes(array);
				result = WbxmlReader.ConvertToSecureString(array);
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
						throw new WbxmlException("Multi-byte integer is too large");
					}
					return (int)num;
				}
			}
			throw new WbxmlException("Multi-byte integer is too large");
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
				if (num > WbxmlReader.maxSecureStringLength)
				{
					throw new WbxmlException("Opaque secure string is too long");
				}
				byte[] array = new byte[num];
				gchandle = GCHandle.Alloc(array, GCHandleType.Pinned);
				this.ReadBytes(array);
				result = WbxmlReader.ConvertToSecureString(array);
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
					throw new WbxmlException("Multiple consecutive page change codes not allowed");
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
						throw new WbxmlException("Unsupported control code");
					}
					throw new WbxmlException("Invalid WBXML - Attributes (potentially malicious)");
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
