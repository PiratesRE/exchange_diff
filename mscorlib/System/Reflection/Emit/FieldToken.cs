using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[Serializable]
	public struct FieldToken
	{
		internal FieldToken(int field, Type fieldClass)
		{
			this.m_fieldTok = field;
			this.m_class = fieldClass;
		}

		public int Token
		{
			get
			{
				return this.m_fieldTok;
			}
		}

		public override int GetHashCode()
		{
			return this.m_fieldTok;
		}

		public override bool Equals(object obj)
		{
			return obj is FieldToken && this.Equals((FieldToken)obj);
		}

		public bool Equals(FieldToken obj)
		{
			return obj.m_fieldTok == this.m_fieldTok && obj.m_class == this.m_class;
		}

		public static bool operator ==(FieldToken a, FieldToken b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(FieldToken a, FieldToken b)
		{
			return !(a == b);
		}

		public static readonly FieldToken Empty;

		internal int m_fieldTok;

		internal object m_class;
	}
}
