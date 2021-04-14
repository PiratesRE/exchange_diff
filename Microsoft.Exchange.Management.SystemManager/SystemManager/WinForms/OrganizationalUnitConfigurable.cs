using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class OrganizationalUnitConfigurable : IResultsLoaderConfiguration
	{
		public static DataTable GetDataTable()
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.AddRange(ConfigurableHelper.GetCommonDataColumns());
			dataTable.PrimaryKey = new DataColumn[]
			{
				dataTable.Columns["Identity"]
			};
			dataTable.Columns.Add(new DataColumn("Type", typeof(string)));
			dataTable.Columns.Add(new DataColumn("CanonicalName", typeof(string)));
			DataColumn column = new DataColumn("ImageProperty", typeof(string), string.Format("IIF({0}='Domain', 'domainDNS', IIF({0}='Container', 'container', 'organizationalUnit'))", "Type"));
			dataTable.Columns.Add(column);
			return dataTable;
		}

		public ResultsLoaderProfile BuildResultsLoaderProfile()
		{
			DataTable inputTable = new DataTable();
			ResultsColumnProfile resultsColumnProfile = new ResultsColumnProfile("CanonicalName", true, Strings.Name);
			ResultsLoaderProfile resultsLoaderProfile = new ResultsLoaderProfile(Strings.OrganizationalUnit, false, "ImageProperty", inputTable, OrganizationalUnitConfigurable.GetDataTable(), new ResultsColumnProfile[]
			{
				resultsColumnProfile
			});
			resultsLoaderProfile.UseTreeViewForm = true;
			ExchangeCommandBuilder exchangeCommandBuilder = new ExchangeCommandBuilder();
			exchangeCommandBuilder.ScopeBuilder = new OrganizationalUnitScopeBuilder();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["IncludeContainers"] = string.Empty;
			resultsLoaderProfile.AddTableFiller(new MonadAdapterFiller("Get-OrganizationalUnit", exchangeCommandBuilder)
			{
				Parameters = dictionary,
				AddtionalParameters = 
				{
					"Identity",
					"SingleNodeOnly"
				}
			});
			resultsLoaderProfile.HelpTopic = "Microsoft.Exchange.Management.SystemManager.WinForms.OrganizationalUnitPicker";
			return resultsLoaderProfile;
		}
	}
}
