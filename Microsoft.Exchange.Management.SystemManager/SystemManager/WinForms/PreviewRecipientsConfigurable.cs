using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class PreviewRecipientsConfigurable : IResultsLoaderConfiguration
	{
		public ResultsLoaderProfile BuildResultsLoaderProfile()
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("RecipientPreviewFilter", typeof(QueryFilter)));
			ResultsColumnProfile resultsColumnProfile = new ResultsColumnProfile("DisplayName", true, Strings.DisplayNameColumnInPicker);
			ResultsColumnProfile resultsColumnProfile2 = new ResultsColumnProfile("Alias", true, Strings.AliasColumnInPicker);
			ResultsColumnProfile resultsColumnProfile3 = new ResultsColumnProfile("RecipientTypeDetails", true, Strings.RecipientTypeDetailsColumnInPicker);
			ResultsColumnProfile resultsColumnProfile4 = new ResultsColumnProfile("PrimarySmtpAddressToString", true, Strings.PrimarySmtpAddressColumnInPicker);
			ResultsLoaderProfile resultsLoaderProfile = new ResultsLoaderProfile(Strings.Recipient, false, "RecipientTypeDetails", dataTable, ContactConfigurable.GetDataTable(), new ResultsColumnProfile[]
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
			dictionary["RecipientTypeDetails"] = "RemoteUserMailbox,RemoteRoomMailbox,RemoteEquipmentMailbox,RemoteSharedMailbox,RemoteTeamMailbox,MailUser,UserMailbox,LinkedMailbox,SharedMailbox,TeamMailbox,LegacyMailbox,RoomMailbox,EquipmentMailbox,PublicFolder,MailContact,MailForestContact,MailUniversalDistributionGroup,MailUniversalSecurityGroup,MailNonUniversalGroup,DynamicDistributionGroup";
			resultsLoaderProfile.AddTableFiller(new MonadAdapterFiller("Get-Recipient", new ExchangeCommandBuilder(new RecipientPreviewFilterBuilder())
			{
				SearchType = 0
			})
			{
				Parameters = dictionary,
				AddtionalParameters = 
				{
					"RecipientPreviewFilter"
				}
			});
			resultsLoaderProfile.NameProperty = "DisplayName";
			resultsLoaderProfile.HelpTopic = "Microsoft.Exchange.Management.SystemManager.WinForms.PreviewRecipientsPicker";
			return resultsLoaderProfile;
		}
	}
}
