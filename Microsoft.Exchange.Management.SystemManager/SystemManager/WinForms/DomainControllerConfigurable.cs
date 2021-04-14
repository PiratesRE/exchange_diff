using System;
using System.Data;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class DomainControllerConfigurable : IResultsLoaderConfiguration
	{
		public ResultsLoaderProfile BuildResultsLoaderProfile()
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("Forest", typeof(string)));
			dataTable.Columns.Add(new DataColumn("Credential", typeof(PSCredential)));
			dataTable.Columns.Add(new DataColumn("DomainName", typeof(string)));
			dataTable.Columns.Add(new DataColumn("IsGlobalCatalog", typeof(bool)));
			DataTable commonDataTable = ConfigurableHelper.GetCommonDataTable();
			commonDataTable.Columns.Add(new DataColumn("DnsHostName", typeof(string)));
			commonDataTable.Columns.Add(new DataColumn("ADSite", typeof(ADObjectId)));
			DataColumn dataColumn = new DataColumn("ADSiteShortName", typeof(string));
			dataColumn.ExtendedProperties["LambdaExpression"] = string.Format("{0}=>ADObjectId(@0[{0}]).Name", "ADSite");
			commonDataTable.Columns.Add(dataColumn);
			DataColumn dataColumn2 = new DataColumn("ImageProperty", typeof(string));
			dataColumn2.DefaultValue = "ExchangeServerPicker";
			commonDataTable.Columns.Add(dataColumn2);
			ResultsColumnProfile resultsColumnProfile = new ResultsColumnProfile("DnsHostName", true, Strings.Name);
			ResultsColumnProfile resultsColumnProfile2 = new ResultsColumnProfile("ADSiteShortName", true, Strings.Site);
			ResultsLoaderProfile resultsLoaderProfile = new ResultsLoaderProfile(Strings.DomainController, false, "ImageProperty", dataTable, commonDataTable, new ResultsColumnProfile[]
			{
				resultsColumnProfile,
				resultsColumnProfile2
			})
			{
				SerializationLevel = ExchangeRunspaceConfigurationSettings.SerializationLevel.Full
			};
			resultsLoaderProfile.NameProperty = "DnsHostName";
			resultsLoaderProfile.AddTableFiller(new ProcedureDataFiller("Get-DomainController", new DomainControllerCommandBuilder
			{
				ResolveProperty = "Identity",
				NamePropertyFilter = "DnsHostName",
				SearchType = 2,
				UseFilterToResolveNonId = false
			}));
			resultsLoaderProfile.HelpTopic = "Microsoft.Exchange.Management.SystemManager.WinForms.DomainControllerPicker";
			return resultsLoaderProfile;
		}
	}
}
