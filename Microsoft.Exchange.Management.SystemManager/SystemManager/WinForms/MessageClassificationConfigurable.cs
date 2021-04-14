using System;
using System.Data;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class MessageClassificationConfigurable : IResultsLoaderConfiguration
	{
		public ResultsLoaderProfile BuildResultsLoaderProfile()
		{
			DataTable inputTable = new DataTable();
			DataTable commonDataTable = ConfigurableHelper.GetCommonDataTable();
			commonDataTable.Columns.Add(new DataColumn("DisplayName", typeof(string)));
			ResultsColumnProfile resultsColumnProfile = new ResultsColumnProfile("DisplayName", true, Strings.DisplayNameColumnInPicker);
			return new ResultsLoaderProfile(Strings.MessageClassification, true, null, "Get-MessageClassification", inputTable, commonDataTable, new ResultsColumnProfile[]
			{
				resultsColumnProfile
			}, new ExchangeCommandBuilder())
			{
				NameProperty = "DisplayName",
				HelpTopic = "Microsoft.Exchange.Management.SystemManager.WinForms.MessageClassificationPicker"
			};
		}
	}
}
