using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Exchange.Search.Core.Abstraction;
using Microsoft.Exchange.Search.Mdb;

namespace Microsoft.Exchange.Search.Engine
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(404224661U, "InterlockedCounterDisposed");
			Strings.stringIDs.Add(3118336813U, "InterlockedCounterTimeout");
		}

		public static LocalizedString FeedingSkipped(MdbInfo mdbInfo, ContentIndexStatusType state, IndexStatusErrorCode indexStatusErrorCode)
		{
			return new LocalizedString("FeedingSkipped", Strings.ResourceManager, new object[]
			{
				mdbInfo,
				state,
				indexStatusErrorCode
			});
		}

		public static LocalizedString SearchFeedingControllerException(string databaseName, string exceptionDetails)
		{
			return new LocalizedString("SearchFeedingControllerException", Strings.ResourceManager, new object[]
			{
				databaseName,
				exceptionDetails
			});
		}

		public static LocalizedString ReseedOnPassiveServer(string database)
		{
			return new LocalizedString("ReseedOnPassiveServer", Strings.ResourceManager, new object[]
			{
				database
			});
		}

		public static LocalizedString MissingNotifications(string database)
		{
			return new LocalizedString("MissingNotifications", Strings.ResourceManager, new object[]
			{
				database
			});
		}

		public static LocalizedString InterlockedCounterDisposed
		{
			get
			{
				return new LocalizedString("InterlockedCounterDisposed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InterlockedCounterTimeout
		{
			get
			{
				return new LocalizedString("InterlockedCounterTimeout", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GracefulDegradationManagerException(string memoryUsageInfo, string catalogItemsInfo)
		{
			return new LocalizedString("GracefulDegradationManagerException", Strings.ResourceManager, new object[]
			{
				memoryUsageInfo,
				catalogItemsInfo
			});
		}

		public static LocalizedString FeedingSkippedWithFailureCode(MdbInfo mdbInfo, ContentIndexStatusType state, IndexStatusErrorCode indexStatusErrorCode, int? failureCode, string failureReason)
		{
			return new LocalizedString("FeedingSkippedWithFailureCode", Strings.ResourceManager, new object[]
			{
				mdbInfo,
				state,
				indexStatusErrorCode,
				failureCode,
				failureReason
			});
		}

		public static LocalizedString ReadLastEventFailure(string database, long lastEvent)
		{
			return new LocalizedString("ReadLastEventFailure", Strings.ResourceManager, new object[]
			{
				database,
				lastEvent
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(2);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Search.Engine.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			InterlockedCounterDisposed = 404224661U,
			InterlockedCounterTimeout = 3118336813U
		}

		private enum ParamIDs
		{
			FeedingSkipped,
			SearchFeedingControllerException,
			ReseedOnPassiveServer,
			MissingNotifications,
			GracefulDegradationManagerException,
			FeedingSkippedWithFailureCode,
			ReadLastEventFailure
		}
	}
}
