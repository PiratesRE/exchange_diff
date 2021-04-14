using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public class DAGCodeBehind
	{
		public static void GetDAGPostAction(DataRow row, DataTable dataTable, DataObjectStore store)
		{
			IEnumerable<ADObjectId> enumerable = (IEnumerable<ADObjectId>)store.GetValue("DatabaseAvailabilityGroup", "Servers");
			List<ServerResolverRow> list = (enumerable != null) ? ServerResolver.Instance.ResolveObjects(enumerable).ToList<ServerResolverRow>() : null;
			IEnumerable<ADObjectId> enumerable2 = (IEnumerable<ADObjectId>)store.GetValue("DatabaseAvailabilityGroup", "OperationalServers");
			if (list != null && enumerable2 != null)
			{
				foreach (ADObjectId identity in enumerable2)
				{
					Identity operationalServerIdentity = identity.ToIdentity();
					list.ForEach(delegate(ServerResolverRow resolvedServer)
					{
						if (resolvedServer.Identity == operationalServerIdentity)
						{
							resolvedServer.OperationalState = Strings.Yes;
						}
					});
				}
			}
			dataTable.Rows[0]["ServersWithOperationalState"] = list;
			dataTable.Rows[0]["WitnessServer"] = store.GetValue("DatabaseAvailabilityGroup", "WitnessServer");
			dataTable.Rows[0]["WitnessDirectory"] = store.GetValue("DatabaseAvailabilityGroup", "WitnessDirectory");
		}

		public static void GetDAGListPostAction(DataRow row, DataTable dataTable, DataObjectStore store)
		{
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				dataRow["WitnessServer"] = dataRow["WitnessServerValue"];
				dataRow["WitnessDirectory"] = dataRow["WitnessDirectoryValue"];
			}
		}
	}
}
