using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal static class MigMonHealthMonitor
	{
		public static void PublishServerHealthStatus()
		{
			List<MigMonHealthMonitor.ServerInfo> list = MigMonHealthMonitor.FindMailboxServers();
			if (list == null || !list.Any<MigMonHealthMonitor.ServerInfo>())
			{
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, "No mailbox servers found in the site. Site is probably decomissioned.", new object[0]);
				return;
			}
			MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Information, "Uploading server health info.", new object[0]);
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("ServerFQDN", typeof(string));
			dataTable.Columns.Add("AdminDisplayVersion", typeof(string));
			dataTable.Columns.Add("IsOnline", typeof(bool));
			foreach (MigMonHealthMonitor.ServerInfo serverInfo in list)
			{
				dataTable.Rows.Add(new object[]
				{
					serverInfo.ServerFQDN,
					serverInfo.AdminDisplayVersion,
					serverInfo.IsOnline
				});
			}
			List<SqlParameter> list2 = new List<SqlParameter>();
			list2.Add(new SqlParameter("ServerList", dataTable)
			{
				SqlDbType = SqlDbType.Structured,
				TypeName = "dbo.ServerHealthStatus"
			});
			try
			{
				MigrationMonitor.SqlHelper.ExecuteSprocNonQuery("MIGMON_UpdateServerHealthStatus", list2, 30);
			}
			catch (SqlQueryFailedException ex)
			{
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, ex, "Error updating server health info to the database. Will attempt again next cycle.", new object[0]);
				throw new HealthStatusPublishFailureException(ex.InnerException);
			}
		}

		private static List<MigMonHealthMonitor.ServerInfo> FindMailboxServers()
		{
			List<MigMonHealthMonitor.ServerInfo> list = new List<MigMonHealthMonitor.ServerInfo>();
			MiniServer[] array;
			try
			{
				AnchorADProvider rootOrgProvider = AnchorADProvider.GetRootOrgProvider(MigrationMonitor.MigrationMonitorContext);
				array = CommonUtils.FindServers(rootOrgProvider.ConfigurationSession, Server.E15MinVersion, ServerRole.Mailbox, CommonUtils.LocalSiteId, new ADPropertyDefinition[]
				{
					ServerSchema.ComponentStates
				});
			}
			catch (LocalizedException ex)
			{
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Error, "Exception encountered looking for servers. Will try again next cycle. Error: {0}", new object[]
				{
					ex
				});
				throw new HealthStatusPublishFailureException(ex);
			}
			if (array == null)
			{
				MigrationMonitor.MigrationMonitorContext.Logger.Log(MigrationEventType.Information, "Found no e15 mbx servers on the local site", new object[0]);
				return list;
			}
			foreach (MiniServer miniServer in array)
			{
				list.Add(new MigMonHealthMonitor.ServerInfo
				{
					ServerFQDN = miniServer.Fqdn,
					AdminDisplayVersion = miniServer.AdminDisplayVersion.ToString(),
					IsOnline = ServerComponentStates.IsServerOnline((MultiValuedProperty<string>)miniServer[ServerSchema.ComponentStates])
				});
			}
			return list;
		}

		private const string SProcNameHealthUpdate = "MIGMON_UpdateServerHealthStatus";

		private struct ServerInfo
		{
			public string ServerFQDN { get; set; }

			public string AdminDisplayVersion { get; set; }

			public bool IsOnline { get; set; }
		}
	}
}
