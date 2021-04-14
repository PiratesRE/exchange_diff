using System;
using System.Runtime.InteropServices;

namespace System.Security.Principal
{
	[ComVisible(false)]
	public abstract class IdentityReference
	{
		internal IdentityReference()
		{
		}

		public abstract string Value { get; }

		public abstract bool IsValidTargetType(Type targetType);

		public abstract IdentityReference Translate(Type targetType);

		public abstract override bool Equals(object o);

		public abstract override int GetHashCode();

		public abstract override string ToString();

		public static bool operator ==(IdentityReference left, IdentityReference right)
		{
			return (left == null && right == null) || (left != null && right != null && left.Equals(right));
		}

		public static bool operator !=(IdentityReference left, IdentityReference right)
		{
			return !(left == right);
		}
	}
}
