using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Search.Mdb
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(330076707U, "DocumentProcessingFailed");
			Strings.stringIDs.Add(3296324356U, "FailedToOpenAdminRpcConnection");
			Strings.stringIDs.Add(1985214299U, "FailedToRegisterDatabaseChangeNotification");
			Strings.stringIDs.Add(2036468886U, "FailedToGetLocalServer");
			Strings.stringIDs.Add(151923492U, "FailedToGetMailboxDatabases");
			Strings.stringIDs.Add(3211562794U, "FailedToGetDatabasesContainerId");
			Strings.stringIDs.Add(3857521097U, "FailedToShutdownFeeder");
			Strings.stringIDs.Add(2230516668U, "InvalidDocument");
			Strings.stringIDs.Add(2968057743U, "FailedToCrawlMailbox");
			Strings.stringIDs.Add(3667120730U, "FailedToUnRegisterDatabaseChangeNotification");
			Strings.stringIDs.Add(3549756491U, "RetryFailed");
			Strings.stringIDs.Add(3018223464U, "ErrorAccessingStateStorage");
			Strings.stringIDs.Add(1020230779U, "AdOperationFailed");
		}

		public static LocalizedString DocumentProcessingFailed
		{
			get
			{
				return new LocalizedString("DocumentProcessingFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToGetActiveServer(Guid mdbGuid)
		{
			return new LocalizedString("FailedToGetActiveServer", Strings.ResourceManager, new object[]
			{
				mdbGuid
			});
		}

		public static LocalizedString FailedToOpenAdminRpcConnection
		{
			get
			{
				return new LocalizedString("FailedToOpenAdminRpcConnection", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxLoginFailed(StoreSessionCacheKey key)
		{
			return new LocalizedString("MailboxLoginFailed", Strings.ResourceManager, new object[]
			{
				key
			});
		}

		public static LocalizedString MdbMailboxQueryFailed(Guid mdbGuid)
		{
			return new LocalizedString("MdbMailboxQueryFailed", Strings.ResourceManager, new object[]
			{
				mdbGuid
			});
		}

		public static LocalizedString FailedToReadNotifications(Guid mdbGuid)
		{
			return new LocalizedString("FailedToReadNotifications", Strings.ResourceManager, new object[]
			{
				mdbGuid
			});
		}

		public static LocalizedString FailedToRegisterDatabaseChangeNotification
		{
			get
			{
				return new LocalizedString("FailedToRegisterDatabaseChangeNotification", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToGetLocalServer
		{
			get
			{
				return new LocalizedString("FailedToGetLocalServer", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxQuarantined(StoreSessionCacheKey key)
		{
			return new LocalizedString("MailboxQuarantined", Strings.ResourceManager, new object[]
			{
				key
			});
		}

		public static LocalizedString FailedToGetServer(string fqdn)
		{
			return new LocalizedString("FailedToGetServer", Strings.ResourceManager, new object[]
			{
				fqdn
			});
		}

		public static LocalizedString FailedToGetMailboxDatabases
		{
			get
			{
				return new LocalizedString("FailedToGetMailboxDatabases", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToGetDatabasesContainerId
		{
			get
			{
				return new LocalizedString("FailedToGetDatabasesContainerId", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectionToMailboxFailed(Guid mbxGuid)
		{
			return new LocalizedString("ConnectionToMailboxFailed", Strings.ResourceManager, new object[]
			{
				mbxGuid
			});
		}

		public static LocalizedString ServerNotFound(string fqdn)
		{
			return new LocalizedString("ServerNotFound", Strings.ResourceManager, new object[]
			{
				fqdn
			});
		}

		public static LocalizedString FailedToShutdownFeeder
		{
			get
			{
				return new LocalizedString("FailedToShutdownFeeder", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDocument
		{
			get
			{
				return new LocalizedString("InvalidDocument", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToCrawlMailbox
		{
			get
			{
				return new LocalizedString("FailedToCrawlMailbox", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToUnRegisterDatabaseChangeNotification
		{
			get
			{
				return new LocalizedString("FailedToUnRegisterDatabaseChangeNotification", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedCreateEventManager(Guid mdbGuid)
		{
			return new LocalizedString("FailedCreateEventManager", Strings.ResourceManager, new object[]
			{
				mdbGuid
			});
		}

		public static LocalizedString RetryFailed
		{
			get
			{
				return new LocalizedString("RetryFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionOccurred(Guid mdbGuid)
		{
			return new LocalizedString("ExceptionOccurred", Strings.ResourceManager, new object[]
			{
				mdbGuid
			});
		}

		public static LocalizedString ErrorAccessingStateStorage
		{
			get
			{
				return new LocalizedString("ErrorAccessingStateStorage", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AdOperationFailed
		{
			get
			{
				return new LocalizedString("AdOperationFailed", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToFindSystemMailbox(Guid mdbGuid)
		{
			return new LocalizedString("FailedToFindSystemMailbox", Strings.ResourceManager, new object[]
			{
				mdbGuid
			});
		}

		public static LocalizedString MailboxLocked(StoreSessionCacheKey key)
		{
			return new LocalizedString("MailboxLocked", Strings.ResourceManager, new object[]
			{
				key
			});
		}

		public static LocalizedString UnavailableSession(StoreSessionCacheKey key)
		{
			return new LocalizedString("UnavailableSession", Strings.ResourceManager, new object[]
			{
				key
			});
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(13);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Search.Mdb.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			DocumentProcessingFailed = 330076707U,
			FailedToOpenAdminRpcConnection = 3296324356U,
			FailedToRegisterDatabaseChangeNotification = 1985214299U,
			FailedToGetLocalServer = 2036468886U,
			FailedToGetMailboxDatabases = 151923492U,
			FailedToGetDatabasesContainerId = 3211562794U,
			FailedToShutdownFeeder = 3857521097U,
			InvalidDocument = 2230516668U,
			FailedToCrawlMailbox = 2968057743U,
			FailedToUnRegisterDatabaseChangeNotification = 3667120730U,
			RetryFailed = 3549756491U,
			ErrorAccessingStateStorage = 3018223464U,
			AdOperationFailed = 1020230779U
		}

		private enum ParamIDs
		{
			FailedToGetActiveServer,
			MailboxLoginFailed,
			MdbMailboxQueryFailed,
			FailedToReadNotifications,
			MailboxQuarantined,
			FailedToGetServer,
			ConnectionToMailboxFailed,
			ServerNotFound,
			FailedCreateEventManager,
			ExceptionOccurred,
			FailedToFindSystemMailbox,
			MailboxLocked,
			UnavailableSession
		}
	}
}
