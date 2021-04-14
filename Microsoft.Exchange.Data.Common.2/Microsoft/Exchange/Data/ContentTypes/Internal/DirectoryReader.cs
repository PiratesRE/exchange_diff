using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Mime.Encoders;

namespace Microsoft.Exchange.Data.ContentTypes.Internal
{
	internal class DirectoryReader : IDisposable
	{
		public DirectoryReader(Stream inputStream, Encoding outerCharsetEncoding, ComplianceTracker complianceTracker)
		{
			this.inputStream = new UnfoldingStream(inputStream);
			this.outerCharsetEncoding = outerCharsetEncoding;
			this.currentCharsetEncoding = outerCharsetEncoding;
			this.currentCharsetDecoder = outerCharsetEncoding.GetDecoder();
			this.currentCharsetEncoder = outerCharsetEncoding.GetEncoder();
			this.dataBytes = new byte[256];
			this.dataChars = new char[256];
			this.complianceTracker = complianceTracker;
			this.SetFallback();
		}

		public Encoding CurrentCharsetEncoding
		{
			get
			{
				this.CheckDisposed("CurrentEncoding::get");
				return this.currentCharsetEncoding;
			}
		}

		public bool ReadChar(out char result, out bool newLine)
		{
			this.CheckDisposed("ReadChar");
			result = '?';
			newLine = false;
			char? c = this.lastChar;
			int? num = (c != null) ? new int?((int)c.GetValueOrDefault()) : null;
			if (num != null)
			{
				result = this.lastChar.Value;
				this.lastChar = null;
			}
			else if (!this.ReadChar(out result))
			{
				return false;
			}
			if (!this.isDecoding)
			{
				if (result != '\r')
				{
					return true;
				}
				if (this.ReadChar(out result))
				{
					if (result == '\n')
					{
						newLine = true;
						return true;
					}
					this.lastChar = new char?(result);
				}
				result = '\r';
			}
			return true;
		}

		public void SwitchCharsetEncoding(Encoding newCharsetEncoding)
		{
			this.CheckDisposed("SwitchEncoding");
			char? c = this.lastChar;
			int? num = (c != null) ? new int?((int)c.GetValueOrDefault()) : null;
			if (num != null)
			{
				throw new InvalidOperationException();
			}
			if (newCharsetEncoding.WebName == this.CurrentCharsetEncoding.WebName)
			{
				return;
			}
			this.bottomByte += this.currentCharsetEncoder.GetByteCount(this.dataChars, 0, this.idxChar, true);
			if (this.bottomByte > this.topByte)
			{
				this.complianceTracker.SetComplianceStatus(ComplianceStatus.InvalidCharacterInPropertyValue, CalendarStrings.InvalidCharacterInPropertyValue);
				this.bottomByte = this.topByte;
			}
			this.idxByte = this.bottomByte;
			this.idxChar = 0;
			this.topChar = 0;
			this.currentCharsetEncoding = newCharsetEncoding;
			this.currentCharsetEncoder = newCharsetEncoding.GetEncoder();
			this.currentCharsetDecoder = newCharsetEncoding.GetDecoder();
		}

		public void RestoreCharsetEncoding()
		{
			this.CheckDisposed("RestoreEncoding");
			this.SwitchCharsetEncoding(this.outerCharsetEncoding);
		}

		public void ApplyValueDecoder(ByteEncoder decoder)
		{
			this.CheckDisposed("ApplyValueDecoder");
			char? c = this.lastChar;
			int? num = (c != null) ? new int?((int)c.GetValueOrDefault()) : null;
			if (num != null)
			{
				throw new InvalidOperationException();
			}
			if (this.decoderStream != null)
			{
				throw new InvalidOperationException();
			}
			this.decoderStream = new EncoderStream(this.GetValueReadStream(null), decoder, EncoderStreamAccess.Read);
		}

