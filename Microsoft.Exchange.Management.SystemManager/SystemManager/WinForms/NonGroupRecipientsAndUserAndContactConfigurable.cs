using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class NonGroupRecipientsAndUserAndContactConfigurable : IResultsLoaderConfiguration
	{
		public ResultsLoaderProfile BuildResultsLoaderProfile()
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("ExcludeObject", typeof(ADObjectId)));
			ResultsColumnProfile resultsColumnProfile = new ResultsColumnProfile("DisplayName", true, Strings.DisplayNameColumnInPicker);
			ResultsColumnProfile resultsColumnProfile2 = new ResultsColumnProfile("Alias", true, Strings.AliasColumnInPicker);
			ResultsColumnProfile resultsColumnProfile3 = new ResultsColumnProfile("RecipientTypeDetails", true, Strings.RecipientTypeDetailsColumnInPicker);
			ResultsColumnProfile resultsColumnProfile4 = new ResultsColumnProfile("PrimarySmtpAddressToString", true, Strings.PrimarySmtpAddressColumnInPicker);
			ResultsLoaderProfile resultsLoaderProfile = new ResultsLoaderProfile(Strings.RecipientUserOrContact, false, "RecipientTypeDetails", dataTable, ContactConfigurable.GetDataTable(), new ResultsColumnProfile[]
			{
				resultsColumnProfile,
				resultsColumnProfile2,
				resultsColumnProfile3,
				resultsColumnProfile4
			});
			resultsLoaderProfile.ScopeSupportingLevel = ScopeSupportingLevel.FullScoping;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["ResultSize"] = "Unlimited";
			dictionary["PropertySet"] = "ConsoleSmallSet";
			dictionary["RecipientTypeDetails"] = "RemoteUserMailbox,RemoteRoomMailbox,RemoteEquipmentMailbox,RemoteSharedMailbox,MailUser,UserMailbox,LinkedMailbox,SharedMailbox,LegacyMailbox,RoomMailbox,EquipmentMailbox,MailContact,MailForestContact";
			resultsLoaderProfile.AddTableFiller(new MonadAdapterFiller("Get-Recipient", new ExchangeCommandBuilder(new ExcludeObjectFilterBuilder())
			{
				SearchType = 0
			})
			{
				Parameters = dictionary
			});
			dictionary = new Dictionary<string, string>();
			dictionary["ResultSize"] = "Unlimited";
			dictionary["RecipientTypeDetails"] = "User";
			resultsLoaderProfile.AddTableFiller(new MonadAdapterFiller("Get-User", new ExchangeCommandBuilder(new ExcludeObjectFilterBuilder())
			{
				SearchType = 0
			})
			{
				Parameters = dictionary
			});
			dictionary = new Dictionary<string, string>();
			dictionary["ResultSize"] = "Unlimited";
			dictionary["RecipientTypeDetails"] = "Contact";
			resultsLoaderProfile.AddTableFiller(new MonadAdapterFiller("Get-Contact", new ExchangeCommandBuilder(new ExcludeObjectFilterBuilder())
			{
				SearchType = 0
			})
			{
				Parameters = dictionary
			});
			resultsLoaderProfile.NameProperty = "DisplayName";
			resultsLoaderProfile.HelpTopic = "Microsoft.Exchange.Management.SystemManager.WinForms.NonGroupRecipientsAndUserAndContactPicker";
			return resultsLoaderProfile;
		}
	}
}
