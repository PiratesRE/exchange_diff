using System;
using System.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class UPNSuffixesConfigurable : IResultsLoaderConfiguration
	{
		public ResultsLoaderProfile BuildResultsLoaderProfile()
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("OtherSuffix", typeof(string)));
			dataTable.Columns.Add(new DataColumn("OrganizationalUnit", typeof(ADObjectId)));
			DataTable dataTable2 = new DataTable();
			DataColumn dataColumn = new DataColumn("CanonicalName", typeof(string));
			dataTable2.Columns.Add(dataColumn);
			dataTable2.PrimaryKey = new DataColumn[]
			{
				dataColumn
			};
			ResultsColumnProfile resultsColumnProfile = new ResultsColumnProfile("CanonicalName", true, Strings.Name);
			ResultsLoaderProfile resultsLoaderProfile = new ResultsLoaderProfile(null, true, "ImageProperty", dataTable, dataTable2, new ResultsColumnProfile[]
			{
				resultsColumnProfile
			});
			resultsLoaderProfile.WholeObjectProperty = "CanonicalName";
			resultsLoaderProfile.NameProperty = "CanonicalName";
			resultsLoaderProfile.DistinguishIdentity = "CanonicalName";
			resultsLoaderProfile.AddTableFiller(new MonadAdapterFiller("Get-UserPrincipalNamesSuffix", new ExchangeCommandBuilder(new UPNSuffixesFilterBuilder())));
			resultsLoaderProfile.AddTableFiller(new OtherUPNSuffixFiller("OtherSuffix", "CanonicalName"));
			resultsLoaderProfile.HelpTopic = "Microsoft.Exchange.Management.SystemManager.WinForms.UPNSuffixesPicker";
			return resultsLoaderProfile;
		}
	}
}
