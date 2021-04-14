using System;
using System.Runtime.CompilerServices;
using System.Security;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public struct ArrayWithOffset
	{
		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public ArrayWithOffset(object array, int offset)
		{
			this.m_array = array;
			this.m_offset = offset;
			this.m_count = 0;
			this.m_count = this.CalculateCount();
		}

		[__DynamicallyInvokable]
		public object GetArray()
		{
			return this.m_array;
		}

		[__DynamicallyInvokable]
		public int GetOffset()
		{
			return this.m_offset;
		}

		[__DynamicallyInvokable]
		public override int GetHashCode()
		{
			return this.m_count + this.m_offset;
		}

		[__DynamicallyInvokable]
		public override bool Equals(object obj)
		{
			return obj is ArrayWithOffset && this.Equals((ArrayWithOffset)obj);
		}

		[__DynamicallyInvokable]
		public bool Equals(ArrayWithOffset obj)
		{
			return obj.m_array == this.m_array && obj.m_offset == this.m_offset && obj.m_count == this.m_count;
		}

		[__DynamicallyInvokable]
		public static bool operator ==(ArrayWithOffset a, ArrayWithOffset b)
		{
			return a.Equals(b);
		}

		[__DynamicallyInvokable]
		public static bool operator !=(ArrayWithOffset a, ArrayWithOffset b)
		{
			return !(a == b);
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int CalculateCount();

		private object m_array;

		private int m_offset;

		private int m_count;
	}
}
