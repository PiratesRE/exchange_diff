using System;
using System.Security;

namespace System.Reflection
{
	[Serializable]
	internal struct ConstArray
	{
		public IntPtr Signature
		{
			get
			{
				return this.m_constArray;
			}
		}

		public int Length
		{
			get
			{
				return this.m_length;
			}
		}

		public unsafe byte this[int index]
		{
			[SecuritySafeCritical]
			get
			{
				if (index < 0 || index >= this.m_length)
				{
					throw new IndexOutOfRangeException();
				}
				return ((byte*)this.m_constArray.ToPointer())[index];
			}
		}

		internal int m_length;

		internal IntPtr m_constArray;
	}
}
