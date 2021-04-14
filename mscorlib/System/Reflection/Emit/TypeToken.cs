using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[Serializable]
	public struct TypeToken
	{
		internal TypeToken(int str)
		{
			this.m_class = str;
		}

		public int Token
		{
			get
			{
				return this.m_class;
			}
		}

		public override int GetHashCode()
		{
			return this.m_class;
		}

		public override bool Equals(object obj)
		{
			return obj is TypeToken && this.Equals((TypeToken)obj);
		}

		public bool Equals(TypeToken obj)
		{
			return obj.m_class == this.m_class;
		}

		public static bool operator ==(TypeToken a, TypeToken b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(TypeToken a, TypeToken b)
		{
			return !(a == b);
		}

		public static readonly TypeToken Empty;

		internal int m_class;
	}
}
