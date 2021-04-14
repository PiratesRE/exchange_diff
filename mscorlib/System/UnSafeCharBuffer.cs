using System;
using System.Runtime.CompilerServices;
using System.Security;

namespace System
{
	internal struct UnSafeCharBuffer
	{
		[SecurityCritical]
		public unsafe UnSafeCharBuffer(char* buffer, int bufferSize)
		{
			this.m_buffer = buffer;
			this.m_totalSize = bufferSize;
			this.m_length = 0;
		}

		[SecuritySafeCritical]
		public unsafe void AppendString(string stringToAppend)
		{
			if (string.IsNullOrEmpty(stringToAppend))
			{
				return;
			}
			if (this.m_totalSize - this.m_length < stringToAppend.Length)
			{
				throw new IndexOutOfRangeException();
			}
			fixed (string text = stringToAppend)
			{
				char* ptr = text;
				if (ptr != null)
				{
					ptr += RuntimeHelpers.OffsetToStringData / 2;
				}
				Buffer.Memcpy((byte*)(this.m_buffer + this.m_length), (byte*)ptr, stringToAppend.Length * 2);
			}
			this.m_length += stringToAppend.Length;
		}

		public int Length
		{
			get
			{
				return this.m_length;
			}
		}

		[SecurityCritical]
		private unsafe char* m_buffer;

		private int m_totalSize;

		private int m_length;
	}
}
