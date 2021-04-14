using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Search.Fast
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(135840484U, "ConnectionFailure");
			Strings.stringIDs.Add(1417813431U, "PerformingFastOperationException");
			Strings.stringIDs.Add(3134189720U, "FailureToDetectFastInstallation");
			Strings.stringIDs.Add(4004591056U, "NoFASTNodesFound");
			Strings.stringIDs.Add(1948702154U, "UpdateConfigurationFailed");
		}

		public static LocalizedString ConnectionFailure
		{
			get
			{
				return new LocalizedString("ConnectionFailure", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToAcquireFolder(Guid mdbGuid, string folderName)
		{
			return new LocalizedString("FailedToAcquireFolder", Strings.ResourceManager, new object[]
			{
				mdbGuid,
				folderName
			});
		}

		public static LocalizedString FailedToConnectToSystemMailbox(Guid mdbGuid)
		{
			return new LocalizedString("FailedToConnectToSystemMailbox", Strings.ResourceManager, new object[]
			{
				mdbGuid
			});
		}

		public static LocalizedString PerformingFastOperationException
		{
			get
			{
				return new LocalizedString("PerformingFastOperationException", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToCreateNewItem(Guid mdbGuid, Guid mailboxGuid)
		{
			return new LocalizedString("FailedToCreateNewItem", Strings.ResourceManager, new object[]
			{
				mdbGuid,
				mailboxGuid
			});
		}

		public static LocalizedString FailureToDetectFastInstallation
		{
			get
			{
				return new LocalizedString("FailureToDetectFastInstallation", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToReadRetriableTableEntry(Guid mdbGuid, string itemId)
		{
			return new LocalizedString("FailedToReadRetriableTableEntry", Strings.ResourceManager, new object[]
			{
				mdbGuid,
				itemId
			});
		}

		public static LocalizedString FailedToQueryPermanentFailures(Guid mdbGuid)
		{
			return new LocalizedString("FailedToQueryPermanentFailures", Strings.ResourceManager, new object[]
			{
				mdbGuid
			});
		}

		public static LocalizedString NoFASTNodesFound
		{
			get
			{
				return new LocalizedString("NoFASTNodesFound", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateConfigurationFailed
		{
			get
			{
				return new LocalizedString("UpdateConfigurationFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FastCannotProcessDocument(string msg)
		{
			return new LocalizedString("FastCannotProcessDocument", Strings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString FailedToQueryRetriableItems(Guid mdbGuid)
		{
			return new LocalizedString("FailedToQueryRetriableItems", Strings.ResourceManager, new object[]
			{
				mdbGuid
			});
		}

		public static LocalizedString LostCallbackFailure(string msg)
		{
			return new LocalizedString("LostCallbackFailure", Strings.ResourceManager, new object[]
			{
				msg
			});
		}

		public static LocalizedString DatabasePathDoesNotExist(string databasePath)
		{
			return new LocalizedString("DatabasePathDoesNotExist", Strings.ResourceManager, new object[]
			{
				databasePath
			});
		}

		public static LocalizedString FailedToQueryTable(Guid mdbGuid, string folderName)
		{
			return new LocalizedString("FailedToQueryTable", Strings.ResourceManager, new object[]
			{
				mdbGuid,
				folderName
			});
		}

		public static LocalizedString IndexSystemForFlowDoesNotExist(string flowName)
		{
			return new LocalizedString("IndexSystemForFlowDoesNotExist", Strings.ResourceManager, new object[]
			{
				flowName
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(5);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Search.Fast.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ConnectionFailure = 135840484U,
			PerformingFastOperationException = 1417813431U,
			FailureToDetectFastInstallation = 3134189720U,
			NoFASTNodesFound = 4004591056U,
			UpdateConfigurationFailed = 1948702154U
		}

		private enum ParamIDs
		{
			FailedToAcquireFolder,
			FailedToConnectToSystemMailbox,
			FailedToCreateNewItem,
			FailedToReadRetriableTableEntry,
			FailedToQueryPermanentFailures,
			FastCannotProcessDocument,
			FailedToQueryRetriableItems,
			LostCallbackFailure,
			DatabasePathDoesNotExist,
			FailedToQueryTable,
			IndexSystemForFlowDoesNotExist
		}
	}
}
