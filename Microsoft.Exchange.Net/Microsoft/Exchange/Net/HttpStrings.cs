using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net
{
	internal static class HttpStrings
	{
		static HttpStrings()
		{
			HttpStrings.stringIDs.Add(1040461213U, "DownloadPermanentException");
			HttpStrings.stringIDs.Add(155224738U, "DownloadTimeoutException");
			HttpStrings.stringIDs.Add(3994627926U, "DownloadCanceledException");
			HttpStrings.stringIDs.Add(1904454819U, "DownloadTransientException");
		}

		public static LocalizedString ServerProtocolViolationException(string size)
		{
			return new LocalizedString("ServerProtocolViolationException", "", false, false, HttpStrings.ResourceManager, new object[]
			{
				size
			});
		}

		public static LocalizedString UnsupportedUriFormatException(string uri)
		{
			return new LocalizedString("UnsupportedUriFormatException", "", false, false, HttpStrings.ResourceManager, new object[]
			{
				uri
			});
		}

		public static LocalizedString BadRedirectedUriException(string uri)
		{
			return new LocalizedString("BadRedirectedUriException", "", false, false, HttpStrings.ResourceManager, new object[]
			{
				uri
			});
		}

		public static LocalizedString DownloadPermanentException
		{
			get
			{
				return new LocalizedString("DownloadPermanentException", "", false, false, HttpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DownloadLimitExceededException(string size)
		{
			return new LocalizedString("DownloadLimitExceededException", "", false, false, HttpStrings.ResourceManager, new object[]
			{
				size
			});
		}

		public static LocalizedString DownloadTimeoutException
		{
			get
			{
				return new LocalizedString("DownloadTimeoutException", "", false, false, HttpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DownloadCanceledException
		{
			get
			{
				return new LocalizedString("DownloadCanceledException", "", false, false, HttpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DownloadTransientException
		{
			get
			{
				return new LocalizedString("DownloadTransientException", "", false, false, HttpStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(HttpStrings.IDs key)
		{
			return new LocalizedString(HttpStrings.stringIDs[(uint)key], HttpStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(4);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Net.HttpStrings", typeof(HttpStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			DownloadPermanentException = 1040461213U,
			DownloadTimeoutException = 155224738U,
			DownloadCanceledException = 3994627926U,
			DownloadTransientException = 1904454819U
		}

		private enum ParamIDs
		{
			ServerProtocolViolationException,
			UnsupportedUriFormatException,
			BadRedirectedUriException,
			DownloadLimitExceededException
		}
	}
}
