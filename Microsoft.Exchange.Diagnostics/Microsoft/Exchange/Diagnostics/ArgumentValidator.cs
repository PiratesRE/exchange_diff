using System;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Exchange.Diagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ArgumentValidator
	{
		public static void ThrowIfNull(string name, object arg)
		{
			if (arg == null)
			{
				throw new ArgumentNullException(name);
			}
		}

		public static void ThrowIfEmpty(string name, string arg)
		{
			if (arg.Equals(string.Empty))
			{
				throw new ArgumentException("The value is set to empty", name);
			}
		}

		public static void ThrowIfNullOrEmpty(string name, string arg)
		{
			if (string.IsNullOrEmpty(arg))
			{
				throw new ArgumentException("The value is set to null or empty", name);
			}
		}

		public static void ThrowIfNullOrEmpty(string name, object[] arg)
		{
			if (arg == null)
			{
				throw new ArgumentNullException(name);
			}
			if (arg.Length == 0)
			{
				throw new ArgumentException("The array value is set to empty", name);
			}
		}

		public static void ThrowIfNullOrWhiteSpace(string name, string arg)
		{
			if (string.IsNullOrWhiteSpace(arg))
			{
				throw new ArgumentException("The value is set to null or white space", name);
			}
		}

		public static void ThrowIfEmpty(string name, Guid arg)
		{
			if (arg.Equals(Guid.Empty))
			{
				throw new ArgumentException("The value is set to empty", name);
			}
		}

		public static void ThrowIfNegative(string name, int arg)
		{
			if (arg < 0)
			{
				throw new ArgumentOutOfRangeException(name, arg, "The value is negative");
			}
		}

		public static void ThrowIfZeroOrNegative(string name, int arg)
		{
			if (arg <= 0)
			{
				throw new ArgumentOutOfRangeException(name, arg, "The value is zero or negative");
			}
		}

		public static void ThrowIfOutOfRange<T>(string name, T arg, T min, T max) where T : struct, IComparable<T>
		{
			if (arg.CompareTo(min) < 0 || arg.CompareTo(max) > 0)
			{
				throw new ArgumentOutOfRangeException(name, arg, string.Format("The value is out of the valid range {0}:{1}", min, max));
			}
		}

		public static void ThrowIfTypeInvalid<T>(string name, object arg)
		{
			if (arg == null || (arg.GetType() != typeof(T) && !arg.GetType().GetTypeInfo().IsSubclassOf(typeof(T))))
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid type for arg:{0}, Expected:{1}, Actual:{2}", new object[]
				{
					name,
					typeof(T).Name,
					arg.GetType().Name
				}));
			}
		}

		public static void ThrowIfTypeInvalid(string name, object arg, Type expectedType)
		{
			if (arg == null || (arg.GetType() != expectedType && !arg.GetType().GetTypeInfo().IsSubclassOf(expectedType)))
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid type for arg:{0}, Expected:{1}, Actual:{2}", new object[]
				{
					name,
					expectedType.Name,
					arg.GetType().Name
				}));
			}
		}

		public static void ThrowIfInvalidValue<T>(string name, T arg, Predicate<T> validator)
		{
			if (!validator(arg))
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid value for arg:{0}, value:{1}", new object[]
				{
					name,
					arg
				}));
			}
		}
	}
}
