using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.DDIService
{
	public class ServerPickerService : DDICodeBehind
	{
		public static void GetReceiveConnectorServerListPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			dataTable.BeginLoadData();
			List<DataRow> list = new List<DataRow>();
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				ServerRole serverRole = (ServerRole)dataRow["ServerRole"];
				ServerVersion serverVersion = (ServerVersion)dataRow["AdminDisplayVersion"];
				if (serverVersion.Major == 15)
				{
					if ((serverRole & ServerRole.ClientAccess) != ServerRole.ClientAccess && (serverRole & ServerRole.Mailbox) != ServerRole.Mailbox)
					{
						list.Add(dataRow);
					}
					else if ((serverRole & ServerRole.ClientAccess) == ServerRole.ClientAccess && (serverRole & ServerRole.Mailbox) == ServerRole.Mailbox)
					{
						dataRow["ServerData"] = string.Format("{0},{1}", (string)dataRow["Fqdn"], ServerPickerService.ServerRoleType.Mixed.ToString());
					}
					else if ((serverRole & ServerRole.ClientAccess) == ServerRole.ClientAccess)
					{
						dataRow["ServerData"] = string.Format("{0},{1}", (string)dataRow["Fqdn"], ServerPickerService.ServerRoleType.ClientAccess.ToString());
					}
					else if ((serverRole & ServerRole.Mailbox) == ServerRole.Mailbox)
					{
						dataRow["ServerData"] = string.Format("{0},{1}", (string)dataRow["Fqdn"], ServerPickerService.ServerRoleType.Mailbox.ToString());
					}
				}
				else if ((serverRole & ServerRole.HubTransport) != ServerRole.HubTransport)
				{
					list.Add(dataRow);
				}
				else
				{
					dataRow["ServerData"] = string.Format("{0},{1}", (string)dataRow["Fqdn"], ServerPickerService.ServerRoleType.HubTransport.ToString());
				}
			}
			foreach (DataRow row in list)
			{
				dataTable.Rows.Remove(row);
			}
			dataTable.EndLoadData();
		}

		public static void GetSendConnectorSouceServerListPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			dataTable.BeginLoadData();
			List<DataRow> list = new List<DataRow>();
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				bool flag = (bool)dataRow["IsEdgeServer"];
				bool flag2 = (bool)dataRow["IsHubTransportServer"];
				if (!flag && !flag2)
				{
					list.Add(dataRow);
				}
			}
			foreach (DataRow row in list)
			{
				dataTable.Rows.Remove(row);
			}
			dataTable.EndLoadData();
		}

		public static string ExtractServerId(string serverData)
		{
			return serverData.Split(new char[]
			{
				','
			})[0];
		}

		public enum ServerRoleType
		{
			Mixed = 1,
			ClientAccess,
			Mailbox,
			HubTransport
		}
	}
}
