using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Common
{
	internal static class CommonStrings
	{
		static CommonStrings()
		{
			CommonStrings.stringIDs.Add(2988408489U, "InvalidTypeToCompare");
			CommonStrings.stringIDs.Add(1246015474U, "AsyncCopyGetException");
			CommonStrings.stringIDs.Add(1020926234U, "InvalidScheduleIntervalFormat");
		}

		public static LocalizedString InvalidTypeToCompare
		{
			get
			{
				return new LocalizedString("InvalidTypeToCompare", "", false, false, CommonStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExDbApiException(Win32Exception ex)
		{
			return new LocalizedString("ExDbApiException", "", false, false, CommonStrings.ResourceManager, new object[]
			{
				ex
			});
		}

		public static LocalizedString ExClusTransientException(string funName)
		{
			return new LocalizedString("ExClusTransientException", "", false, false, CommonStrings.ResourceManager, new object[]
			{
				funName
			});
		}

		public static LocalizedString AsyncCopyGetException
		{
			get
			{
				return new LocalizedString("AsyncCopyGetException", "", false, false, CommonStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotDetermineExchangeModeException(string reason)
		{
			return new LocalizedString("CannotDetermineExchangeModeException", "", false, false, CommonStrings.ResourceManager, new object[]
			{
				reason
			});
		}

		public static LocalizedString AsyncExceptionMessage(object message)
		{
			return new LocalizedString("AsyncExceptionMessage", "ExC35295", false, true, CommonStrings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString InvalidFailureItemException(string param)
		{
			return new LocalizedString("InvalidFailureItemException", "", false, false, CommonStrings.ResourceManager, new object[]
			{
				param
			});
		}

		public static LocalizedString InvalidScheduleIntervalFormat
		{
			get
			{
				return new LocalizedString("InvalidScheduleIntervalFormat", "", false, false, CommonStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(CommonStrings.IDs key)
		{
			return new LocalizedString(CommonStrings.stringIDs[(uint)key], CommonStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(3);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Common.Strings", typeof(CommonStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			InvalidTypeToCompare = 2988408489U,
			AsyncCopyGetException = 1246015474U,
			InvalidScheduleIntervalFormat = 1020926234U
		}

		private enum ParamIDs
		{
			ExDbApiException,
			ExClusTransientException,
			CannotDetermineExchangeModeException,
			AsyncExceptionMessage,
			InvalidFailureItemException
		}
	}
}
