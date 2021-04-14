using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ADObjectWrapperFactory
	{
		public static IADObjectCommon CreateWrapper(ADObject adObject)
		{
			if (adObject == null)
			{
				return null;
			}
			if (adObject is Database)
			{
				return ADObjectWrapperFactory.CreateWrapper((Database)adObject);
			}
			if (adObject is MiniDatabase)
			{
				return ADObjectWrapperFactory.CreateWrapper((MiniDatabase)adObject);
			}
			if (adObject is DatabaseCopy)
			{
				return ADObjectWrapperFactory.CreateWrapper((DatabaseCopy)adObject);
			}
			if (adObject is ADComputer)
			{
				return ADObjectWrapperFactory.CreateWrapper((ADComputer)adObject);
			}
			if (adObject is Server)
			{
				return ADObjectWrapperFactory.CreateWrapper((Server)adObject);
			}
			if (adObject is MiniServer)
			{
				return ADObjectWrapperFactory.CreateWrapper((MiniServer)adObject);
			}
			if (adObject is DatabaseAvailabilityGroup)
			{
				return ADObjectWrapperFactory.CreateWrapper((DatabaseAvailabilityGroup)adObject);
			}
			if (adObject is ClientAccessArray)
			{
				return ADObjectWrapperFactory.CreateWrapper((ClientAccessArray)adObject);
			}
			if (adObject is MiniClientAccessServerOrArray)
			{
				return ADObjectWrapperFactory.CreateWrapper((MiniClientAccessServerOrArray)adObject);
			}
			if (adObject is ADSite)
			{
				return ADObjectWrapperFactory.CreateWrapper((ADSite)adObject);
			}
			ExAssert.RetailAssert(false, "Type '{0}' is not supported by CreateWrapper", new object[]
			{
				adObject.GetType()
			});
			return null;
		}

		public static ADDatabaseWrapper CreateWrapper(Database database)
		{
			return ADDatabaseWrapper.CreateWrapper(database);
		}

		public static ADDatabaseWrapper CreateWrapper(MiniDatabase database)
		{
			return ADDatabaseWrapper.CreateWrapper(database);
		}

		public static ADDatabaseCopyWrapper CreateWrapper(DatabaseCopy databaseCopy)
		{
			return ADDatabaseCopyWrapper.CreateWrapper(databaseCopy);
		}

		public static ADComputerWrapper CreateWrapper(ADComputer adComputer)
		{
			return ADComputerWrapper.CreateWrapper(adComputer);
		}

		public static ADServerWrapper CreateWrapper(Server server)
		{
			return ADServerWrapper.CreateWrapper(server);
		}

		public static ADServerWrapper CreateWrapper(MiniServer server)
		{
			return ADServerWrapper.CreateWrapper(server);
		}

		public static ADDatabaseAvailabilityGroupWrapper CreateWrapper(DatabaseAvailabilityGroup dag)
		{
			return ADDatabaseAvailabilityGroupWrapper.CreateWrapper(dag);
		}

		public static ADClientAccessArrayWrapper CreateWrapper(ClientAccessArray caArray)
		{
			return ADClientAccessArrayWrapper.CreateWrapper(caArray);
		}

		public static ADMiniClientAccessServerOrArrayWrapper CreateWrapper(MiniClientAccessServerOrArray caServerOrArray)
		{
			return ADMiniClientAccessServerOrArrayWrapper.CreateWrapper(caServerOrArray);
		}

		public static ADSiteWrapper CreateWrapper(ADSite adSite)
		{
			return ADSiteWrapper.CreateWrapper(adSite);
		}
	}
}
