using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[Serializable]
	public struct PropertyToken
	{
		internal PropertyToken(int str)
		{
			this.m_property = str;
		}

		public int Token
		{
			get
			{
				return this.m_property;
			}
		}

		public override int GetHashCode()
		{
			return this.m_property;
		}

		public override bool Equals(object obj)
		{
			return obj is PropertyToken && this.Equals((PropertyToken)obj);
		}

		public bool Equals(PropertyToken obj)
		{
			return obj.m_property == this.m_property;
		}

		public static bool operator ==(PropertyToken a, PropertyToken b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(PropertyToken a, PropertyToken b)
		{
			return !(a == b);
		}

		public static readonly PropertyToken Empty;

		internal int m_property;
	}
}
