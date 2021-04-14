using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public sealed class KeySizes
	{
		public int MinSize
		{
			get
			{
				return this.m_minSize;
			}
		}

		public int MaxSize
		{
			get
			{
				return this.m_maxSize;
			}
		}

		public int SkipSize
		{
			get
			{
				return this.m_skipSize;
			}
		}

		public KeySizes(int minSize, int maxSize, int skipSize)
		{
			this.m_minSize = minSize;
			this.m_maxSize = maxSize;
			this.m_skipSize = skipSize;
		}

		private int m_minSize;

		private int m_maxSize;

		private int m_skipSize;
	}
}
