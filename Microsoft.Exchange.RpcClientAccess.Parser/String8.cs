using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal sealed class String8
	{
		public String8(ArraySegment<byte> encodedBytes)
		{
			this.encodedBytes = encodedBytes;
		}

		public String8(string resolvedString)
		{
			if (resolvedString == null)
			{
				throw new ArgumentNullException("resolvedString");
			}
			this.resolvedString = resolvedString;
		}

		public string StringValue
		{
			get
			{
				if (this.resolvedString == null)
				{
					throw new InvalidOperationException("String8 is unresolved");
				}
				return this.resolvedString;
			}
		}

		public static String8 Parse(Reader reader, bool useUnicode, StringFlags flags)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (useUnicode)
			{
				return new String8(reader.ReadUnicodeString(flags));
			}
			return reader.ReadString8(flags);
		}

		public static String8 Create(string resolvedString)
		{
			if (resolvedString == null)
			{
				return null;
			}
			return new String8(resolvedString);
		}

		public override string ToString()
		{
			if (this.resolvedString != null)
			{
				return this.resolvedString;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("UnresolvedString8: ");
			Util.AppendToString(stringBuilder, this.encodedBytes.Array, this.encodedBytes.Offset, this.encodedBytes.Count);
			return stringBuilder.ToString();
		}

		internal void ResolveString8Values(Encoding string8Encoding)
		{
			if (this.resolvedString == null)
			{
				if (string8Encoding == null)
				{
					throw new ArgumentNullException("string8Encoding");
				}
				String8Encodings.ThrowIfInvalidString8Encoding(string8Encoding);
				int num = this.encodedBytes.Count;
				if (num != 0 && this.encodedBytes.Array[this.encodedBytes.Offset + num - 1] == 0)
				{
					num--;
				}
				this.resolvedString = string8Encoding.GetString(this.encodedBytes.Array, this.encodedBytes.Offset, num);
				this.encodedBytes = default(ArraySegment<byte>);
			}
		}

		private ArraySegment<byte> encodedBytes;

		private string resolvedString;
	}
}
