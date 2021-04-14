using System;
using System.Collections;
using System.Globalization;
using System.Security;

namespace System
{
	internal sealed class OrdinalRandomizedComparer : StringComparer, IWellKnownStringEqualityComparer
	{
		internal OrdinalRandomizedComparer(bool ignoreCase)
		{
			this._ignoreCase = ignoreCase;
			this._entropy = HashHelpers.GetEntropy();
		}

		public override int Compare(string x, string y)
		{
			if (x == y)
			{
				return 0;
			}
			if (x == null)
			{
				return -1;
			}
			if (y == null)
			{
				return 1;
			}
			if (this._ignoreCase)
			{
				return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
			}
			return string.CompareOrdinal(x, y);
		}

		public override bool Equals(string x, string y)
		{
			if (x == y)
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}
			if (this._ignoreCase)
			{
				return x.Length == y.Length && string.Compare(x, y, StringComparison.OrdinalIgnoreCase) == 0;
			}
			return x.Equals(y);
		}

		[SecuritySafeCritical]
		public override int GetHashCode(string obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (this._ignoreCase)
			{
				return TextInfo.GetHashCodeOrdinalIgnoreCase(obj, true, this._entropy);
			}
			return string.InternalMarvin32HashString(obj, obj.Length, this._entropy);
		}

		public override bool Equals(object obj)
		{
			OrdinalRandomizedComparer ordinalRandomizedComparer = obj as OrdinalRandomizedComparer;
			return ordinalRandomizedComparer != null && this._ignoreCase == ordinalRandomizedComparer._ignoreCase && this._entropy == ordinalRandomizedComparer._entropy;
		}

		public override int GetHashCode()
		{
			string text = "OrdinalRandomizedComparer";
			int hashCode = text.GetHashCode();
			return (this._ignoreCase ? (~hashCode) : hashCode) ^ (int)(this._entropy & 2147483647L);
		}

		IEqualityComparer IWellKnownStringEqualityComparer.GetRandomizedEqualityComparer()
		{
			return new OrdinalRandomizedComparer(this._ignoreCase);
		}

		IEqualityComparer IWellKnownStringEqualityComparer.GetEqualityComparerForSerialization()
		{
			return new OrdinalComparer(this._ignoreCase);
		}

		private bool _ignoreCase;

		private long _entropy;
	}
}
