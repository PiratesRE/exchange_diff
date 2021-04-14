using System;
using System.Runtime.CompilerServices;
using System.Security;

namespace System.Runtime.InteropServices
{
	internal class StringBuffer : NativeBuffer
	{
		public StringBuffer(uint initialCapacity = 0U) : base((ulong)initialCapacity * 2UL)
		{
		}

		public StringBuffer(string initialContents) : base(0UL)
		{
			if (initialContents != null)
			{
				this.Append(initialContents, 0, -1);
			}
		}

		public StringBuffer(StringBuffer initialContents) : base(0UL)
		{
			if (initialContents != null)
			{
				this.Append(initialContents, 0U);
			}
		}

		public unsafe char this[uint index]
		{
			[SecuritySafeCritical]
			get
			{
				if (index >= this._length)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return this.CharPointer[(ulong)index * 2UL / 2UL];
			}
			[SecuritySafeCritical]
			set
			{
				if (index >= this._length)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				this.CharPointer[(ulong)index * 2UL / 2UL] = value;
			}
		}

		public uint CharCapacity
		{
			[SecuritySafeCritical]
			get
			{
				ulong byteCapacity = base.ByteCapacity;
				ulong num = (byteCapacity == 0UL) ? 0UL : (byteCapacity / 2UL);
				if (num <= (ulong)-1)
				{
					return (uint)num;
				}
				return uint.MaxValue;
			}
		}

		[SecuritySafeCritical]
		public void EnsureCharCapacity(uint minCapacity)
		{
			base.EnsureByteCapacity((ulong)minCapacity * 2UL);
		}

		public unsafe uint Length
		{
			get
			{
				return this._length;
			}
			[SecuritySafeCritical]
			set
			{
				if (value == 4294967295U)
				{
					throw new ArgumentOutOfRangeException("Length");
				}
				this.EnsureCharCapacity(value + 1U);
				this.CharPointer[(ulong)value * 2UL / 2UL] = '\0';
				this._length = value;
			}
		}

		[SecuritySafeCritical]
		public unsafe void SetLengthToFirstNull()
		{
			char* charPointer = this.CharPointer;
			uint charCapacity = this.CharCapacity;
			for (uint num = 0U; num < charCapacity; num += 1U)
			{
				if (charPointer[(ulong)num * 2UL / 2UL] == '\0')
				{
					this._length = num;
					return;
				}
			}
		}

		internal unsafe char* CharPointer
		{
			[SecurityCritical]
			get
			{
				return (char*)base.VoidPointer;
			}
		}

		[SecurityCritical]
		public unsafe bool Contains(char value)
		{
			char* charPointer = this.CharPointer;
			uint length = this._length;
			for (uint num = 0U; num < length; num += 1U)
			{
				if (*(charPointer++) == value)
				{
					return true;
				}
			}
			return false;
		}

		[SecuritySafeCritical]
		public bool StartsWith(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return this._length >= (uint)value.Length && this.SubstringEquals(value, 0U, value.Length);
		}

		[SecuritySafeCritical]
		public unsafe bool SubstringEquals(string value, uint startIndex = 0U, int count = -1)
		{
			if (value == null)
			{
				return false;
			}
			if (count < -1)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (startIndex > this._length)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			uint num = (count == -1) ? (this._length - startIndex) : ((uint)count);
			if (checked(startIndex + num) > this._length)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			int length = value.Length;
			if (num != (uint)length)
			{
				return false;
			}
			fixed (string text = value)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				char* ptr2 = this.CharPointer + (ulong)startIndex * 2UL / 2UL;
				for (int i = 0; i < length; i++)
				{
					if (*(ptr2++) != ptr[i])
					{
						return false;
					}
				}
			}
			return true;
		}

		[SecuritySafeCritical]
		public void Append(string value, int startIndex = 0, int count = -1)
		{
			this.CopyFrom(this._length, value, startIndex, count);
		}

