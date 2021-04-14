using System;
using System.Data;
using System.Text;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SystemManager;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SnapIn.Esm
{
	internal class ServerDataColumnsCalculator : IDataColumnsCalculator
	{
		public void Calculate(ResultsLoaderProfile profile, DataTable dataTable, DataRow dataRow)
		{
			StringBuilder stringBuilder = new StringBuilder();
			ServerRole serverRole = ServerRole.None;
			if ((bool)dataRow["IsProvisionedServer"])
			{
				stringBuilder.AppendFormat("{0}, ", Strings.Provisioned);
				serverRole = ServerRole.ProvisionedServer;
			}
			else
			{
				if ((bool)dataRow["IsHubTransportServer"])
				{
					stringBuilder.AppendFormat("{0}, ", Strings.Bridgehead);
					serverRole |= ServerRole.HubTransport;
				}
				if ((bool)dataRow["IsClientAccessServer"])
				{
					stringBuilder.AppendFormat("{0}, ", Strings.ClientAccess);
					serverRole |= ServerRole.ClientAccess;
				}
				if ((bool)dataRow["IsMailboxServer"])
				{
					stringBuilder.AppendFormat("{0}, ", Strings.Mailbox);
					serverRole |= ServerRole.Mailbox;
				}
				if ((bool)dataRow["IsUnifiedMessagingServer"])
				{
					stringBuilder.AppendFormat("{0}, ", Strings.UnifiedMessaging);
					serverRole |= ServerRole.UnifiedMessaging;
				}
				if ((bool)dataRow["IsEdgeServer"])
				{
					stringBuilder.AppendFormat("{0}, ", Strings.Gateway);
					serverRole |= ServerRole.Edge;
				}
				if (stringBuilder.Length == 0)
				{
					stringBuilder.Append(Strings.Unknown);
				}
			}
			dataRow["DisplayServerRole"] = stringBuilder.ToString().Trim(new char[]
			{
				' ',
				','
			});
			dataRow["CurrentServerRole"] = serverRole.ToString();
		}
	}
}
