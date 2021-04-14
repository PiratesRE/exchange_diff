using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Assistants
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(172990383U, "descSkipException");
			Strings.stringIDs.Add(1233540867U, "MailboxPublicFolderFilter");
			Strings.stringIDs.Add(145250360U, "descNo");
			Strings.stringIDs.Add(825595121U, "MailboxInaccessibleFilter");
			Strings.stringIDs.Add(214179510U, "MailboxArchiveFilter");
			Strings.stringIDs.Add(1711506384U, "descTransientException");
			Strings.stringIDs.Add(645888561U, "MailboxMoveDestinationFilter");
			Strings.stringIDs.Add(2834798506U, "MailboxNotUserFilter");
			Strings.stringIDs.Add(384543172U, "descDeadMailboxException");
			Strings.stringIDs.Add(1232667862U, "descYes");
			Strings.stringIDs.Add(2233046866U, "MailboxNoGuidFilter");
			Strings.stringIDs.Add(1323233535U, "MailboxNotInDirectoryFilter");
			Strings.stringIDs.Add(2547632703U, "descDisconnectedMailboxException");
			Strings.stringIDs.Add(2813894235U, "MailboxInDemandJobFilter");
			Strings.stringIDs.Add(1914410581U, "descInvalidLanguageMailboxException");
			Strings.stringIDs.Add(3450913809U, "descLargeNumberSkippedMailboxes");
			Strings.stringIDs.Add(3870763552U, "descMailboxOrDatabaseNotSpecified");
			Strings.stringIDs.Add(3426940714U, "descAmbiguousAliasMailboxException");
			Strings.stringIDs.Add(1802393217U, "driverName");
		}

		public static LocalizedString descSkipException
		{
			get
			{
				return new LocalizedString("descSkipException", "Ex3DE87C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descUnknownAssistant(string assistantName)
		{
			return new LocalizedString("descUnknownAssistant", "Ex3696FC", false, true, Strings.ResourceManager, new object[]
			{
				assistantName
			});
		}

		public static LocalizedString MailboxPublicFolderFilter
		{
			get
			{
				return new LocalizedString("MailboxPublicFolderFilter", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descNo
		{
			get
			{
				return new LocalizedString("descNo", "ExA39D5F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxInaccessibleFilter
		{
			get
			{
				return new LocalizedString("MailboxInaccessibleFilter", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxArchiveFilter
		{
			get
			{
				return new LocalizedString("MailboxArchiveFilter", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descTransientException
		{
			get
			{
				return new LocalizedString("descTransientException", "Ex51BE76", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxMoveDestinationFilter
		{
			get
			{
				return new LocalizedString("MailboxMoveDestinationFilter", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxNotUserFilter
		{
			get
			{
				return new LocalizedString("MailboxNotUserFilter", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descDeadMailboxException
		{
			get
			{
				return new LocalizedString("descDeadMailboxException", "Ex2FC4BE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descYes
		{
			get
			{
				return new LocalizedString("descYes", "ExC8EBAE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxNoGuidFilter
		{
			get
			{
				return new LocalizedString("MailboxNoGuidFilter", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxNotInDirectoryFilter
		{
			get
			{
				return new LocalizedString("MailboxNotInDirectoryFilter", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descMissingSystemMailbox(string name)
		{
			return new LocalizedString("descMissingSystemMailbox", "Ex2C26A1", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString descDisconnectedMailboxException
		{
			get
			{
				return new LocalizedString("descDisconnectedMailboxException", "ExCF2CBB", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descUnknownDatabase(string databaseId)
		{
			return new LocalizedString("descUnknownDatabase", "Ex6EF341", false, true, Strings.ResourceManager, new object[]
			{
				databaseId
			});
		}

		public static LocalizedString MailboxInDemandJobFilter
		{
			get
			{
				return new LocalizedString("MailboxInDemandJobFilter", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descInvalidLanguageMailboxException
		{
			get
			{
				return new LocalizedString("descInvalidLanguageMailboxException", "Ex7F0128", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descLargeNumberSkippedMailboxes
		{
			get
			{
				return new LocalizedString("descLargeNumberSkippedMailboxes", "ExE7E5CA", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descMailboxOrDatabaseNotSpecified
		{
			get
			{
				return new LocalizedString("descMailboxOrDatabaseNotSpecified", "Ex00BAFD", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descAmbiguousAliasMailboxException
		{
			get
			{
				return new LocalizedString("descAmbiguousAliasMailboxException", "Ex8F97A8", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString driverName
		{
			get
			{
				return new LocalizedString("driverName", "Ex3B873A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(19);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Assistants.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			descSkipException = 172990383U,
			MailboxPublicFolderFilter = 1233540867U,
			descNo = 145250360U,
			MailboxInaccessibleFilter = 825595121U,
			MailboxArchiveFilter = 214179510U,
			descTransientException = 1711506384U,
			MailboxMoveDestinationFilter = 645888561U,
			MailboxNotUserFilter = 2834798506U,
			descDeadMailboxException = 384543172U,
			descYes = 1232667862U,
			MailboxNoGuidFilter = 2233046866U,
			MailboxNotInDirectoryFilter = 1323233535U,
			descDisconnectedMailboxException = 2547632703U,
			MailboxInDemandJobFilter = 2813894235U,
			descInvalidLanguageMailboxException = 1914410581U,
			descLargeNumberSkippedMailboxes = 3450913809U,
			descMailboxOrDatabaseNotSpecified = 3870763552U,
			descAmbiguousAliasMailboxException = 3426940714U,
			driverName = 1802393217U
		}

		private enum ParamIDs
		{
			descUnknownAssistant,
			descMissingSystemMailbox,
			descUnknownDatabase
		}
	}
}