		public Stream GetValueReadStream(DirectoryReader.OnValueEndFunc callback)
		{
			this.CheckDisposed("GetValueReadStream");
			char? c = this.lastChar;
			int? num = (c != null) ? new int?((int)c.GetValueOrDefault()) : null;
			if (num != null)
			{
				throw new InvalidOperationException();
			}
			this.bottomByte += this.currentCharsetEncoder.GetByteCount(this.dataChars, 0, this.idxChar, true);
			if (this.bottomByte > this.topByte)
			{
				this.complianceTracker.SetComplianceStatus(ComplianceStatus.InvalidCharacterInPropertyValue, CalendarStrings.InvalidCharacterInPropertyValue);
				this.bottomByte = this.topByte;
			}
			this.idxByte = this.bottomByte;
			this.idxChar = 0;
			this.topChar = 0;
			this.inputStream.Rewind(this.topByte - this.idxByte);
			this.topByte = 0;
			this.idxByte = 0;
			this.bottomByte = 0;
			return new DirectoryReader.AdapterStream(this.inputStream, callback);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				throw new ObjectDisposedException("DirectoryReader", methodName);
			}
		}

		private bool ReadChar(out char result)
		{
			result = '?';
			if (this.idxChar >= this.topChar)
			{
				this.idxChar = 0;
				this.topChar = 0;
				int num = 0;
				int num2 = 0;
				bool flag = false;
				if (this.idxByte < this.topByte)
				{
					this.currentCharsetDecoder.Convert(this.dataBytes, this.idxByte, this.topByte - this.idxByte, this.dataChars, 0, this.dataChars.Length, false, out num, out num2, out flag);
					this.topChar = num2;
					this.idxByte += num;
				}
				while (this.topChar == 0)
				{
					for (int i = 0; i < this.topByte - this.idxByte; i++)
					{
						this.dataBytes[i] = this.dataBytes[this.idxByte + i];
					}
					this.topByte -= this.idxByte;
					this.bottomByte = 0;
					this.idxByte = 0;
					int num3 = this.ReadInputStream(this.dataBytes, this.topByte, this.dataBytes.Length - this.topByte);
					this.topByte += num3;
					this.currentCharsetDecoder.Convert(this.dataBytes, 0, this.topByte, this.dataChars, 0, this.dataChars.Length, num3 == 0, out num, out num2, out flag);
					this.topChar = num2;
					this.idxByte += num;
					if (num3 == 0 && this.topChar == 0)
					{
						return false;
					}
				}
			}
			result = this.dataChars[this.idxChar++];
			if (this.swallowUTFByteOrderMark && (result == '￾' || result == '﻿'))
			{
				this.swallowUTFByteOrderMark = false;
				return this.ReadChar(out result);
			}
			this.swallowUTFByteOrderMark = false;
			return true;
		}

