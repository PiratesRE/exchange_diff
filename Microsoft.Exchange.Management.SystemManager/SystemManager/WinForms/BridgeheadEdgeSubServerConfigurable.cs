using System;
using System.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class BridgeheadEdgeSubServerConfigurable : IResultsLoaderConfiguration
	{
		public static DataTable GetDataTable()
		{
			DataTable commonDataTable = ConfigurableHelper.GetCommonDataTable();
			DataColumn dataColumn = new DataColumn("ImageProperty", typeof(string));
			dataColumn.DefaultValue = "ExchangeServerPicker";
			commonDataTable.Columns.Add(dataColumn);
			commonDataTable.Columns.Add(new DataColumn("NetworkAddress", typeof(string)));
			commonDataTable.Columns.Add(new DataColumn("Fqdn", typeof(string)));
			commonDataTable.Columns.Add(new DataColumn("ServerRole", typeof(ServerRole)));
			commonDataTable.Columns.Add(new DataColumn("Site", typeof(string)));
			DataColumn dataColumn2 = new DataColumn("ADSiteShortName", typeof(string));
			dataColumn2.ExtendedProperties["LambdaExpression"] = string.Format("{0}=>iif(DBNull.Value.Equals(@0[{0}]), String.Empty, WinformsHelper.GetADShortName(String(@0[{0}])))", "Site");
			commonDataTable.Columns.Add(dataColumn2);
			commonDataTable.Columns.Add(new DataColumn("CustomerFeedbackEnabled", typeof(bool)));
			commonDataTable.Columns.Add(new DataColumn("AdminDisplayVersion", typeof(string)));
			commonDataTable.Columns.Add(new DataColumn("ExchangeLegacyDN", typeof(string)));
			DataColumn column = new DataColumn("CeipStatus", typeof(string), string.Format("IIF({0}=true, '{1}', IIF({0}=false, '{2}', '{3}'))", new object[]
			{
				"CustomerFeedbackEnabled",
				Strings.CeipStatusOptedIn,
				Strings.CeipStatusOptedOut,
				Strings.CeipStatusNotEnrolled
			}));
			commonDataTable.Columns.Add(column);
			return commonDataTable;
		}

		public ResultsLoaderProfile BuildResultsLoaderProfile()
		{
			DataTable dataTable = new DataTable();
			DataColumn column = new DataColumn("MinVersion", typeof(long));
			DataColumn column2 = new DataColumn("DesiredServerRoleBitMask", typeof(ServerRole));
			DataColumn column3 = new DataColumn("IncludeLegacyServer", typeof(bool));
			dataTable.Columns.Add(column);
			dataTable.Columns.Add(column2);
			dataTable.Columns.Add(column3);
			ResultsColumnProfile resultsColumnProfile = new ResultsColumnProfile("Name", true, Strings.Name);
			ResultsColumnProfile resultsColumnProfile2 = new ResultsColumnProfile("ADSiteShortName", true, Strings.ServerSite);
			ResultsColumnProfile resultsColumnProfile3 = new ResultsColumnProfile("ServerRole", true, Strings.ServerRole);
			return new ResultsLoaderProfile(Strings.BridgeheadsAndEdgeSubscriptionsPickerText, "ImageProperty", "Get-ExchangeServer", dataTable, BridgeheadEdgeSubServerConfigurable.GetDataTable(), new ResultsColumnProfile[]
			{
				resultsColumnProfile,
				resultsColumnProfile2,
				resultsColumnProfile3
			}, new ExchangeCommandBuilder(new ExchangeServerFilterBuilder()))
			{
				HelpTopic = "Microsoft.Exchange.Management.SystemManager.WinForms.BridgeheadEdgeSubscriptionPicker"
			};
		}
	}
}
