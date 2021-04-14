using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.ThirdPartyReplication
{
	internal static class ThirdPartyReplication
	{
		static ThirdPartyReplication()
		{
			ThirdPartyReplication.stringIDs.Add(3969839167U, "NoPAMDesignated");
			ThirdPartyReplication.stringIDs.Add(3499394288U, "NotAuthorizedError");
		}

		public static LocalizedString TPRBaseError(string error)
		{
			return new LocalizedString("TPRBaseError", "Ex382280", false, true, ThirdPartyReplication.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString NoCopyOnServer(Guid dbId, string dbName, string serverName)
		{
			return new LocalizedString("NoCopyOnServer", "ExCDCA66", false, true, ThirdPartyReplication.ResourceManager, new object[]
			{
				dbId,
				dbName,
				serverName
			});
		}

		public static LocalizedString FailedCommunication(string reason)
		{
			return new LocalizedString("FailedCommunication", "Ex90A3E6", false, true, ThirdPartyReplication.ResourceManager, new object[]
			{
				reason
			});
		}

		public static LocalizedString ImmediateDismountMailboxDatabaseFailed(Guid dbId, string reason)
		{
			return new LocalizedString("ImmediateDismountMailboxDatabaseFailed", "Ex7FF234", false, true, ThirdPartyReplication.ResourceManager, new object[]
			{
				dbId,
				reason
			});
		}

		public static LocalizedString NoPAMDesignated
		{
			get
			{
				return new LocalizedString("NoPAMDesignated", "ExE29DBC", false, true, ThirdPartyReplication.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OnlyPAMError(string apiName)
		{
			return new LocalizedString("OnlyPAMError", "Ex754C02", false, true, ThirdPartyReplication.ResourceManager, new object[]
			{
				apiName
			});
		}

		public static LocalizedString NoSuchDatabase(Guid dbId)
		{
			return new LocalizedString("NoSuchDatabase", "ExC6D439", false, true, ThirdPartyReplication.ResourceManager, new object[]
			{
				dbId
			});
		}

		public static LocalizedString GetPamError(string reason)
		{
			return new LocalizedString("GetPamError", "Ex533813", false, true, ThirdPartyReplication.ResourceManager, new object[]
			{
				reason
			});
		}

		public static LocalizedString ChangeActiveServerFailed(Guid dbId, string newServer, string reason)
		{
			return new LocalizedString("ChangeActiveServerFailed", "Ex21120F", false, true, ThirdPartyReplication.ResourceManager, new object[]
			{
				dbId,
				newServer,
				reason
			});
		}

		public static LocalizedString NotAuthorizedError
		{
			get
			{
				return new LocalizedString("NotAuthorizedError", "Ex7EFE98", false, true, ThirdPartyReplication.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(ThirdPartyReplication.IDs key)
		{
			return new LocalizedString(ThirdPartyReplication.stringIDs[(uint)key], ThirdPartyReplication.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(2);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.ThirdPartyReplication.Strings", typeof(ThirdPartyReplication).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			NoPAMDesignated = 3969839167U,
			NotAuthorizedError = 3499394288U
		}

		private enum ParamIDs
		{
			TPRBaseError,
			NoCopyOnServer,
			FailedCommunication,
			ImmediateDismountMailboxDatabaseFailed,
			OnlyPAMError,
			NoSuchDatabase,
			GetPamError,
			ChangeActiveServerFailed
		}
	}
}
