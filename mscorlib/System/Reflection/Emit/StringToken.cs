using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[Serializable]
	public struct StringToken
	{
		internal StringToken(int str)
		{
			this.m_string = str;
		}

		public int Token
		{
			get
			{
				return this.m_string;
			}
		}

		public override int GetHashCode()
		{
			return this.m_string;
		}

		public override bool Equals(object obj)
		{
			return obj is StringToken && this.Equals((StringToken)obj);
		}

		public bool Equals(StringToken obj)
		{
			return obj.m_string == this.m_string;
		}

		public static bool operator ==(StringToken a, StringToken b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(StringToken a, StringToken b)
		{
			return !(a == b);
		}

		internal int m_string;
	}
}
