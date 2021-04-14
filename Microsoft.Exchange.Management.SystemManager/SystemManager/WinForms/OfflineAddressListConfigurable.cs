using System;
using System.Data;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class OfflineAddressListConfigurable : IResultsLoaderConfiguration
	{
		public ResultsLoaderProfile BuildResultsLoaderProfile()
		{
			DataTable inputTable = new DataTable();
			DataTable commonDataTable = ConfigurableHelper.GetCommonDataTable();
			DataColumn dataColumn = new DataColumn("ImageProperty", typeof(string));
			dataColumn.DefaultValue = "OfflineAddressBookPicker";
			commonDataTable.Columns.Add(dataColumn);
			ResultsColumnProfile resultsColumnProfile = new ResultsColumnProfile("Name", true, Strings.Name);
			return new ResultsLoaderProfile(Strings.OfflineAddressList, "ImageProperty", "Get-OfflineAddressBook", inputTable, commonDataTable, new ResultsColumnProfile[]
			{
				resultsColumnProfile
			}, new ExchangeCommandBuilder())
			{
				HelpTopic = "Microsoft.Exchange.Management.SystemManager.WinForms.OfflineAddressListPicker"
			};
		}
	}
}
