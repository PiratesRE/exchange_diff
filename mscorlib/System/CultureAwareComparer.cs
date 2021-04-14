using System;
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	internal sealed class CultureAwareComparer : StringComparer, IWellKnownStringEqualityComparer
	{
		internal CultureAwareComparer(CultureInfo culture, bool ignoreCase)
		{
			this._compareInfo = culture.CompareInfo;
			this._ignoreCase = ignoreCase;
			this._options = (ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
		}

		internal CultureAwareComparer(CompareInfo compareInfo, bool ignoreCase)
		{
			this._compareInfo = compareInfo;
			this._ignoreCase = ignoreCase;
			this._options = (ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
		}

		internal CultureAwareComparer(CompareInfo compareInfo, CompareOptions options)
		{
			this._compareInfo = compareInfo;
			this._options = options;
			this._ignoreCase = ((options & CompareOptions.IgnoreCase) == CompareOptions.IgnoreCase || (options & CompareOptions.OrdinalIgnoreCase) == CompareOptions.OrdinalIgnoreCase);
		}

		public override int Compare(string x, string y)
		{
			this.EnsureInitialization();
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
			return this._compareInfo.Compare(x, y, this._options);
		}

		public override bool Equals(string x, string y)
		{
			this.EnsureInitialization();
			return x == y || (x != null && y != null && this._compareInfo.Compare(x, y, this._options) == 0);
		}

		public override int GetHashCode(string obj)
		{
			this.EnsureInitialization();
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			return this._compareInfo.GetHashCodeOfString(obj, this._options);
		}

		public override bool Equals(object obj)
		{
			this.EnsureInitialization();
			CultureAwareComparer cultureAwareComparer = obj as CultureAwareComparer;
			return cultureAwareComparer != null && this._ignoreCase == cultureAwareComparer._ignoreCase && this._compareInfo.Equals(cultureAwareComparer._compareInfo) && this._options == cultureAwareComparer._options;
		}

		public override int GetHashCode()
		{
			this.EnsureInitialization();
			int hashCode = this._compareInfo.GetHashCode();
			if (!this._ignoreCase)
			{
				return hashCode;
			}
			return ~hashCode;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureInitialization()
		{
			if (this._initializing)
			{
				this._options |= (this._ignoreCase ? CompareOptions.IgnoreCase : CompareOptions.None);
				this._initializing = false;
			}
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext ctx)
		{
			this._initializing = true;
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext ctx)
		{
			this.EnsureInitialization();
		}

		IEqualityComparer IWellKnownStringEqualityComparer.GetRandomizedEqualityComparer()
		{
			return new CultureAwareRandomizedComparer(this._compareInfo, this._ignoreCase);
		}

		IEqualityComparer IWellKnownStringEqualityComparer.GetEqualityComparerForSerialization()
		{
			return this;
		}

		private CompareInfo _compareInfo;

		private bool _ignoreCase;

		[OptionalField]
		private CompareOptions _options;

		[NonSerialized]
		private bool _initializing;
	}
}
