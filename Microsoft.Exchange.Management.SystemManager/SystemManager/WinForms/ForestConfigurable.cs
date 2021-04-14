using System;
using System.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ForestConfigurable : IResultsLoaderConfiguration
	{
		public ResultsLoaderProfile BuildResultsLoaderProfile()
		{
			DataTable commonDataTable = ConfigurableHelper.GetCommonDataTable();
			commonDataTable.Columns.Add(new DataColumn("TrustType", typeof(ADTrustType)));
			DataColumn dataColumn = new DataColumn("ImageProperty", typeof(string));
			dataColumn.DefaultValue = "ExchangeServerPicker";
			commonDataTable.Columns.Add(dataColumn);
			ResultsColumnProfile resultsColumnProfile = new ResultsColumnProfile("Name", true, Strings.Name);
			ResultsColumnProfile resultsColumnProfile2 = new ResultsColumnProfile("TrustType", true, Strings.TrustType);
			ResultsLoaderProfile resultsLoaderProfile = new ResultsLoaderProfile(Strings.ForestDomain, false, "ImageProperty", new DataTable(), commonDataTable, new ResultsColumnProfile[]
			{
				resultsColumnProfile,
				resultsColumnProfile2
			});
			resultsLoaderProfile.AddTableFiller(new MonadAdapterFiller("Get-Trust", new ExchangeCommandBuilder
			{
				SearchType = 2,
				NamePropertyFilter = resultsLoaderProfile.NameProperty
			}));
			resultsLoaderProfile.HelpTopic = "Microsoft.Exchange.Management.SystemManager.WinForms.ForestPicker";
			return resultsLoaderProfile;
		}
	}
}
