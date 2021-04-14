using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.HA.DirectoryServices;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal static class DirectoryHelper
	{
		public static IADDatabaseAvailabilityGroup GetLocalServerDatabaseAvailabilityGroup(IADToplogyConfigurationSession adSession, out Exception exception)
		{
			IADDatabaseAvailabilityGroup dag = null;
			Exception objNotFoundEx = null;
			exception = null;
			exception = SharedHelper.RunADOperationEx(delegate(object param0, EventArgs param1)
			{
				if (adSession == null)
				{
					adSession = ADSessionFactory.CreateIgnoreInvalidRootOrgSession(true);
				}
				if (adSession != null)
				{
					IADServer iadserver = adSession.FindServerByName(SharedDependencies.ManagementClassHelper.LocalMachineName);
					if (iadserver != null)
					{
						ADObjectId databaseAvailabilityGroup = iadserver.DatabaseAvailabilityGroup;
						if (databaseAvailabilityGroup != null)
						{
							dag = adSession.ReadADObject<IADDatabaseAvailabilityGroup>(databaseAvailabilityGroup);
							return;
						}
					}
					else
					{
						objNotFoundEx = new CouldNotFindServerObject(Environment.MachineName);
					}
				}
			});
			if (objNotFoundEx != null)
			{
				exception = objNotFoundEx;
			}
			return dag;
		}
	}
}
