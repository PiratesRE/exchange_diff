using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ContactConfigurable : IResultsLoaderConfiguration
	{
		public static DataTable GetDataTable()
		{
			DataTable commonDataTable = ConfigurableHelper.GetCommonDataTable();
			commonDataTable.Columns.Add(new DataColumn("City", typeof(string)));
			commonDataTable.Columns.Add(new DataColumn("Company", typeof(string)));
			commonDataTable.Columns.Add(new DataColumn("DisplayName", typeof(string)));
			commonDataTable.Columns.Add(new DataColumn("Department", typeof(string)));
			commonDataTable.Columns.Add(new DataColumn("FirstName", typeof(string)));
			commonDataTable.Columns.Add(new DataColumn("LastName", typeof(string)));
			commonDataTable.Columns.Add(new DataColumn("Manager", typeof(string)));
			commonDataTable.Columns.Add(new DataColumn("Office", typeof(string)));
			commonDataTable.Columns.Add(new DataColumn("Phone", typeof(string)));
			commonDataTable.Columns.Add(new DataColumn("StateOrProvince", typeof(string)));
			commonDataTable.Columns.Add(new DataColumn("Title", typeof(string)));
			commonDataTable.Columns.Add(new DataColumn("OrganizationalUnit", typeof(string)));
			commonDataTable.Columns.Add(new DataColumn("RecipientTypeDetails", typeof(RecipientTypeDetails)));
			commonDataTable.Columns.Add(new DataColumn("RecipientType", typeof(RecipientType)));
			commonDataTable.Columns.Add(new DataColumn("GroupType", typeof(GroupTypeFlags)));
			commonDataTable.Columns.Add(new DataColumn("ManagedBy", typeof(string)));
			commonDataTable.Columns.Add(new DataColumn("DatabaseName", typeof(string)));
			commonDataTable.Columns.Add(new DataColumn("ExpansionServer", typeof(string)));
			commonDataTable.Columns.AddColumnWithExpectedType("ExternalEmailAddress", typeof(ProxyAddress));
			commonDataTable.Columns.Add(new DataColumn("HiddenFromAddressListsEnabled", typeof(bool)));
			commonDataTable.Columns.Add(new DataColumn("UMEnabled", typeof(bool)));
			commonDataTable.Columns.Add(new DataColumn("Alias", typeof(string)));
			commonDataTable.Columns.AddColumnWithExpectedType("PrimarySmtpAddress", typeof(SmtpAddress));
			commonDataTable.Columns.AddColumnWithLambdaExpression("PrimarySmtpAddressToString", typeof(string), "PrimarySmtpAddress=>@0[PrimarySmtpAddress].ToString()");
			commonDataTable.Columns.AddColumnWithLambdaExpression("NonEmptyDisplayName", typeof(string), "DisplayName,Name=>iif(DBNull.Value.Equals(@0[DisplayName]) or String(@0[DisplayName]) == String.Empty, @0[Name].ToString(),@0[DisplayName].ToString())");
			commonDataTable.Columns.AddColumnWithLambdaExpression("DisplayRecipientTypeDetails", typeof(string), "RecipientTypeDetails=>@0[\"RecipientTypeDetails\"].ToString()");
			commonDataTable.Columns.Add(new DataColumn("WholeObjectProperty", typeof(object)));
			return commonDataTable;
		}

		public ResultsLoaderProfile BuildResultsLoaderProfile()
		{
			ResultsColumnProfile resultsColumnProfile = new ResultsColumnProfile("Name", true, Strings.Name);
			ResultsColumnProfile resultsColumnProfile2 = new ResultsColumnProfile("OrganizationalUnit", true, Strings.OUColumnInPicker);
			ResultsLoaderProfile resultsLoaderProfile = new ResultsLoaderProfile(Strings.Contact, false, "RecipientTypeDetails", new DataTable(), ContactConfigurable.GetDataTable(), new ResultsColumnProfile[]
			{
				resultsColumnProfile,
				resultsColumnProfile2
			});
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["ResultSize"] = "Unlimited";
			dictionary["RecipientTypeDetails"] = "Contact";
			resultsLoaderProfile.AddTableFiller(new MonadAdapterFiller("Get-Contact", new ExchangeCommandBuilder
			{
				SearchType = 0
			})
			{
				Parameters = dictionary
			});
			resultsLoaderProfile.HelpTopic = "Microsoft.Exchange.Management.SystemManager.WinForms.ContactPicker";
			return resultsLoaderProfile;
		}
	}
}
