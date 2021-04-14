using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class MerMailEnableRecipientConfigurable : IResultsLoaderConfiguration
	{
		public MerMailEnableRecipientConfigurable() : this(true, true, true, Strings.Recipient)
		{
		}

		protected MerMailEnableRecipientConfigurable(bool allowedGroups, bool allowedNonGroups, bool allowedPublicFolder, string displayName)
		{
			this.allowedGroups = allowedGroups;
			this.allowedNonGroups = allowedNonGroups;
			this.allowedPublicFolder = allowedPublicFolder;
			this.displayName = displayName;
		}

		public virtual ResultsLoaderProfile BuildResultsLoaderProfile()
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("ExcludeObject", typeof(ADObjectId)));
			ResultsColumnProfile resultsColumnProfile = new ResultsColumnProfile("DisplayName", true, Strings.DisplayNameColumnInPicker);
			ResultsColumnProfile resultsColumnProfile2 = new ResultsColumnProfile("Alias", true, Strings.AliasColumnInPicker);
			ResultsColumnProfile resultsColumnProfile3 = new ResultsColumnProfile("RecipientTypeDetails", true, Strings.RecipientTypeDetailsColumnInPicker);
			ResultsColumnProfile resultsColumnProfile4 = new ResultsColumnProfile("PrimarySmtpAddressToString", true, Strings.PrimarySmtpAddressColumnInPicker);
			ResultsLoaderProfile resultsLoaderProfile = new ResultsLoaderProfile(this.displayName, false, "RecipientTypeDetails", dataTable, ContactConfigurable.GetDataTable(), new ResultsColumnProfile[]
			{
				resultsColumnProfile,
				resultsColumnProfile2,
				resultsColumnProfile3,
				resultsColumnProfile4
			});
			resultsLoaderProfile.ScopeSupportingLevel = ScopeSupportingLevel.FullScoping;
			resultsLoaderProfile.WholeObjectProperty = "WholeObjectProperty";
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["ResultSize"] = "Unlimited";
			dictionary["PropertySet"] = "ConsoleSmallSet";
			StringBuilder stringBuilder = new StringBuilder();
			if (this.allowedNonGroups)
			{
				stringBuilder.Append(",RoomMailbox,EquipmentMailbox,LegacyMailbox,LinkedMailbox,UserMailbox,MailContact,MailForestContact,MailUser,RemoteUserMailbox,RemoteRoomMailbox,RemoteEquipmentMailbox,RemoteSharedMailbox,SharedMailbox");
			}
			if (this.allowedPublicFolder)
			{
				stringBuilder.Append(",PublicFolder");
			}
			if (this.allowedGroups)
			{
				stringBuilder.Append(",DynamicDistributionGroup,MailNonUniversalGroup,MailUniversalDistributionGroup,MailUniversalSecurityGroup");
			}
			stringBuilder.Remove(0, 1);
			dictionary["RecipientTypeDetails"] = stringBuilder.ToString();
			resultsLoaderProfile.AddTableFiller(new MonadAdapterFiller("Get-Recipient", new ExchangeCommandBuilder(new WithPrimarySmtpAddressRecipientFilterBuilder())
			{
				SearchType = 0
			})
			{
				Parameters = dictionary
			});
			resultsLoaderProfile.NameProperty = "DisplayName";
			resultsLoaderProfile.HelpTopic = "Microsoft.Exchange.Management.SystemManager.WinForms.MerMailEnableRecipientConfigurable";
			return resultsLoaderProfile;
		}

		private bool allowedGroups;

		private bool allowedNonGroups;

		private bool allowedPublicFolder;

		private string displayName;
	}
}
