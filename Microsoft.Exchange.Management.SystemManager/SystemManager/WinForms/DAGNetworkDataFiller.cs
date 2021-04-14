using System;
using System.Data;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class DAGNetworkDataFiller : MonadAdapterFiller
	{
		public DAGNetworkDataFiller(string commandText, ICommandBuilder builder) : base(commandText, builder)
		{
		}

		protected override void OnFill(DataTable table)
		{
			DataTable dataTable = table.Clone();
			base.OnFill(dataTable);
			DataTable dataTable2 = table.Clone();
			dataTable2.BeginLoadData();
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				string text = (string)dataRow["Identity"];
				DataRow dataRow2 = dataTable2.NewRow();
				dataRow2["Identity"] = text.Replace("\\", "\\\\");
				dataRow2["Name"] = dataRow["Name"];
				dataRow2["NetworkIdentity"] = text.Substring(text.IndexOf("\\") + 1);
				dataRow2["DatabaseAvailabilityGroup"] = text.Substring(0, text.IndexOf("\\"));
				dataRow2["OriginalIdentity"] = text;
				dataRow2["Image"] = "DAGNetwork";
				dataTable2.Rows.Add(dataRow2);
				string text2 = (string)dataRow2["Identity"];
				dataRow2 = dataTable2.NewRow();
				dataRow2["ParentColumn"] = text2;
				dataRow2["Identity"] = text2 + "\\ReplicationEnabled";
				dataRow2["Name"] = (((bool)dataRow["ReplicationEnabled"]) ? Strings.ReplicationEnabled : Strings.ReplicationDisabled);
				dataRow2["NetworkIdentity"] = text.Substring(text.IndexOf("\\") + 1);
				dataRow2["DatabaseAvailabilityGroup"] = text.Substring(0, text.IndexOf("\\"));
				dataRow2["OriginalIdentity"] = text;
				dataRow2["Image"] = (((bool)dataRow["ReplicationEnabled"]) ? "DAGReplicationEnabled" : "DAGReplicationDisabled");
				dataTable2.Rows.Add(dataRow2);
				dataRow2 = dataTable2.NewRow();
				dataRow2["ParentColumn"] = text2;
				dataRow2["Identity"] = text2 + "\\Subnets";
				dataRow2["Name"] = Strings.Subnets;
				dataRow2["NetworkIdentity"] = text.Substring(text.IndexOf("\\") + 1);
				dataRow2["DatabaseAvailabilityGroup"] = text.Substring(0, text.IndexOf("\\"));
				dataRow2["OriginalIdentity"] = text;
				dataRow2["Image"] = "DAGSubnets";
				dataTable2.Rows.Add(dataRow2);
				string value = (string)dataRow2["Identity"];
				DagNetMultiValuedProperty<DatabaseAvailabilityGroupNetworkSubnet> dagNetMultiValuedProperty = (DagNetMultiValuedProperty<DatabaseAvailabilityGroupNetworkSubnet>)dataRow["Subnets"];
				foreach (DatabaseAvailabilityGroupNetworkSubnet databaseAvailabilityGroupNetworkSubnet in dagNetMultiValuedProperty)
				{
					dataRow2 = dataTable2.NewRow();
					dataRow2["ParentColumn"] = value;
					dataRow2["Identity"] = text2 + "\\Subnets\\" + databaseAvailabilityGroupNetworkSubnet.SubnetId.IPRange.ToString();
					dataRow2["Name"] = databaseAvailabilityGroupNetworkSubnet.SubnetId.IPRange.ToString();
					dataRow2["State"] = databaseAvailabilityGroupNetworkSubnet.State;
					dataRow2["NetworkIdentity"] = text.Substring(text.IndexOf("\\") + 1);
					dataRow2["DatabaseAvailabilityGroup"] = text.Substring(0, text.IndexOf("\\"));
					dataRow2["OriginalIdentity"] = text;
					switch (databaseAvailabilityGroupNetworkSubnet.State)
					{
					case DatabaseAvailabilityGroupNetworkSubnet.SubnetState.Unknown:
						dataRow2["StateIcon"] = "DagSubnetUnknown";
						break;
					case DatabaseAvailabilityGroupNetworkSubnet.SubnetState.Up:
						dataRow2["StateIcon"] = "DagSubnetUp";
						break;
					case DatabaseAvailabilityGroupNetworkSubnet.SubnetState.Down:
						dataRow2["StateIcon"] = "DagSubnetDown";
						break;
					case DatabaseAvailabilityGroupNetworkSubnet.SubnetState.Partitioned:
						dataRow2["StateIcon"] = "DagSubnetPartitioned";
						break;
					case DatabaseAvailabilityGroupNetworkSubnet.SubnetState.Misconfigured:
						dataRow2["StateIcon"] = "DagSubnetMisconfigured";
						break;
					case DatabaseAvailabilityGroupNetworkSubnet.SubnetState.Unavailable:
						dataRow2["StateIcon"] = "DagSubnetUnavailable";
						break;
					}
					dataTable2.Rows.Add(dataRow2);
				}
				dataRow2 = dataTable2.NewRow();
				dataRow2["ParentColumn"] = text2;
				dataRow2["Identity"] = text2 + "\\NetworkInterface";
				dataRow2["Name"] = Strings.NetworkInterfaces;
				dataRow2["NetworkIdentity"] = text.Substring(text.IndexOf("\\") + 1);
				dataRow2["DatabaseAvailabilityGroup"] = text.Substring(0, text.IndexOf("\\"));
				dataRow2["OriginalIdentity"] = text;
				dataRow2["Image"] = "DAGNetworkInterface";
				dataTable2.Rows.Add(dataRow2);
				value = (string)dataRow2["Identity"];
				DagNetMultiValuedProperty<DatabaseAvailabilityGroupNetworkInterface> dagNetMultiValuedProperty2 = (DagNetMultiValuedProperty<DatabaseAvailabilityGroupNetworkInterface>)dataRow["Interfaces"];
				foreach (DatabaseAvailabilityGroupNetworkInterface databaseAvailabilityGroupNetworkInterface in dagNetMultiValuedProperty2)
				{
					dataRow2 = dataTable2.NewRow();
					dataRow2["ParentColumn"] = value;
					dataRow2["Identity"] = text2 + "\\NetworkInterface\\" + databaseAvailabilityGroupNetworkInterface.IPAddress.ToString();
					dataRow2["Name"] = databaseAvailabilityGroupNetworkInterface.IPAddress.ToString();
					dataRow2["State"] = databaseAvailabilityGroupNetworkInterface.State;
					dataRow2["NetworkIdentity"] = text.Substring(text.IndexOf("\\") + 1);
					dataRow2["DatabaseAvailabilityGroup"] = text.Substring(0, text.IndexOf("\\"));
					dataRow2["OriginalIdentity"] = text;
					switch (databaseAvailabilityGroupNetworkInterface.State)
					{
					case DatabaseAvailabilityGroupNetworkInterface.InterfaceState.Unknown:
						dataRow2["StateIcon"] = "DagNetworkInterfaceUnknown";
						break;
					case DatabaseAvailabilityGroupNetworkInterface.InterfaceState.Up:
						dataRow2["StateIcon"] = "DagNetworkInterfaceUp";
						break;
					case DatabaseAvailabilityGroupNetworkInterface.InterfaceState.Failed:
						dataRow2["StateIcon"] = "DagNetworkInterfaceFailed";
						break;
					case DatabaseAvailabilityGroupNetworkInterface.InterfaceState.Unreachable:
						dataRow2["StateIcon"] = "DagNetworkInterfaceUnreachable";
						break;
					case DatabaseAvailabilityGroupNetworkInterface.InterfaceState.Unavailable:
						dataRow2["StateIcon"] = "DagNetworkInterfaceUnavailable";
						break;
					}
					dataTable2.Rows.Add(dataRow2);
				}
			}
			dataTable2.EndLoadData();
			table.Merge(dataTable2);
		}

		public override object Clone()
		{
			return new DAGNetworkDataFiller(base.CommandText, this.CommandBuilder)
			{
				ResolveCommandText = base.ResolveCommandText,
				IsResolving = base.IsResolving
			};
		}
	}
}
