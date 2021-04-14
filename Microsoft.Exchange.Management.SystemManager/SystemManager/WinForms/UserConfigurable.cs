using System;
using System.Data;
using System.Management.Automation;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class UserConfigurable : IResultsLoaderConfiguration
	{
		public ResultsLoaderProfile BuildResultsLoaderProfile()
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("UserRange", typeof(UserRange)));
			dataTable.Columns.Add(new DataColumn("NameForFilter", typeof(string)));
			dataTable.Columns.Add(new DataColumn("DisplayNameForFilter", typeof(string)));
			dataTable.Columns.Add(new DataColumn("DomainController", typeof(string)));
			dataTable.Columns.Add(new DataColumn("Credential", typeof(PSCredential)));
			DataTable dataTable2 = ContactConfigurable.GetDataTable();
			dataTable2.Columns.Add(new DataColumn("UserPrincipalName", typeof(string)));
			dataTable2.Columns.Add(new DataColumn("SamAccountName", typeof(string)));
			DataColumn dataColumn = new DataColumn("LogonName", typeof(string));
			dataColumn.ExtendedProperties["LambdaExpression"] = "SamAccountName,DistinguishedName,Guid=>WinformsHelper.GenerateADObjectId(Guid(@0[\"Guid\"]),@0[DistinguishedName].ToString()).DomainId.Name + \"\\\" + @0[SamAccountName]";
			dataTable2.Columns.Add(dataColumn);
			ResultsColumnProfile[] displayedColumnCollection = new ResultsColumnProfile[]
			{
				new ResultsColumnProfile("City", false, Strings.CityColumnInPicker),
				new ResultsColumnProfile("Company", false, Strings.CompanyColumnInPicker),
				new ResultsColumnProfile("DisplayName", false, Strings.DisplayNameColumnInPicker),
				new ResultsColumnProfile("Department", false, Strings.DepartmentColumnInPicker),
				new ResultsColumnProfile("FirstName", false, Strings.FirstNameColumnInPicker),
				new ResultsColumnProfile("LastName", false, Strings.LastNameColumnInPicker),
				new ResultsColumnProfile("Manager", false, Strings.ManagerColumnInPicker),
				new ResultsColumnProfile("Office", false, Strings.OfficeColumnInPicker),
				new ResultsColumnProfile("Phone", false, Strings.PhoneColumnInPicker),
				new ResultsColumnProfile("StateOrProvince", false, Strings.StateOrProvinceColumnInPicker),
				new ResultsColumnProfile("Title", false, Strings.TitleColumnInPicker),
				new ResultsColumnProfile("Name", true, Strings.NameColumnInPicker),
				new ResultsColumnProfile("OrganizationalUnit", true, Strings.OUColumnInPicker)
			};
			ResultsLoaderProfile resultsLoaderProfile = new ResultsLoaderProfile(Strings.User, false, "RecipientTypeDetails", dataTable, dataTable2, displayedColumnCollection);
			resultsLoaderProfile.AddTableFiller(new ProcedureDataFiller("Get-User", new UserCommandBuilder
			{
				SearchType = 0,
				UseFilterToResolveNonId = true
			}));
			resultsLoaderProfile.HelpTopic = "Microsoft.Exchange.Management.SystemManager.WinForms.OrganizationalUnitPicker";
			return resultsLoaderProfile;
		}
	}
}