		public void Append(StringBuffer value, uint startIndex = 0U)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.Length == 0U)
			{
				return;
			}
			value.CopyTo(startIndex, this, this._length, value.Length);
		}

		public void Append(StringBuffer value, uint startIndex, uint count)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (count == 0U)
			{
				return;
			}
			value.CopyTo(startIndex, this, this._length, count);
		}

		[SecuritySafeCritical]
		public unsafe void CopyTo(uint bufferIndex, StringBuffer destination, uint destinationIndex, uint count)
		{
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			if (destinationIndex > destination._length)
			{
				throw new ArgumentOutOfRangeException("destinationIndex");
			}
			if (bufferIndex >= this._length)
			{
				throw new ArgumentOutOfRangeException("bufferIndex");
			}
			checked
			{
				if (this._length < bufferIndex + count)
				{
					throw new ArgumentOutOfRangeException("count");
				}
				if (count == 0U)
				{
					return;
				}
				uint num = destinationIndex + count;
				if (destination._length < num)
				{
					destination.Length = num;
				}
				Buffer.MemoryCopy((void*)(unchecked(this.CharPointer + (ulong)bufferIndex * 2UL / 2UL)), (void*)(unchecked(destination.CharPointer + (ulong)destinationIndex * 2UL / 2UL)), (long)(destination.ByteCapacity - unchecked((ulong)(checked(destinationIndex * 2U)))), (long)(unchecked((ulong)count) * 2UL));
			}
		}

		[SecuritySafeCritical]
		public unsafe void CopyFrom(uint bufferIndex, string source, int sourceIndex = 0, int count = -1)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (bufferIndex > this._length)
			{
				throw new ArgumentOutOfRangeException("bufferIndex");
			}
			if (sourceIndex < 0 || sourceIndex >= source.Length)
			{
				throw new ArgumentOutOfRangeException("sourceIndex");
			}
			if (count == -1)
			{
				count = source.Length - sourceIndex;
			}
			if (count < 0 || source.Length - count < sourceIndex)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (count == 0)
			{
				return;
			}
			uint num = bufferIndex + (uint)count;
			if (this._length < num)
			{
				this.Length = num;
			}
			fixed (string text = source)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				Buffer.MemoryCopy((void*)(ptr + sourceIndex), (void*)(this.CharPointer + (ulong)bufferIndex * 2UL / 2UL), checked((long)(base.ByteCapacity - unchecked((ulong)(checked(bufferIndex * 2U))))), (long)count * 2L);
			}
		}

		[SecuritySafeCritical]
		public unsafe void TrimEnd(char[] values)
		{
			if (values == null || values.Length == 0 || this._length == 0U)
			{
				return;
			}
			char* ptr = this.CharPointer + (ulong)this._length * 2UL / 2UL - 1;
			while (this._length > 0U && Array.IndexOf<char>(values, *ptr) >= 0)
			{
				this.Length = this._length - 1U;
				ptr--;
			}
		}

		[SecuritySafeCritical]
		public override string ToString()
		{
			if (this._length == 0U)
			{
				return string.Empty;
			}
			if (this._length > 2147483647U)
			{
				throw new InvalidOperationException();
			}
			return new string(this.CharPointer, 0, (int)this._length);
		}

		[SecuritySafeCritical]
		public string Substring(uint startIndex, int count = -1)
		{
			if (startIndex > ((this._length == 0U) ? 0U : (this._length - 1U)))
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			if (count < -1)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			uint num = (count == -1) ? (this._length - startIndex) : ((uint)count);
			if (num > 2147483647U || checked(startIndex + num) > this._length)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (num == 0U)
			{
				return string.Empty;
			}
			return new string(this.CharPointer + (ulong)startIndex * 2UL / 2UL, 0, (int)num);
		}

		[SecuritySafeCritical]
		public override void Free()
		{
			base.Free();
			this._length = 0U;
		}

		private uint _length;
	}
}
