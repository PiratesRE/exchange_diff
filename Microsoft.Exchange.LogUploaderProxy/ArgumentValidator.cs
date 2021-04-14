using System;
using System.Collections.Generic;
using Microsoft.Office.CompliancePolicy;

namespace Microsoft.Exchange.LogUploaderProxy
{
	public static class ArgumentValidator
	{
		public static void ThrowIfCollectionNullOrEmpty<T>(string name, IEnumerable<T> arg)
		{
			ArgumentValidator.ThrowIfCollectionNullOrEmpty<T>(name, arg);
		}

		public static void ThrowIfNegativeTimeSpan(string name, TimeSpan arg)
		{
			ArgumentValidator.ThrowIfNegativeTimeSpan(name, arg);
		}

		public static void ThrowIfNull(string name, object arg)
		{
			ArgumentValidator.ThrowIfNull(name, arg);
		}

		public static void ThrowIfNullOrEmpty(string name, string arg)
		{
			ArgumentValidator.ThrowIfNullOrEmpty(name, arg);
		}

		public static void ThrowIfWrongType(string name, object arg, Type expectedType)
		{
			ArgumentValidator.ThrowIfWrongType(name, arg, expectedType);
		}

		public static void ThrowIfZero(string name, uint arg)
		{
			ArgumentValidator.ThrowIfZero(name, arg);
		}

		public static void ThrowIfZeroOrNegative(string name, int arg)
		{
			ArgumentValidator.ThrowIfZeroOrNegative(name, arg);
		}
	}
}
