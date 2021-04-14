using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Audio
{
	internal class WindowsMediaBuffer : IDisposable
	{
		internal WindowsMediaBuffer(INSSBuffer inssBuffer)
		{
			this.inssBuffer = inssBuffer;
			this.inssBuffer.GetBufferAndLength(out this.bufferPtr, out this.length);
			this.inssBuffer.GetMaxLength(out this.maxLength);
		}

		internal uint Length
		{
			get
			{
				return this.length;
			}
		}

		internal uint Position
		{
			get
			{
				return this.position;
			}
		}

		public void Dispose()
		{
			Marshal.ReleaseComObject(this.inssBuffer);
			this.inssBuffer = null;
		}

		internal int Read(byte[] buffer, int offset, int numBytes)
		{
			int num = 0;
			if (this.position < this.length)
			{
				long num2 = (long)((ulong)(this.length - this.position));
				IntPtr source = (IntPtr)(this.bufferPtr.ToInt64() + (long)((ulong)this.position));
				num = Math.Min(numBytes, (int)num2);
				num = Math.Min(num, buffer.Length);
				if (num > 0)
				{
					Marshal.Copy(source, buffer, offset, num);
					this.position += (uint)num;
				}
				else
				{
					num = 0;
				}
			}
			return num;
		}

		internal void Write(byte[] buffer, int count)
		{
			IntPtr destination = (IntPtr)(this.bufferPtr.ToInt64() + (long)((ulong)this.position));
			Marshal.Copy(buffer, 0, destination, count);
			this.position += (uint)count;
		}

		private INSSBuffer inssBuffer;

		private uint length;

		private uint maxLength;

		private IntPtr bufferPtr;

		private uint position;
	}
}
