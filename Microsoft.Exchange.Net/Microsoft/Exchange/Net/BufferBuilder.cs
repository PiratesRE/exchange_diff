using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Microsoft.Exchange.Net
{
	internal class BufferBuilder
	{
		internal BufferBuilder() : this(256)
		{
		}

		internal BufferBuilder(int capacity)
		{
			this.buffer = new byte[capacity];
		}

		internal int Capacity
		{
			get
			{
				return this.buffer.Length;
			}
		}

		internal int Length
		{
			get
			{
				return this.offset;
			}
		}

		internal bool Secure
		{
			get
			{
				return this.secure;
			}
			set
			{
				if (!value)
				{
					throw new InvalidOperationException();
				}
				this.secure = value;
			}
		}

		public override string ToString()
		{
			if (this.secure)
			{
				throw new InvalidOperationException();
			}
			return Encoding.ASCII.GetString(this.buffer, 0, this.offset);
		}

		internal void Append(byte value)
		{
			this.EnsureBuffer(1);
			this.buffer[this.offset++] = value;
		}

		internal void Append(byte[] value)
		{
			this.Append(value, 0, value.Length);
		}

		internal void Append(byte[] value, int offset, int count)
		{
			this.EnsureBuffer(count);
			Buffer.BlockCopy(value, offset, this.buffer, this.offset, count);
			this.offset += count;
		}

		internal void Append(string value)
		{
			this.Append(value, 0, value.Length);
		}

		internal void Append(string value, int offset, int count)
		{
			this.EnsureBuffer(count);
			for (int i = 0; i < count; i++)
			{
				char c = value[offset + i];
				if (c > 'ÿ')
				{
					throw new ArgumentException(NetException.StringContainsInvalidCharacters, "value");
				}
				this.buffer[this.offset + i] = (byte)c;
			}
			this.offset += count;
		}

		internal void Append(SecureString value)
		{
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				this.secure = true;
				this.EnsureBuffer(value.Length);
				intPtr = Marshal.SecureStringToCoTaskMemUnicode(value);
				for (int i = 0; i < value.Length; i++)
				{
					char c = (char)Marshal.ReadInt16(intPtr, i * Marshal.SizeOf(typeof(short)));
					if (c > 'ÿ')
					{
						throw new ArgumentException(NetException.StringContainsInvalidCharacters, "value");
					}
					this.Append((byte)c);
				}
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.ZeroFreeCoTaskMemUnicode(intPtr);
				}
			}
		}

		internal byte[] GetBuffer()
		{
			return this.buffer;
		}

		internal void RemoveUnusedBufferSpace()
		{
			if (this.offset != this.buffer.Length)
			{
				byte[] dst = new byte[this.offset];
				Buffer.BlockCopy(this.buffer, 0, dst, 0, this.offset);
				if (this.secure)
				{
					Array.Clear(this.buffer, 0, this.buffer.Length);
				}
				this.buffer = dst;
			}
		}

		internal void Reset()
		{
			this.offset = 0;
			if (this.secure)
			{
				Array.Clear(this.buffer, 0, this.buffer.Length);
				this.secure = false;
			}
		}

		internal void SetLength(int newLength)
		{
			if (newLength > this.buffer.Length)
			{
				this.EnsureBuffer(newLength - this.buffer.Length);
			}
			this.offset = newLength;
		}

		protected void EnsureBuffer(int count)
		{
			if (count > this.buffer.Length - this.offset)
			{
				byte[] dst = new byte[Math.Max(this.buffer.Length * 2, this.buffer.Length + count)];
				Buffer.BlockCopy(this.buffer, 0, dst, 0, this.offset);
				if (this.secure)
				{
					Array.Clear(this.buffer, 0, this.buffer.Length);
				}
				this.buffer = dst;
			}
		}

		private byte[] buffer;

		private int offset;

		private bool secure;
	}
}
