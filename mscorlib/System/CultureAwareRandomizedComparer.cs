using System;
using System.Collections;
using System.Globalization;

namespace System
{
	internal sealed class CultureAwareRandomizedComparer : StringComparer, IWellKnownStringEqualityComparer
	{
		internal CultureAwareRandomizedComparer(CompareInfo compareInfo, bool ignoreCase)
		{
			this._compareInfo = compareInfo;
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
			return this._compareInfo.Compare(x, y, this._ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
		}

		public override bool Equals(string x, string y)
		{
			return x == y || (x != null && y != null && this._compareInfo.Compare(x, y, this._ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None) == 0);
		}

		public override int GetHashCode(string obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			CompareOptions compareOptions = CompareOptions.None;
			if (this._ignoreCase)
			{
				compareOptions |= CompareOptions.IgnoreCase;
			}
			return this._compareInfo.GetHashCodeOfString(obj, compareOptions, true, this._entropy);
		}

		public override bool Equals(object obj)
		{
			CultureAwareRandomizedComparer cultureAwareRandomizedComparer = obj as CultureAwareRandomizedComparer;
			return cultureAwareRandomizedComparer != null && (this._ignoreCase == cultureAwareRandomizedComparer._ignoreCase && this._compareInfo.Equals(cultureAwareRandomizedComparer._compareInfo)) && this._entropy == cultureAwareRandomizedComparer._entropy;
		}

		public override int GetHashCode()
		{
			int hashCode = this._compareInfo.GetHashCode();
			return (this._ignoreCase ? (~hashCode) : hashCode) ^ (int)(this._entropy & 2147483647L);
		}

		IEqualityComparer IWellKnownStringEqualityComparer.GetRandomizedEqualityComparer()
		{
			return new CultureAwareRandomizedComparer(this._compareInfo, this._ignoreCase);
		}

		IEqualityComparer IWellKnownStringEqualityComparer.GetEqualityComparerForSerialization()
		{
			return new CultureAwareComparer(this._compareInfo, this._ignoreCase);
		}

		private CompareInfo _compareInfo;

		private bool _ignoreCase;

		private long _entropy;
	}
}
