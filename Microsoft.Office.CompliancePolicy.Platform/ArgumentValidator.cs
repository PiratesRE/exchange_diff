using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Office.CompliancePolicy
{
	internal static class ArgumentValidator
	{
		public static void ThrowIfNull(string name, object arg)
		{
			if (arg == null)
			{
				throw new ArgumentNullException(name);
			}
		}

		public static void ThrowIfNullOrEmpty(string name, string arg)
		{
			if (string.IsNullOrEmpty(arg))
			{
				throw new ArgumentException("The value is set to null or empty", name);
			}
		}

		public static void ThrowIfNullOrWhiteSpace(string name, string arg)
		{
			if (string.IsNullOrWhiteSpace(arg))
			{
				throw new ArgumentException("The value is set to null or empty or white space", name);
			}
		}

		public static void ThrowIfCollectionNullOrEmpty<T>(string name, IEnumerable<T> arg)
		{
			if (arg == null || !arg.Any<T>())
			{
				throw new ArgumentException("The collection is set to null or empty", name);
			}
		}

		public static void ThrowIfZero(string name, uint arg)
		{
			if (arg == 0U)
			{
				throw new ArgumentException("The number is set to 0", name);
			}
		}

		public static void ThrowIfZeroOrNegative(string name, int arg)
		{
			if (0 >= arg)
			{
				throw new ArgumentException("The number is set to 0 or negative", name);
			}
		}

		public static void ThrowIfNegative(string name, int arg)
		{
			if (0 > arg)
			{
				throw new ArgumentException("The number is set to negative", name);
			}
		}

		public static void ThrowIfNegativeTimeSpan(string name, TimeSpan arg)
		{
			if (arg < TimeSpan.Zero)
			{
				throw new ArgumentException("The time span is set to negative", name);
			}
		}

		public static void ThrowIfWrongType(string name, object arg, Type expectedType)
		{
			if (arg.GetType() != expectedType)
			{
				throw new ArgumentException("The argument type is not correct", name);
			}
		}

		public static void ThrowIfOutOfRange<T>(string name, T arg, T min, T max) where T : struct, IComparable<T>
		{
			if (arg.CompareTo(min) < 0 || arg.CompareTo(max) > 0)
			{
				throw new ArgumentOutOfRangeException(name, arg, string.Format("The value is out of the valid range {0}:{1}", min, max));
			}
		}
	}
}
