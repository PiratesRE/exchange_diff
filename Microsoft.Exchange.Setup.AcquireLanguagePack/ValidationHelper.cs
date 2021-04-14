using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.Exchange.Setup.AcquireLanguagePack
{
	internal static class ValidationHelper
	{
		public static void ThrowIfDirectoryNotExist(string directoryName, string name)
		{
			ValidationHelper.ThrowIfNullOrEmpty(directoryName, name);
			if (!Directory.Exists(directoryName))
			{
				throw new ArgumentException(Strings.NotExist(directoryName));
			}
		}

		public static void ThrowIfFileNotExist(string fileName, string name)
		{
			ValidationHelper.ThrowIfNullOrEmpty(fileName, name);
			if (!File.Exists(fileName))
			{
				throw new ArgumentException(Strings.NotExist(fileName));
			}
		}

		public static void ThrowIfNullOrEmpty(string value, string name)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentNullException(Strings.IsNullOrEmpty(name ?? "Unknown"));
			}
		}

		public static void ThrowIfNullOrEmpty<T>(IEnumerable<T> value, string name)
		{
			if (value == null || !value.Any<T>())
			{
				throw new ArgumentNullException(Strings.IsNullOrEmpty(name ?? "Unknown"));
			}
		}

		public static void ThrowIfNull(object value, string name)
		{
			if (value == null)
			{
				throw new ArgumentNullException(Strings.IsNullOrEmpty(name ?? "Unknown"));
			}
		}
	}
}
