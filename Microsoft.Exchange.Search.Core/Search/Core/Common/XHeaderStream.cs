using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal class XHeaderStream : Stream
	{
		public XHeaderStream(Action<string, string> setHeader)
		{
			Util.ThrowOnNullArgument(setHeader, "setHeader");
			this.setHeader = setHeader;
		}

		public XHeaderStream(Func<string, string> getHeader)
		{
			Util.ThrowOnNullArgument(getHeader, "getHeader");
			this.getHeader = getHeader;
			string text = this.getHeader("X-MS-Exchange-Forest-IndexAgent");
			if (!XHeaderStream.TryParseVersionHeader(text, out this.length))
			{
				throw new InvalidDataException("InvalidVersion: " + text);
			}
		}

		public override bool CanRead
		{
			get
			{
				return this.getHeader != null;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.setHeader != null;
			}
		}

		public override long Length
		{
			get
			{
				return this.length;
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (!this.CanRead)
			{
				throw new NotSupportedException("Read");
			}
			int num = 0;
			while (count > 0 && this.position != this.length)
			{
				if (this.bufferPosition == this.buffer.Length)
				{
					string text = XHeaderStream.FormatHeaderName(this.currentHeader++);
					string text2 = this.getHeader(text);
					if (string.IsNullOrEmpty(text2))
					{
						throw new IOException("Read:" + text);
					}
					try
					{
						text2 = text2.Replace(" ", string.Empty);
						this.buffer = Convert.FromBase64String(text2);
					}
					catch (FormatException innerException)
					{
						throw new IOException("Base64:" + text, innerException);
					}
					this.bufferPosition = 0;
				}
				int num2 = Math.Min(count, this.buffer.Length - this.bufferPosition);
				Buffer.BlockCopy(this.buffer, this.bufferPosition, buffer, offset, num2);
				this.bufferPosition += num2;
				this.position += (long)num2;
				offset += num2;
				count -= num2;
				num += num2;
			}
			return num;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (!this.CanWrite)
			{
				throw new NotSupportedException("Write");
			}
			if (count > 20000)
			{
				throw new ArgumentException("count");
			}
			string arg = XHeaderStream.FormatHeaderName(this.currentHeader++);
			string text = Convert.ToBase64String(buffer, offset, count);
			int capacity = text.Length + (text.Length + 54 - 1) / 54 - 1;
			StringBuilder stringBuilder = new StringBuilder(capacity);
			int i = text.Length;
			while (i > 0)
			{
				int num = Math.Min(i, 54);
				stringBuilder.Append(text, offset, num);
				offset += num;
				i -= num;
				if (i != 0)
				{
					stringBuilder.Append(' ');
				}
			}
			this.setHeader(arg, stringBuilder.ToString());
			this.length += (long)count;
			this.position += (long)count;
		}

		public override void Flush()
		{
			if (!this.CanWrite)
			{
				throw new NotSupportedException("Flush");
			}
			string arg = string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
			{
				1,
				this.length
			});
			this.setHeader("X-MS-Exchange-Forest-IndexAgent", arg);
		}

		internal static bool IsVersionSupported(string versionString)
		{
			long num;
			return XHeaderStream.TryParseVersionHeader(versionString, out num);
		}

		internal static bool TryParseVersionHeader(string versionString, out long length)
		{
			length = 0L;
			if (string.IsNullOrEmpty(versionString))
			{
				return false;
			}
			string[] array = versionString.Split(null, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length != 2)
			{
				return false;
			}
			long[] array2 = new long[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (!long.TryParse(array[i], NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo, out array2[i]))
				{
					return false;
				}
			}
			if (array2[0] != 1L)
			{
				return false;
			}
			length = array2[1];
			return true;
		}

		internal static string FormatHeaderName(int headerNumber)
		{
			return string.Format(CultureInfo.InvariantCulture, "X-MS-Exchange-Forest-IndexAgent-{0}", new object[]
			{
				headerNumber
			});
		}

		internal void RemoveHeaders()
		{
			if (!this.CanWrite)
			{
				throw new NotSupportedException("RemoveHeaders");
			}
			this.setHeader("X-MS-Exchange-Forest-IndexAgent", null);
			for (int i = 0; i < this.currentHeader; i++)
			{
				this.setHeader(XHeaderStream.FormatHeaderName(i), null);
			}
		}

		protected override void Dispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (this.CanWrite && this.length > 0L)
				{
					this.Flush();
				}
				this.getHeader = null;
				this.setHeader = null;
			}
			base.Dispose(calledFromDispose);
		}

		internal const int MaxBufferSize = 20000;

		internal const int Base64CharsPerLine = 72;

		internal const int BytesPerLine = 54;

		internal const string VersionHeader = "X-MS-Exchange-Forest-IndexAgent";

		private const string Prefix = "X-MS-Exchange-Forest-IndexAgent";

		private const int VersionNumber = 1;

		private const string VersionFormat = "{0} {1}";

		private const string HeaderNameFormat = "X-MS-Exchange-Forest-IndexAgent-{0}";

		private static readonly byte[] EmptyBuffer = new byte[0];

		private int currentHeader;

		private long length;

		private long position;

		private int bufferPosition;

		private byte[] buffer = XHeaderStream.EmptyBuffer;

		private Func<string, string> getHeader;

		private Action<string, string> setHeader;
	}
}
