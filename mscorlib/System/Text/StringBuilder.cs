using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace System.Text
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public sealed class StringBuilder : ISerializable
	{
		[__DynamicallyInvokable]
		public StringBuilder() : this(16)
		{
		}

		[__DynamicallyInvokable]
		public StringBuilder(int capacity) : this(string.Empty, capacity)
		{
		}

		[__DynamicallyInvokable]
		public StringBuilder(string value) : this(value, 16)
		{
		}

		[__DynamicallyInvokable]
		public StringBuilder(string value, int capacity) : this(value, 0, (value != null) ? value.Length : 0, capacity)
		{
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public unsafe StringBuilder(string value, int startIndex, int length, int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity", Environment.GetResourceString("ArgumentOutOfRange_MustBePositive", new object[]
				{
					"capacity"
				}));
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", Environment.GetResourceString("ArgumentOutOfRange_MustBeNonNegNum", new object[]
				{
					"length"
				}));
			}
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_StartIndex"));
			}
			if (value == null)
			{
				value = string.Empty;
			}
			if (startIndex > value.Length - length)
			{
				throw new ArgumentOutOfRangeException("length", Environment.GetResourceString("ArgumentOutOfRange_IndexLength"));
			}
			this.m_MaxCapacity = int.MaxValue;
			if (capacity == 0)
			{
				capacity = 16;
			}
			if (capacity < length)
			{
				capacity = length;
			}
			this.m_ChunkChars = new char[capacity];
			this.m_ChunkLength = length;
			fixed (string text = value)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				StringBuilder.ThreadSafeCopy(ptr + startIndex, this.m_ChunkChars, 0, length);
			}
		}

		[__DynamicallyInvokable]
		public StringBuilder(int capacity, int maxCapacity)
		{
			if (capacity > maxCapacity)
			{
				throw new ArgumentOutOfRangeException("capacity", Environment.GetResourceString("ArgumentOutOfRange_Capacity"));
			}
			if (maxCapacity < 1)
			{
				throw new ArgumentOutOfRangeException("maxCapacity", Environment.GetResourceString("ArgumentOutOfRange_SmallMaxCapacity"));
			}
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity", Environment.GetResourceString("ArgumentOutOfRange_MustBePositive", new object[]
				{
					"capacity"
				}));
			}
			if (capacity == 0)
			{
				capacity = Math.Min(16, maxCapacity);
			}
			this.m_MaxCapacity = maxCapacity;
			this.m_ChunkChars = new char[capacity];
		}

		[SecurityCritical]
		private StringBuilder(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			int num = 0;
			string text = null;
			int num2 = int.MaxValue;
			bool flag = false;
			SerializationInfoEnumerator enumerator = info.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string name = enumerator.Name;
				if (!(name == "m_MaxCapacity"))
				{
					if (!(name == "m_StringValue"))
					{
						if (name == "Capacity")
						{
							num = info.GetInt32("Capacity");
							flag = true;
						}
					}
					else
					{
						text = info.GetString("m_StringValue");
					}
				}
				else
				{
					num2 = info.GetInt32("m_MaxCapacity");
				}
			}
			if (text == null)
			{
				text = string.Empty;
			}
			if (num2 < 1 || text.Length > num2)
			{
				throw new SerializationException(Environment.GetResourceString("Serialization_StringBuilderMaxCapacity"));
			}
			if (!flag)
			{
				num = 16;
				if (num < text.Length)
				{
					num = text.Length;
				}
				if (num > num2)
				{
					num = num2;
				}
			}
			if (num < 0 || num < text.Length || num > num2)
			{
				throw new SerializationException(Environment.GetResourceString("Serialization_StringBuilderCapacity"));
			}
			this.m_MaxCapacity = num2;
			this.m_ChunkChars = new char[num];
			text.CopyTo(0, this.m_ChunkChars, 0, text.Length);
			this.m_ChunkLength = text.Length;
			this.m_ChunkPrevious = null;
		}

		[SecurityCritical]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("m_MaxCapacity", this.m_MaxCapacity);
			info.AddValue("Capacity", this.Capacity);
			info.AddValue("m_StringValue", this.ToString());
			info.AddValue("m_currentThread", 0);
		}

		[Conditional("_DEBUG")]
		private void VerifyClassInvariant()
		{
			StringBuilder stringBuilder = this;
			int maxCapacity = this.m_MaxCapacity;
			for (;;)
			{
				StringBuilder chunkPrevious = stringBuilder.m_ChunkPrevious;
				if (chunkPrevious == null)
				{
					break;
				}
				stringBuilder = chunkPrevious;
			}
		}

		[__DynamicallyInvokable]
		public int Capacity
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_ChunkChars.Length + this.m_ChunkOffset;
			}
			[__DynamicallyInvokable]
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("ArgumentOutOfRange_NegativeCapacity"));
				}
				if (value > this.MaxCapacity)
				{
					throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("ArgumentOutOfRange_Capacity"));
				}
				if (value < this.Length)
				{
					throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("ArgumentOutOfRange_SmallCapacity"));
				}
				if (this.Capacity != value)
				{
					int num = value - this.m_ChunkOffset;
					char[] array = new char[num];
					Array.Copy(this.m_ChunkChars, array, this.m_ChunkLength);
					this.m_ChunkChars = array;
				}
			}
		}

		[__DynamicallyInvokable]
		public int MaxCapacity
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_MaxCapacity;
			}
		}

		[__DynamicallyInvokable]
		public int EnsureCapacity(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity", Environment.GetResourceString("ArgumentOutOfRange_NegativeCapacity"));
			}
			if (this.Capacity < capacity)
			{
				this.Capacity = capacity;
			}
			return this.Capacity;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public unsafe override string ToString()
		{
			if (this.Length == 0)
			{
				return string.Empty;
			}
			string text = string.FastAllocateString(this.Length);
			StringBuilder stringBuilder = this;
			fixed (string text2 = text)
			{
				char* ptr = text2;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				for (;;)
				{
					if (stringBuilder.m_ChunkLength > 0)
					{
						char[] chunkChars = stringBuilder.m_ChunkChars;
						int chunkOffset = stringBuilder.m_ChunkOffset;
						int chunkLength = stringBuilder.m_ChunkLength;
						if ((ulong)(chunkLength + chunkOffset) > (ulong)((long)text.Length) || chunkLength > chunkChars.Length)
						{
							break;
						}
						fixed (char* ptr2 = chunkChars)
						{
							string.wstrcpy(ptr + chunkOffset, ptr2, chunkLength);
						}
					}
					stringBuilder = stringBuilder.m_ChunkPrevious;
					if (stringBuilder == null)
					{
						goto Block_7;
					}
				}
				throw new ArgumentOutOfRangeException("chunkLength", Environment.GetResourceString("ArgumentOutOfRange_Index"));
				Block_7:;
			}
			return text;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public unsafe string ToString(int startIndex, int length)
		{
			int length2 = this.Length;
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_StartIndex"));
			}
			if (startIndex > length2)
			{
				throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_StartIndexLargerThanLength"));
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", Environment.GetResourceString("ArgumentOutOfRange_NegativeLength"));
			}
			if (startIndex > length2 - length)
			{
				throw new ArgumentOutOfRangeException("length", Environment.GetResourceString("ArgumentOutOfRange_IndexLength"));
			}
			StringBuilder stringBuilder = this;
			int num = startIndex + length;
			string text = string.FastAllocateString(length);
			int i = length;
			fixed (string text2 = text)
			{
				char* ptr = text2;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				while (i > 0)
				{
					int num2 = num - stringBuilder.m_ChunkOffset;
					if (num2 >= 0)
					{
						if (num2 > stringBuilder.m_ChunkLength)
						{
							num2 = stringBuilder.m_ChunkLength;
						}
						int num3 = i;
						int num4 = num3;
						int num5 = num2 - num3;
						if (num5 < 0)
						{
							num4 += num5;
							num5 = 0;
						}
						i -= num4;
						if (num4 > 0)
						{
							char[] chunkChars = stringBuilder.m_ChunkChars;
							if ((ulong)(num4 + i) > (ulong)((long)length) || num4 + num5 > chunkChars.Length)
							{
								throw new ArgumentOutOfRangeException("chunkCount", Environment.GetResourceString("ArgumentOutOfRange_Index"));
							}
							fixed (char* ptr2 = &chunkChars[num5])
							{
								string.wstrcpy(ptr + i, ptr2, num4);
							}
						}
					}
					stringBuilder = stringBuilder.m_ChunkPrevious;
				}
			}
			return text;
		}

		[__DynamicallyInvokable]
		public StringBuilder Clear()
		{
			this.Length = 0;
			return this;
		}

		[__DynamicallyInvokable]
		public int Length
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_ChunkOffset + this.m_ChunkLength;
			}
			[__DynamicallyInvokable]
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("ArgumentOutOfRange_NegativeLength"));
				}
				if (value > this.MaxCapacity)
				{
					throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("ArgumentOutOfRange_SmallCapacity"));
				}
				int capacity = this.Capacity;
				if (value == 0 && this.m_ChunkPrevious == null)
				{
					this.m_ChunkLength = 0;
					this.m_ChunkOffset = 0;
					return;
				}
				int num = value - this.Length;
				if (num > 0)
				{
					this.Append('\0', num);
					return;
				}
				StringBuilder stringBuilder = this.FindChunkForIndex(value);
				if (stringBuilder != this)
				{
					int num2 = capacity - stringBuilder.m_ChunkOffset;
					char[] array = new char[num2];
					Array.Copy(stringBuilder.m_ChunkChars, array, stringBuilder.m_ChunkLength);
					this.m_ChunkChars = array;
					this.m_ChunkPrevious = stringBuilder.m_ChunkPrevious;
					this.m_ChunkOffset = stringBuilder.m_ChunkOffset;
				}
				this.m_ChunkLength = value - stringBuilder.m_ChunkOffset;
			}
		}

		[__DynamicallyInvokable]
		[IndexerName("Chars")]
		public char this[int index]
		{
			[__DynamicallyInvokable]
			get
			{
				StringBuilder stringBuilder = this;
				int num;
				for (;;)
				{
					num = index - stringBuilder.m_ChunkOffset;
					if (num >= 0)
					{
						break;
					}
					stringBuilder = stringBuilder.m_ChunkPrevious;
					if (stringBuilder == null)
					{
						goto Block_3;
					}
				}
				if (num >= stringBuilder.m_ChunkLength)
				{
					throw new IndexOutOfRangeException();
				}
				return stringBuilder.m_ChunkChars[num];
				Block_3:
				throw new IndexOutOfRangeException();
			}
			[__DynamicallyInvokable]
			set
			{
				StringBuilder stringBuilder = this;
				int num;
				for (;;)
				{
					num = index - stringBuilder.m_ChunkOffset;
					if (num >= 0)
					{
						break;
					}
					stringBuilder = stringBuilder.m_ChunkPrevious;
					if (stringBuilder == null)
					{
						goto Block_3;
					}
				}
				if (num >= stringBuilder.m_ChunkLength)
				{
					throw new ArgumentOutOfRangeException("index", Environment.GetResourceString("ArgumentOutOfRange_Index"));
				}
				stringBuilder.m_ChunkChars[num] = value;
				return;
				Block_3:
				throw new ArgumentOutOfRangeException("index", Environment.GetResourceString("ArgumentOutOfRange_Index"));
			}
		}

		[__DynamicallyInvokable]
		public StringBuilder Append(char value, int repeatCount)
		{
			if (repeatCount < 0)
			{
				throw new ArgumentOutOfRangeException("repeatCount", Environment.GetResourceString("ArgumentOutOfRange_NegativeCount"));
			}
			if (repeatCount == 0)
			{
				return this;
			}
			int num = this.m_ChunkLength;
			while (repeatCount > 0)
			{
				if (num < this.m_ChunkChars.Length)
				{
					this.m_ChunkChars[num++] = value;
					repeatCount--;
				}
				else
				{
					this.m_ChunkLength = num;
					this.ExpandByABlock(repeatCount);
					num = 0;
				}
			}
			this.m_ChunkLength = num;
			return this;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public unsafe StringBuilder Append(char[] value, int startIndex, int charCount)
		{
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_GenericPositive"));
			}
			if (charCount < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_GenericPositive"));
			}
			if (CompatibilitySwitches.IsAppEarlierThanWindowsPhone8 && charCount == 0)
			{
				return this;
			}
			if (value == null)
			{
				if (startIndex == 0 && charCount == 0)
				{
					return this;
				}
				throw new ArgumentNullException("value");
			}
			else
			{
				if (charCount > value.Length - startIndex)
				{
					throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_Index"));
				}
				if (charCount == 0)
				{
					return this;
				}
				fixed (char* ptr = &value[startIndex])
				{
					this.Append(ptr, charCount);
				}
				return this;
			}
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public unsafe StringBuilder Append(string value)
		{
			if (value != null)
			{
				char[] chunkChars = this.m_ChunkChars;
				int chunkLength = this.m_ChunkLength;
				int length = value.Length;
				int num = chunkLength + length;
				if (num < chunkChars.Length)
				{
					if (length <= 2)
					{
						if (length > 0)
						{
							chunkChars[chunkLength] = value[0];
						}
						if (length > 1)
						{
							chunkChars[chunkLength + 1] = value[1];
						}
					}
					else
					{
						fixed (string text = value)
						{
							char* ptr = text;
							if (ptr != null)
							{
								ptr += RuntimeHelpers.OffsetToStringData / 2;
							}
							fixed (char* ptr2 = &chunkChars[chunkLength])
							{
								string.wstrcpy(ptr2, ptr, length);
							}
						}
					}
					this.m_ChunkLength = num;
				}
				else
				{
					this.AppendHelper(value);
				}
			}
			return this;
		}

		[SecuritySafeCritical]
		private unsafe void AppendHelper(string value)
		{
			fixed (string text = value)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				this.Append(ptr, value.Length);
			}
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe extern void ReplaceBufferInternal(char* newBuffer, int newLength);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe extern void ReplaceBufferAnsiInternal(sbyte* newBuffer, int newLength);

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public unsafe StringBuilder Append(string value, int startIndex, int count)
		{
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_Index"));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_GenericPositive"));
			}
			if (CompatibilitySwitches.IsAppEarlierThanWindowsPhone8 && count == 0)
			{
				return this;
			}
			if (value == null)
			{
				if (startIndex == 0 && count == 0)
				{
					return this;
				}
				throw new ArgumentNullException("value");
			}
			else
			{
				if (count == 0)
				{
					return this;
				}
				if (startIndex > value.Length - count)
				{
					throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_Index"));
				}
				fixed (string text = value)
				{
					char* ptr = text;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					this.Append(ptr + startIndex, count);
				}
				return this;
			}
		}

		[ComVisible(false)]
		[__DynamicallyInvokable]
		public StringBuilder AppendLine()
		{
			return this.Append(Environment.NewLine);
		}

		[ComVisible(false)]
		[__DynamicallyInvokable]
		public StringBuilder AppendLine(string value)
		{
			this.Append(value);
			return this.Append(Environment.NewLine);
		}

		[ComVisible(false)]
		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
		{
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("Arg_NegativeArgCount"));
			}
			if (destinationIndex < 0)
			{
				throw new ArgumentOutOfRangeException("destinationIndex", Environment.GetResourceString("ArgumentOutOfRange_MustBeNonNegNum", new object[]
				{
					"destinationIndex"
				}));
			}
			if (destinationIndex > destination.Length - count)
			{
				throw new ArgumentException(Environment.GetResourceString("ArgumentOutOfRange_OffsetOut"));
			}
			if (sourceIndex > this.Length)
			{
				throw new ArgumentOutOfRangeException("sourceIndex", Environment.GetResourceString("ArgumentOutOfRange_Index"));
			}
			if (sourceIndex > this.Length - count)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_LongerThanSrcString"));
			}
			StringBuilder stringBuilder = this;
			int num = sourceIndex + count;
			int num2 = destinationIndex + count;
			while (count > 0)
			{
				int num3 = num - stringBuilder.m_ChunkOffset;
				if (num3 >= 0)
				{
					if (num3 > stringBuilder.m_ChunkLength)
					{
						num3 = stringBuilder.m_ChunkLength;
					}
					int num4 = count;
					int num5 = num3 - count;
					if (num5 < 0)
					{
						num4 += num5;
						num5 = 0;
					}
					num2 -= num4;
					count -= num4;
					StringBuilder.ThreadSafeCopy(stringBuilder.m_ChunkChars, num5, destination, num2, num4);
				}
				stringBuilder = stringBuilder.m_ChunkPrevious;
			}
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public unsafe StringBuilder Insert(int index, string value, int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			int length = this.Length;
			if (index > length)
			{
				throw new ArgumentOutOfRangeException("index", Environment.GetResourceString("ArgumentOutOfRange_Index"));
			}
			if (value == null || value.Length == 0 || count == 0)
			{
				return this;
			}
			long num = (long)value.Length * (long)count;
			if (num > (long)(this.MaxCapacity - this.Length))
			{
				throw new OutOfMemoryException();
			}
			StringBuilder stringBuilder;
			int num2;
			this.MakeRoom(index, (int)num, out stringBuilder, out num2, false);
			fixed (string text = value)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				while (count > 0)
				{
					this.ReplaceInPlaceAtChunk(ref stringBuilder, ref num2, ptr, value.Length);
					count--;
				}
			}
			return this;
		}

		[__DynamicallyInvokable]
		public StringBuilder Remove(int startIndex, int length)
		{
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", Environment.GetResourceString("ArgumentOutOfRange_NegativeLength"));
			}
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_StartIndex"));
			}
			if (length > this.Length - startIndex)
			{
				throw new ArgumentOutOfRangeException("index", Environment.GetResourceString("ArgumentOutOfRange_Index"));
			}
			if (this.Length == length && startIndex == 0)
			{
				this.Length = 0;
				return this;
			}
			if (length > 0)
			{
				StringBuilder stringBuilder;
				int num;
				this.Remove(startIndex, length, out stringBuilder, out num);
			}
			return this;
		}

		[__DynamicallyInvokable]
		public StringBuilder Append(bool value)
		{
			return this.Append(value.ToString());
		}

		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public StringBuilder Append(sbyte value)
		{
			return this.Append(value.ToString(CultureInfo.CurrentCulture));
		}

		[__DynamicallyInvokable]
		public StringBuilder Append(byte value)
		{
			return this.Append(value.ToString(CultureInfo.CurrentCulture));
		}

		[__DynamicallyInvokable]
		public StringBuilder Append(char value)
		{
			if (this.m_ChunkLength < this.m_ChunkChars.Length)
			{
				char[] chunkChars = this.m_ChunkChars;
				int chunkLength = this.m_ChunkLength;
				this.m_ChunkLength = chunkLength + 1;
				chunkChars[chunkLength] = value;
			}
			else
			{
				this.Append(value, 1);
			}
			return this;
		}

		[__DynamicallyInvokable]
		public StringBuilder Append(short value)
		{
			return this.Append(value.ToString(CultureInfo.CurrentCulture));
		}

		[__DynamicallyInvokable]
		public StringBuilder Append(int value)
		{
			return this.Append(value.ToString(CultureInfo.CurrentCulture));
		}

		[__DynamicallyInvokable]
		public StringBuilder Append(long value)
		{
			return this.Append(value.ToString(CultureInfo.CurrentCulture));
		}

		[__DynamicallyInvokable]
		public StringBuilder Append(float value)
		{
			return this.Append(value.ToString(CultureInfo.CurrentCulture));
		}

		[__DynamicallyInvokable]
		public StringBuilder Append(double value)
		{
			return this.Append(value.ToString(CultureInfo.CurrentCulture));
		}

		[__DynamicallyInvokable]
		public StringBuilder Append(decimal value)
		{
			return this.Append(value.ToString(CultureInfo.CurrentCulture));
		}

		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public StringBuilder Append(ushort value)
		{
			return this.Append(value.ToString(CultureInfo.CurrentCulture));
		}

		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public StringBuilder Append(uint value)
		{
			return this.Append(value.ToString(CultureInfo.CurrentCulture));
		}

		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public StringBuilder Append(ulong value)
		{
			return this.Append(value.ToString(CultureInfo.CurrentCulture));
		}

		[__DynamicallyInvokable]
		public StringBuilder Append(object value)
		{
			if (value == null)
			{
				return this;
			}
			return this.Append(value.ToString());
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public unsafe StringBuilder Append(char[] value)
		{
			if (value != null && value.Length != 0)
			{
				fixed (char* ptr = &value[0])
				{
					this.Append(ptr, value.Length);
				}
			}
			return this;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public unsafe StringBuilder Insert(int index, string value)
		{
			if (index > this.Length)
			{
				throw new ArgumentOutOfRangeException("index", Environment.GetResourceString("ArgumentOutOfRange_Index"));
			}
			if (value != null)
			{
				fixed (string text = value)
				{
					char* ptr = text;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					this.Insert(index, ptr, value.Length);
				}
			}
			return this;
		}

		[__DynamicallyInvokable]
		public StringBuilder Insert(int index, bool value)
		{
			return this.Insert(index, value.ToString(), 1);
		}

		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public StringBuilder Insert(int index, sbyte value)
		{
			return this.Insert(index, value.ToString(CultureInfo.CurrentCulture), 1);
		}

		[__DynamicallyInvokable]
		public StringBuilder Insert(int index, byte value)
		{
			return this.Insert(index, value.ToString(CultureInfo.CurrentCulture), 1);
		}

		[__DynamicallyInvokable]
		public StringBuilder Insert(int index, short value)
		{
			return this.Insert(index, value.ToString(CultureInfo.CurrentCulture), 1);
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public unsafe StringBuilder Insert(int index, char value)
		{
			this.Insert(index, &value, 1);
			return this;
		}

		[__DynamicallyInvokable]
		public StringBuilder Insert(int index, char[] value)
		{
			if (index > this.Length)
			{
				throw new ArgumentOutOfRangeException("index", Environment.GetResourceString("ArgumentOutOfRange_Index"));
			}
			if (value != null)
			{
				this.Insert(index, value, 0, value.Length);
			}
			return this;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public unsafe StringBuilder Insert(int index, char[] value, int startIndex, int charCount)
		{
			int length = this.Length;
			if (index > length)
			{
				throw new ArgumentOutOfRangeException("index", Environment.GetResourceString("ArgumentOutOfRange_Index"));
			}
			if (value == null)
			{
				if (startIndex == 0 && charCount == 0)
				{
					return this;
				}
				throw new ArgumentNullException(Environment.GetResourceString("ArgumentNull_String"));
			}
			else
			{
				if (startIndex < 0)
				{
					throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_StartIndex"));
				}
				if (charCount < 0)
				{
					throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_GenericPositive"));
				}
				if (startIndex > value.Length - charCount)
				{
					throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_Index"));
				}
				if (charCount > 0)
				{
					fixed (char* ptr = &value[startIndex])
					{
						this.Insert(index, ptr, charCount);
					}
				}
				return this;
			}
		}

		[__DynamicallyInvokable]
		public StringBuilder Insert(int index, int value)
		{
			return this.Insert(index, value.ToString(CultureInfo.CurrentCulture), 1);
		}

		[__DynamicallyInvokable]
		public StringBuilder Insert(int index, long value)
		{
			return this.Insert(index, value.ToString(CultureInfo.CurrentCulture), 1);
		}

		[__DynamicallyInvokable]
		public StringBuilder Insert(int index, float value)
		{
			return this.Insert(index, value.ToString(CultureInfo.CurrentCulture), 1);
		}

		[__DynamicallyInvokable]
		public StringBuilder Insert(int index, double value)
		{
			return this.Insert(index, value.ToString(CultureInfo.CurrentCulture), 1);
		}

		[__DynamicallyInvokable]
		public StringBuilder Insert(int index, decimal value)
		{
			return this.Insert(index, value.ToString(CultureInfo.CurrentCulture), 1);
		}

		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public StringBuilder Insert(int index, ushort value)
		{
			return this.Insert(index, value.ToString(CultureInfo.CurrentCulture), 1);
		}

		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public StringBuilder Insert(int index, uint value)
		{
			return this.Insert(index, value.ToString(CultureInfo.CurrentCulture), 1);
		}

		[CLSCompliant(false)]
		[__DynamicallyInvokable]
		public StringBuilder Insert(int index, ulong value)
		{
			return this.Insert(index, value.ToString(CultureInfo.CurrentCulture), 1);
		}

		[__DynamicallyInvokable]
		public StringBuilder Insert(int index, object value)
		{
			if (value == null)
			{
				return this;
			}
			return this.Insert(index, value.ToString(), 1);
		}

		[__DynamicallyInvokable]
		public StringBuilder AppendFormat(string format, object arg0)
		{
			return this.AppendFormatHelper(null, format, new ParamsArray(arg0));
		}

		[__DynamicallyInvokable]
		public StringBuilder AppendFormat(string format, object arg0, object arg1)
		{
			return this.AppendFormatHelper(null, format, new ParamsArray(arg0, arg1));
		}

		[__DynamicallyInvokable]
		public StringBuilder AppendFormat(string format, object arg0, object arg1, object arg2)
		{
			return this.AppendFormatHelper(null, format, new ParamsArray(arg0, arg1, arg2));
		}

		[__DynamicallyInvokable]
		public StringBuilder AppendFormat(string format, params object[] args)
		{
			if (args == null)
			{
				throw new ArgumentNullException((format == null) ? "format" : "args");
			}
			return this.AppendFormatHelper(null, format, new ParamsArray(args));
		}

		[__DynamicallyInvokable]
		public StringBuilder AppendFormat(IFormatProvider provider, string format, object arg0)
		{
			return this.AppendFormatHelper(provider, format, new ParamsArray(arg0));
		}

		[__DynamicallyInvokable]
		public StringBuilder AppendFormat(IFormatProvider provider, string format, object arg0, object arg1)
		{
			return this.AppendFormatHelper(provider, format, new ParamsArray(arg0, arg1));
		}

		[__DynamicallyInvokable]
		public StringBuilder AppendFormat(IFormatProvider provider, string format, object arg0, object arg1, object arg2)
		{
			return this.AppendFormatHelper(provider, format, new ParamsArray(arg0, arg1, arg2));
		}

		[__DynamicallyInvokable]
		public StringBuilder AppendFormat(IFormatProvider provider, string format, params object[] args)
		{
			if (args == null)
			{
				throw new ArgumentNullException((format == null) ? "format" : "args");
			}
			return this.AppendFormatHelper(provider, format, new ParamsArray(args));
		}

		private static void FormatError()
		{
			throw new FormatException(Environment.GetResourceString("Format_InvalidString"));
		}

		internal StringBuilder AppendFormatHelper(IFormatProvider provider, string format, ParamsArray args)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			int i = 0;
			int length = format.Length;
			char c = '\0';
			ICustomFormatter customFormatter = null;
			if (provider != null)
			{
				customFormatter = (ICustomFormatter)provider.GetFormat(typeof(ICustomFormatter));
			}
			for (;;)
			{
				while (i < length)
				{
					c = format[i];
					i++;
					if (c == '}')
					{
						if (i < length && format[i] == '}')
						{
							i++;
						}
						else
						{
							StringBuilder.FormatError();
						}
					}
					if (c == '{')
					{
						if (i >= length || format[i] != '{')
						{
							i--;
							break;
						}
						i++;
					}
					this.Append(c);
				}
				if (i == length)
				{
					return this;
				}
				i++;
				if (i == length || (c = format[i]) < '0' || c > '9')
				{
					StringBuilder.FormatError();
				}
				int num = 0;
				do
				{
					num = num * 10 + (int)c - 48;
					i++;
					if (i == length)
					{
						StringBuilder.FormatError();
					}
					c = format[i];
				}
				while (c >= '0' && c <= '9' && num < 1000000);
				if (num >= args.Length)
				{
					break;
				}
				while (i < length && (c = format[i]) == ' ')
				{
					i++;
				}
				bool flag = false;
				int num2 = 0;
				if (c == ',')
				{
					i++;
					while (i < length && format[i] == ' ')
					{
						i++;
					}
					if (i == length)
					{
						StringBuilder.FormatError();
					}
					c = format[i];
					if (c == '-')
					{
						flag = true;
						i++;
						if (i == length)
						{
							StringBuilder.FormatError();
						}
						c = format[i];
					}
					if (c < '0' || c > '9')
					{
						StringBuilder.FormatError();
					}
					do
					{
						num2 = num2 * 10 + (int)c - 48;
						i++;
						if (i == length)
						{
							StringBuilder.FormatError();
						}
						c = format[i];
						if (c < '0' || c > '9')
						{
							break;
						}
					}
					while (num2 < 1000000);
				}
				while (i < length && (c = format[i]) == ' ')
				{
					i++;
				}
				object obj = args[num];
				StringBuilder stringBuilder = null;
				if (c == ':')
				{
					i++;
					for (;;)
					{
						if (i == length)
						{
							StringBuilder.FormatError();
						}
						c = format[i];
						i++;
						if (c == '{')
						{
							if (i < length && format[i] == '{')
							{
								i++;
							}
							else
							{
								StringBuilder.FormatError();
							}
						}
						else if (c == '}')
						{
							if (i >= length || format[i] != '}')
							{
								break;
							}
							i++;
						}
						if (stringBuilder == null)
						{
							stringBuilder = new StringBuilder();
						}
						stringBuilder.Append(c);
					}
					i--;
				}
				if (c != '}')
				{
					StringBuilder.FormatError();
				}
				i++;
				string text = null;
				string text2 = null;
				if (customFormatter != null)
				{
					if (stringBuilder != null)
					{
						text = stringBuilder.ToString();
					}
					text2 = customFormatter.Format(text, obj, provider);
				}
				if (text2 == null)
				{
					IFormattable formattable = obj as IFormattable;
					if (formattable != null)
					{
						if (text == null && stringBuilder != null)
						{
							text = stringBuilder.ToString();
						}
						text2 = formattable.ToString(text, provider);
					}
					else if (obj != null)
					{
						text2 = obj.ToString();
					}
				}
				if (text2 == null)
				{
					text2 = string.Empty;
				}
				int num3 = num2 - text2.Length;
				if (!flag && num3 > 0)
				{
					this.Append(' ', num3);
				}
				this.Append(text2);
				if (flag && num3 > 0)
				{
					this.Append(' ', num3);
				}
			}
			throw new FormatException(Environment.GetResourceString("Format_IndexOutOfRange"));
		}

		[__DynamicallyInvokable]
		public StringBuilder Replace(string oldValue, string newValue)
		{
			return this.Replace(oldValue, newValue, 0, this.Length);
		}

		[__DynamicallyInvokable]
		public bool Equals(StringBuilder sb)
		{
			if (sb == null)
			{
				return false;
			}
			if (this.Capacity != sb.Capacity || this.MaxCapacity != sb.MaxCapacity || this.Length != sb.Length)
			{
				return false;
			}
			if (sb == this)
			{
				return true;
			}
			StringBuilder stringBuilder = this;
			int i = stringBuilder.m_ChunkLength;
			StringBuilder stringBuilder2 = sb;
			int j = stringBuilder2.m_ChunkLength;
			for (;;)
			{
				IL_49:
				i--;
				j--;
				while (i < 0)
				{
					stringBuilder = stringBuilder.m_ChunkPrevious;
					if (stringBuilder != null)
					{
						i = stringBuilder.m_ChunkLength + i;
					}
					else
					{
						IL_7F:
						while (j < 0)
						{
							stringBuilder2 = stringBuilder2.m_ChunkPrevious;
							if (stringBuilder2 == null)
							{
								break;
							}
							j = stringBuilder2.m_ChunkLength + j;
						}
						if (i < 0)
						{
							goto Block_8;
						}
						if (j < 0)
						{
							return false;
						}
						if (stringBuilder.m_ChunkChars[i] != stringBuilder2.m_ChunkChars[j])
						{
							return false;
						}
						goto IL_49;
					}
				}
				goto IL_7F;
			}
			Block_8:
			return j < 0;
		}

		[__DynamicallyInvokable]
		public StringBuilder Replace(string oldValue, string newValue, int startIndex, int count)
		{
			int length = this.Length;
			if (startIndex > length)
			{
				throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_Index"));
			}
			if (count < 0 || startIndex > length - count)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_Index"));
			}
			if (oldValue == null)
			{
				throw new ArgumentNullException("oldValue");
			}
			if (oldValue.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyName"), "oldValue");
			}
			if (newValue == null)
			{
				newValue = "";
			}
			int num = newValue.Length - oldValue.Length;
			int[] array = null;
			int num2 = 0;
			StringBuilder stringBuilder = this.FindChunkForIndex(startIndex);
			int num3 = startIndex - stringBuilder.m_ChunkOffset;
			while (count > 0)
			{
				if (this.StartsWith(stringBuilder, num3, count, oldValue))
				{
					if (array == null)
					{
						array = new int[5];
					}
					else if (num2 >= array.Length)
					{
						int[] array2 = new int[array.Length * 3 / 2 + 4];
						Array.Copy(array, array2, array.Length);
						array = array2;
					}
					array[num2++] = num3;
					num3 += oldValue.Length;
					count -= oldValue.Length;
				}
				else
				{
					num3++;
					count--;
				}
				if (num3 >= stringBuilder.m_ChunkLength || count == 0)
				{
					int num4 = num3 + stringBuilder.m_ChunkOffset;
					this.ReplaceAllInChunk(array, num2, stringBuilder, oldValue.Length, newValue);
					num4 += (newValue.Length - oldValue.Length) * num2;
					num2 = 0;
					stringBuilder = this.FindChunkForIndex(num4);
					num3 = num4 - stringBuilder.m_ChunkOffset;
				}
			}
			return this;
		}

		[__DynamicallyInvokable]
		public StringBuilder Replace(char oldChar, char newChar)
		{
			return this.Replace(oldChar, newChar, 0, this.Length);
		}

		[__DynamicallyInvokable]
		public StringBuilder Replace(char oldChar, char newChar, int startIndex, int count)
		{
			int length = this.Length;
			if (startIndex > length)
			{
				throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_Index"));
			}
			if (count < 0 || startIndex > length - count)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_Index"));
			}
			int num = startIndex + count;
			StringBuilder stringBuilder = this;
			for (;;)
			{
				int num2 = num - stringBuilder.m_ChunkOffset;
				int num3 = startIndex - stringBuilder.m_ChunkOffset;
				if (num2 >= 0)
				{
					int i = Math.Max(num3, 0);
					int num4 = Math.Min(stringBuilder.m_ChunkLength, num2);
					while (i < num4)
					{
						if (stringBuilder.m_ChunkChars[i] == oldChar)
						{
							stringBuilder.m_ChunkChars[i] = newChar;
						}
						i++;
					}
				}
				if (num3 >= 0)
				{
					break;
				}
				stringBuilder = stringBuilder.m_ChunkPrevious;
			}
			return this;
		}

		[SecurityCritical]
		[CLSCompliant(false)]
		public unsafe StringBuilder Append(char* value, int valueCount)
		{
			if (valueCount < 0)
			{
				throw new ArgumentOutOfRangeException("valueCount", Environment.GetResourceString("ArgumentOutOfRange_NegativeCount"));
			}
			int num = valueCount + this.m_ChunkLength;
			if (num <= this.m_ChunkChars.Length)
			{
				StringBuilder.ThreadSafeCopy(value, this.m_ChunkChars, this.m_ChunkLength, valueCount);
				this.m_ChunkLength = num;
			}
			else
			{
				int num2 = this.m_ChunkChars.Length - this.m_ChunkLength;
				if (num2 > 0)
				{
					StringBuilder.ThreadSafeCopy(value, this.m_ChunkChars, this.m_ChunkLength, num2);
					this.m_ChunkLength = this.m_ChunkChars.Length;
				}
				int num3 = valueCount - num2;
				this.ExpandByABlock(num3);
				StringBuilder.ThreadSafeCopy(value + num2, this.m_ChunkChars, 0, num3);
				this.m_ChunkLength = num3;
			}
			return this;
		}

		[SecurityCritical]
		private unsafe void Insert(int index, char* value, int valueCount)
		{
			if (index > this.Length)
			{
				throw new ArgumentOutOfRangeException("index", Environment.GetResourceString("ArgumentOutOfRange_Index"));
			}
			if (valueCount > 0)
			{
				StringBuilder stringBuilder;
				int num;
				this.MakeRoom(index, valueCount, out stringBuilder, out num, false);
				this.ReplaceInPlaceAtChunk(ref stringBuilder, ref num, value, valueCount);
			}
		}

		[SecuritySafeCritical]
		private unsafe void ReplaceAllInChunk(int[] replacements, int replacementsCount, StringBuilder sourceChunk, int removeCount, string value)
		{
			if (replacementsCount <= 0)
			{
				return;
			}
			fixed (string text = value)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				int num = (value.Length - removeCount) * replacementsCount;
				StringBuilder stringBuilder = sourceChunk;
				int num2 = replacements[0];
				if (num > 0)
				{
					this.MakeRoom(stringBuilder.m_ChunkOffset + num2, num, out stringBuilder, out num2, true);
				}
				int num3 = 0;
				for (;;)
				{
					this.ReplaceInPlaceAtChunk(ref stringBuilder, ref num2, ptr, value.Length);
					int num4 = replacements[num3] + removeCount;
					num3++;
					if (num3 >= replacementsCount)
					{
						break;
					}
					int num5 = replacements[num3];
					if (num != 0)
					{
						fixed (char* ptr2 = &sourceChunk.m_ChunkChars[num4])
						{
							this.ReplaceInPlaceAtChunk(ref stringBuilder, ref num2, ptr2, num5 - num4);
						}
					}
					else
					{
						num2 += num5 - num4;
					}
				}
				if (num < 0)
				{
					this.Remove(stringBuilder.m_ChunkOffset + num2, -num, out stringBuilder, out num2);
				}
			}
		}

		private bool StartsWith(StringBuilder chunk, int indexInChunk, int count, string value)
		{
			for (int i = 0; i < value.Length; i++)
			{
				if (count == 0)
				{
					return false;
				}
				if (indexInChunk >= chunk.m_ChunkLength)
				{
					chunk = this.Next(chunk);
					if (chunk == null)
					{
						return false;
					}
					indexInChunk = 0;
				}
				if (value[i] != chunk.m_ChunkChars[indexInChunk])
				{
					return false;
				}
				indexInChunk++;
				count--;
			}
			return true;
		}

		[SecurityCritical]
		private unsafe void ReplaceInPlaceAtChunk(ref StringBuilder chunk, ref int indexInChunk, char* value, int count)
		{
			if (count != 0)
			{
				for (;;)
				{
					int val = chunk.m_ChunkLength - indexInChunk;
					int num = Math.Min(val, count);
					StringBuilder.ThreadSafeCopy(value, chunk.m_ChunkChars, indexInChunk, num);
					indexInChunk += num;
					if (indexInChunk >= chunk.m_ChunkLength)
					{
						chunk = this.Next(chunk);
						indexInChunk = 0;
					}
					count -= num;
					if (count == 0)
					{
						break;
					}
					value += num;
				}
			}
		}

		[SecurityCritical]
		private unsafe static void ThreadSafeCopy(char* sourcePtr, char[] destination, int destinationIndex, int count)
		{
			if (count <= 0)
			{
				return;
			}
			if (destinationIndex <= destination.Length && destinationIndex + count <= destination.Length)
			{
				fixed (char* ptr = &destination[destinationIndex])
				{
					string.wstrcpy(ptr, sourcePtr, count);
				}
				return;
			}
			throw new ArgumentOutOfRangeException("destinationIndex", Environment.GetResourceString("ArgumentOutOfRange_Index"));
		}

		[SecurityCritical]
		private unsafe static void ThreadSafeCopy(char[] source, int sourceIndex, char[] destination, int destinationIndex, int count)
		{
			if (count <= 0)
			{
				return;
			}
			if (sourceIndex <= source.Length && sourceIndex + count <= source.Length)
			{
				fixed (char* ptr = &source[sourceIndex])
				{
					StringBuilder.ThreadSafeCopy(ptr, destination, destinationIndex, count);
				}
				return;
			}
			throw new ArgumentOutOfRangeException("sourceIndex", Environment.GetResourceString("ArgumentOutOfRange_Index"));
		}

		[SecurityCritical]
		internal unsafe void InternalCopy(IntPtr dest, int len)
		{
			if (len == 0)
			{
				return;
			}
			bool flag = true;
			byte* ptr = (byte*)dest.ToPointer();
			StringBuilder stringBuilder = this.FindChunkForByte(len);
			do
			{
				int num = stringBuilder.m_ChunkOffset * 2;
				int len2 = stringBuilder.m_ChunkLength * 2;
				fixed (char* ptr2 = &stringBuilder.m_ChunkChars[0])
				{
					byte* src = (byte*)ptr2;
					if (flag)
					{
						flag = false;
						Buffer.Memcpy(ptr + num, src, len - num);
					}
					else
					{
						Buffer.Memcpy(ptr + num, src, len2);
					}
				}
				stringBuilder = stringBuilder.m_ChunkPrevious;
			}
			while (stringBuilder != null);
		}

		private StringBuilder FindChunkForIndex(int index)
		{
			StringBuilder stringBuilder = this;
			while (stringBuilder.m_ChunkOffset > index)
			{
				stringBuilder = stringBuilder.m_ChunkPrevious;
			}
			return stringBuilder;
		}

		private StringBuilder FindChunkForByte(int byteIndex)
		{
			StringBuilder stringBuilder = this;
			while (stringBuilder.m_ChunkOffset * 2 > byteIndex)
			{
				stringBuilder = stringBuilder.m_ChunkPrevious;
			}
			return stringBuilder;
		}

		private StringBuilder Next(StringBuilder chunk)
		{
			if (chunk == this)
			{
				return null;
			}
			return this.FindChunkForIndex(chunk.m_ChunkOffset + chunk.m_ChunkLength);
		}

		private void ExpandByABlock(int minBlockCharCount)
		{
			if (minBlockCharCount + this.Length < minBlockCharCount || minBlockCharCount + this.Length > this.m_MaxCapacity)
			{
				throw new ArgumentOutOfRangeException("requiredLength", Environment.GetResourceString("ArgumentOutOfRange_SmallCapacity"));
			}
			int num = Math.Max(minBlockCharCount, Math.Min(this.Length, 8000));
			this.m_ChunkPrevious = new StringBuilder(this);
			this.m_ChunkOffset += this.m_ChunkLength;
			this.m_ChunkLength = 0;
			if (this.m_ChunkOffset + num < num)
			{
				this.m_ChunkChars = null;
				throw new OutOfMemoryException();
			}
			this.m_ChunkChars = new char[num];
		}

		private StringBuilder(StringBuilder from)
		{
			this.m_ChunkLength = from.m_ChunkLength;
			this.m_ChunkOffset = from.m_ChunkOffset;
			this.m_ChunkChars = from.m_ChunkChars;
			this.m_ChunkPrevious = from.m_ChunkPrevious;
			this.m_MaxCapacity = from.m_MaxCapacity;
		}

		[SecuritySafeCritical]
		private unsafe void MakeRoom(int index, int count, out StringBuilder chunk, out int indexInChunk, bool doneMoveFollowingChars)
		{
			if (count + this.Length < count || count + this.Length > this.m_MaxCapacity)
			{
				throw new ArgumentOutOfRangeException("requiredLength", Environment.GetResourceString("ArgumentOutOfRange_SmallCapacity"));
			}
			chunk = this;
			while (chunk.m_ChunkOffset > index)
			{
				chunk.m_ChunkOffset += count;
				chunk = chunk.m_ChunkPrevious;
			}
			indexInChunk = index - chunk.m_ChunkOffset;
			if (!doneMoveFollowingChars && chunk.m_ChunkLength <= 32 && chunk.m_ChunkChars.Length - chunk.m_ChunkLength >= count)
			{
				int i = chunk.m_ChunkLength;
				while (i > indexInChunk)
				{
					i--;
					chunk.m_ChunkChars[i + count] = chunk.m_ChunkChars[i];
				}
				chunk.m_ChunkLength += count;
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(Math.Max(count, 16), chunk.m_MaxCapacity, chunk.m_ChunkPrevious);
			stringBuilder.m_ChunkLength = count;
			int num = Math.Min(count, indexInChunk);
			if (num > 0)
			{
				fixed (char* chunkChars = chunk.m_ChunkChars)
				{
					StringBuilder.ThreadSafeCopy(chunkChars, stringBuilder.m_ChunkChars, 0, num);
					int num2 = indexInChunk - num;
					if (num2 >= 0)
					{
						StringBuilder.ThreadSafeCopy(chunkChars + num, chunk.m_ChunkChars, 0, num2);
						indexInChunk = num2;
					}
				}
			}
			chunk.m_ChunkPrevious = stringBuilder;
			chunk.m_ChunkOffset += count;
			if (num < count)
			{
				chunk = stringBuilder;
				indexInChunk = num;
			}
		}

		private StringBuilder(int size, int maxCapacity, StringBuilder previousBlock)
		{
			this.m_ChunkChars = new char[size];
			this.m_MaxCapacity = maxCapacity;
			this.m_ChunkPrevious = previousBlock;
			if (previousBlock != null)
			{
				this.m_ChunkOffset = previousBlock.m_ChunkOffset + previousBlock.m_ChunkLength;
			}
		}

		[SecuritySafeCritical]
		private void Remove(int startIndex, int count, out StringBuilder chunk, out int indexInChunk)
		{
			int num = startIndex + count;
			chunk = this;
			StringBuilder stringBuilder = null;
			int num2 = 0;
			for (;;)
			{
				if (num - chunk.m_ChunkOffset >= 0)
				{
					if (stringBuilder == null)
					{
						stringBuilder = chunk;
						num2 = num - stringBuilder.m_ChunkOffset;
					}
					if (startIndex - chunk.m_ChunkOffset >= 0)
					{
						break;
					}
				}
				else
				{
					chunk.m_ChunkOffset -= count;
				}
				chunk = chunk.m_ChunkPrevious;
			}
			indexInChunk = startIndex - chunk.m_ChunkOffset;
			int num3 = indexInChunk;
			int count2 = stringBuilder.m_ChunkLength - num2;
			if (stringBuilder != chunk)
			{
				num3 = 0;
				chunk.m_ChunkLength = indexInChunk;
				stringBuilder.m_ChunkPrevious = chunk;
				stringBuilder.m_ChunkOffset = chunk.m_ChunkOffset + chunk.m_ChunkLength;
				if (indexInChunk == 0)
				{
					stringBuilder.m_ChunkPrevious = chunk.m_ChunkPrevious;
					chunk = stringBuilder;
				}
			}
			stringBuilder.m_ChunkLength -= num2 - num3;
			if (num3 != num2)
			{
				StringBuilder.ThreadSafeCopy(stringBuilder.m_ChunkChars, num2, stringBuilder.m_ChunkChars, num3, count2);
			}
		}

		internal char[] m_ChunkChars;

		internal StringBuilder m_ChunkPrevious;

		internal int m_ChunkLength;

		internal int m_ChunkOffset;

		internal int m_MaxCapacity;

		internal const int DefaultCapacity = 16;

		private const string CapacityField = "Capacity";

		private const string MaxCapacityField = "m_MaxCapacity";

		private const string StringValueField = "m_StringValue";

		private const string ThreadIDField = "m_currentThread";

		internal const int MaxChunkSize = 8000;
	}
}
