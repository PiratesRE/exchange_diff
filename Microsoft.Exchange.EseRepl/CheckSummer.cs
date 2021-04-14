using System;

namespace Microsoft.Exchange.EseRepl
{
	internal class CheckSummer
	{
		public void Sum(byte[] buf, int off, int len)
		{
			int num = off;
			int i = len;
			while (i >= 4)
			{
				uint num2 = BitConverter.ToUInt32(buf, num);
				num += 4;
				i -= 4;
				this.m_csum ^= num2;
				this.m_totalBytes += 4UL;
			}
		}

		public uint GetSum()
		{
			return this.m_csum;
		}

		public ulong GetTotalBytes()
		{
			return this.m_totalBytes;
		}

		public void Reset()
		{
			this.m_csum = 0U;
			this.m_totalBytes = 0UL;
		}

		private uint m_csum;

		private ulong m_totalBytes;
	}
}
