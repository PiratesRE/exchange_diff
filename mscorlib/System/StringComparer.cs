using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public abstract class StringComparer : IComparer, IEqualityComparer, IComparer<string>, IEqualityComparer<string>
	{
		public static StringComparer InvariantCulture
		{
			get
			{
				return StringComparer._invariantCulture;
			}
		}

		public static StringComparer InvariantCultureIgnoreCase
		{
			get
			{
				return StringComparer._invariantCultureIgnoreCase;
			}
		}

		[__DynamicallyInvokable]
		public static StringComparer CurrentCulture
		{
			[__DynamicallyInvokable]
			get
			{
				return new CultureAwareComparer(CultureInfo.CurrentCulture, false);
			}
		}

		[__DynamicallyInvokable]
		public static StringComparer CurrentCultureIgnoreCase
		{
			[__DynamicallyInvokable]
			get
			{
				return new CultureAwareComparer(CultureInfo.CurrentCulture, true);
			}
		}

		[__DynamicallyInvokable]
		public static StringComparer Ordinal
		{
			[__DynamicallyInvokable]
			get
			{
				return StringComparer._ordinal;
			}
		}

		[__DynamicallyInvokable]
		public static StringComparer OrdinalIgnoreCase
		{
			[__DynamicallyInvokable]
			get
			{
				return StringComparer._ordinalIgnoreCase;
			}
		}

		[__DynamicallyInvokable]
		public static StringComparer Create(CultureInfo culture, bool ignoreCase)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			return new CultureAwareComparer(culture, ignoreCase);
		}

		public int Compare(object x, object y)
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
			string text = x as string;
			if (text != null)
			{
				string text2 = y as string;
				if (text2 != null)
				{
					return this.Compare(text, text2);
				}
			}
			IComparable comparable = x as IComparable;
			if (comparable != null)
			{
				return comparable.CompareTo(y);
			}
			throw new ArgumentException(Environment.GetResourceString("Argument_ImplementIComparable"));
		}

		public bool Equals(object x, object y)
		{
			if (x == y)
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}
			string text = x as string;
			if (text != null)
			{
				string text2 = y as string;
				if (text2 != null)
				{
					return this.Equals(text, text2);
				}
			}
			return x.Equals(y);
		}

		public int GetHashCode(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			string text = obj as string;
			if (text != null)
			{
				return this.GetHashCode(text);
			}
			return obj.GetHashCode();
		}

		[__DynamicallyInvokable]
		public abstract int Compare(string x, string y);

		[__DynamicallyInvokable]
		public abstract bool Equals(string x, string y);

		[__DynamicallyInvokable]
		public abstract int GetHashCode(string obj);

		[__DynamicallyInvokable]
		protected StringComparer()
		{
		}

		private static readonly StringComparer _invariantCulture = new CultureAwareComparer(CultureInfo.InvariantCulture, false);

		private static readonly StringComparer _invariantCultureIgnoreCase = new CultureAwareComparer(CultureInfo.InvariantCulture, true);

		private static readonly StringComparer _ordinal = new OrdinalComparer(false);

		private static readonly StringComparer _ordinalIgnoreCase = new OrdinalComparer(true);
	}
}
