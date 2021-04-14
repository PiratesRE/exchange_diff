using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class AdObjectLookupHelper
	{
		public static IADServer FindLocalServer(IFindAdObject<IADServer> serverLookup)
		{
			string localComputerFqdn = NativeHelpers.GetLocalComputerFqdn(true);
			return serverLookup.FindServerByFqdn(localComputerFqdn);
		}

		public static IADDatabase[] GetAllDatabases(IFindAdObject<IADDatabase> databaseLookup, IADServer server)
		{
			List<IADDatabase> list = new List<IADDatabase>(20);
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, server.Name);
			IADDatabaseCopy[] array = databaseLookup.AdSession.Find<IADDatabaseCopy>(null, QueryScope.SubTree, filter, null, 0);
			foreach (IADDatabaseCopy iaddatabaseCopy in array)
			{
				ADObjectId parent = iaddatabaseCopy.Id.Parent;
				IADDatabase iaddatabase = databaseLookup.ReadAdObjectByObjectId(parent);
				if (iaddatabase != null)
				{
					list.Add(iaddatabase);
				}
			}
			return (list.Count<IADDatabase>() > 0) ? list.ToArray() : null;
		}

		public static IADDatabase[] GetAllDatabases(IADToplogyConfigurationSession adSession, IFindAdObject<IADDatabase> databaseLookup, MiniServer miniServer, out Exception exception)
		{
			IADDatabaseCopy[] copies = null;
			QueryFilter serverFilter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, miniServer.Name);
			exception = ADUtils.RunADOperation(delegate()
			{
				copies = adSession.Find<IADDatabaseCopy>(null, QueryScope.SubTree, serverFilter, null, 0);
			}, 2);
			if (exception != null)
			{
				return null;
			}
			List<IADDatabase> list = new List<IADDatabase>(20);
			foreach (IADDatabaseCopy iaddatabaseCopy in copies)
			{
				ADObjectId parent = iaddatabaseCopy.Id.Parent;
				IADDatabase iaddatabase = databaseLookup.ReadAdObjectByObjectIdEx(parent, out exception);
				if (exception != null)
				{
					return null;
				}
				if (iaddatabase != null)
				{
					list.Add(iaddatabase);
				}
			}
			return (list.Count > 0) ? list.ToArray() : null;
		}
	}
}