		private int ReadInputStream(byte[] buffer, int offset, int count)
		{
			this.isDecoding = false;
			if (this.decoderStream == null)
			{
				return this.inputStream.Read(buffer, offset, count);
			}
			int num = this.decoderStream.Read(buffer, offset, count);
			if (num > 0)
			{
				this.isDecoding = true;
				return num;
			}
			this.decoderStream.Dispose();
			this.decoderStream = null;
			buffer[offset] = 13;
			buffer[offset + 1] = 10;
			return 2;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				if (disposing)
				{
					if (this.decoderStream != null)
					{
						this.decoderStream.Dispose();
						this.decoderStream = null;
					}
					if (this.inputStream != null)
					{
						this.inputStream.Dispose();
						this.inputStream = null;
					}
				}
				this.isDisposed = true;
			}
		}

		private void SetFallback()
		{
			this.currentCharsetDecoder.Fallback = new DecoderReplacementFallback("?");
			this.currentCharsetEncoder.Fallback = new EncoderReplacementFallback("?");
		}

		private const int BufferSize = 256;

		private const char ByteOrderMark1 = '￾';

		private const char ByteOrderMark2 = '﻿';

		private UnfoldingStream inputStream;

		private Stream decoderStream;

		private Encoding outerCharsetEncoding;

		private Encoding currentCharsetEncoding;

		private Encoder currentCharsetEncoder;

		private Decoder currentCharsetDecoder;

		private byte[] dataBytes;

		private char[] dataChars;

		private int bottomByte;

		private int topByte;

		private int idxByte;

		private int topChar;

		private int idxChar;

		private bool isDisposed;

		private bool swallowUTFByteOrderMark = true;

		private bool isDecoding;

		private char? lastChar = null;

		private ComplianceTracker complianceTracker;

		public delegate void OnValueEndFunc();

		private class AdapterStream : Stream
		{
			public AdapterStream(UnfoldingStream inputStream, DirectoryReader.OnValueEndFunc callback)
			{
				this.inputStream = inputStream;
				this.callback = callback;
			}

			public override bool CanRead
			{
				get
				{
					this.CheckDisposed("CanRead:get");
					return true;
				}
			}

			public override bool CanWrite
			{
				get
				{
					this.CheckDisposed("CanWrite:get");
					return false;
				}
			}

			public override bool CanSeek
			{
				get
				{
					this.CheckDisposed("CanSeek:get");
					return false;
				}
			}

			public override long Length
			{
				get
				{
					this.CheckDisposed("Length:Get");
					throw new NotSupportedException();
				}
			}

			public override long Position
			{
				get
				{
					this.CheckDisposed("Position:get");
					return (long)this.position;
				}
				set
				{
					this.CheckDisposed("Position:set");
					throw new NotSupportedException();
				}
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing && !this.isClosed)
				{
					byte[] array = new byte[1024];
					while (this.Read(array, 0, array.Length) > 0)
					{
					}
				}
				this.isClosed = true;
				base.Dispose(disposing);
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				this.CheckDisposed("Write");
				throw new NotSupportedException();
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				int num = this.InternalRead(buffer, offset, count);
				if (num == 0 && this.callback != null)
				{
					this.callback();
					this.callback = null;
				}
				return num;
			}

			public override void SetLength(long value)
			{
				this.CheckDisposed("SetLength");
				throw new NotSupportedException();
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				this.CheckDisposed("Seek");
				throw new NotSupportedException();
			}

			public override void Flush()
			{
				this.CheckDisposed("Flush");
				throw new NotSupportedException();
			}

			private int InternalRead(byte[] buffer, int offset, int count)
			{
				this.CheckDisposed("Read");
				if (this.endIdx >= 0)
				{
					int num = Math.Min(count, this.endIdx - this.idx1);
					Array.Copy(this.tempBuffer, this.idx1, buffer, offset, num);
					this.idx1 += num;
					this.position += num;
					return num;
				}
				if (this.idx1 != 0)
				{
					for (int i = 0; i < this.idx2 - this.idx1; i++)
					{
						this.tempBuffer[i] = this.tempBuffer[this.idx1 + i];
					}
					this.idx2 -= this.idx1;
					this.idx1 = 0;
				}
				int num2 = this.inputStream.Read(this.tempBuffer, this.idx2, this.tempBuffer.Length - this.idx2);
				this.idx2 += num2;
				if (num2 == 0)
				{
					this.endIdx = ((this.idx2 >= 2 && this.tempBuffer[this.idx2 - 2] == 13 && this.tempBuffer[this.idx2 - 1] == 10) ? (this.idx2 - 2) : this.idx2);
					return this.InternalRead(buffer, offset, count);
				}
				while (this.idx1 < count && this.idx1 < this.idx2 - 1)
				{
					if (this.tempBuffer[this.idx1] == 13 && this.tempBuffer[this.idx1 + 1] == 10)
					{
						this.endIdx = this.idx1;
						this.inputStream.Rewind(this.idx2 - (this.idx1 + 2));
						break;
					}
					buffer[this.idx1] = this.tempBuffer[this.idx1];
					this.idx1++;
					this.position++;
				}
				if (this.idx1 != 0)
				{
					return this.idx1;
				}
				return this.InternalRead(buffer, offset, count);
			}

			private void CheckDisposed(string methodName)
			{
				if (this.isClosed)
				{
					throw new ObjectDisposedException("AdapterStream", methodName);
				}
			}

			private bool isClosed;

			private UnfoldingStream inputStream;

			private byte[] tempBuffer = new byte[256];

			private int idx1;

			private int idx2;

			private DirectoryReader.OnValueEndFunc callback;

			private int position;

			private int endIdx = -1;
		}
	}
}
