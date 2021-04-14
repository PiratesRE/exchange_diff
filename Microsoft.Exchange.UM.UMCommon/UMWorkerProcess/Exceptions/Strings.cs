using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.UM.UMWorkerProcess.Exceptions
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(3800718957U, "WPSemaphoreOpenFailure");
		}

		public static LocalizedString WPUnableToFindCertificate(string s)
		{
			return new LocalizedString("WPUnableToFindCertificate", Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString WPInvalidControlPort(int p, int min, int max)
		{
			return new LocalizedString("WPInvalidControlPort", Strings.ResourceManager, new object[]
			{
				p,
				min,
				max
			});
		}

		public static LocalizedString WPDirectoryNotFound(string s)
		{
			return new LocalizedString("WPDirectoryNotFound", Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString WPInvalidArguments(string arg)
		{
			return new LocalizedString("WPInvalidArguments", Strings.ResourceManager, new object[]
			{
				arg
			});
		}

		public static LocalizedString WPSemaphoreOpenFailure
		{
			get
			{
				return new LocalizedString("WPSemaphoreOpenFailure", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WPInvalidSipPort(int p, int min, int max)
		{
			return new LocalizedString("WPInvalidSipPort", Strings.ResourceManager, new object[]
			{
				p,
				min,
				max
			});
		}

		public static LocalizedString WPFileNotFound(string s)
		{
			return new LocalizedString("WPFileNotFound", Strings.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(1);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.UM.UMWorkerProcess.Exceptions.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			WPSemaphoreOpenFailure = 3800718957U
		}

		private enum ParamIDs
		{
			WPUnableToFindCertificate,
			WPInvalidControlPort,
			WPDirectoryNotFound,
			WPInvalidArguments,
			WPInvalidSipPort,
			WPFileNotFound
		}
	}
}
