using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class RecipientSlabControl : SlabControl
	{
		public LocalSearchFilterEditorFeatureSet RecipientFilterEditorFeatureSet { get; set; }

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.listView = this.Controls.OfType<ListView>().First<ListView>();
			Dictionary<string, object> dictionary = new Dictionary<string, object>(RecipientSlabControl.dictCommonValidPropValues);
			dictionary.Add("RecipientTypeDetails", RecipientSlabControl.dictValidRecTypes[this.RecipientFilterEditorFeatureSet]);
			if (this.RecipientFilterEditorFeatureSet == LocalSearchFilterEditorFeatureSet.Contacts || this.RecipientFilterEditorFeatureSet == LocalSearchFilterEditorFeatureSet.Mailboxes || this.RecipientFilterEditorFeatureSet == LocalSearchFilterEditorFeatureSet.ResourceMailboxes || this.RecipientFilterEditorFeatureSet == LocalSearchFilterEditorFeatureSet.SharedMailboxes || this.RecipientFilterEditorFeatureSet == LocalSearchFilterEditorFeatureSet.Members)
			{
				List<string> list = new List<string>(CountryInfo.AllCountryInfos.Count * 2);
				foreach (CountryInfo countryInfo in CountryInfo.AllCountryInfos)
				{
					list.Add(countryInfo.Name);
					list.Add(countryInfo.LocalizedDisplayName);
				}
				dictionary.Add("CountryOrRegion", string.Join(",", list.ToArray()));
			}
			this.listView.SearchTextBox.Attributes.Add("vm-ValidPropertyValueRange", dictionary.ToJsonString(null).ToLowerInvariant());
		}

		protected ListView listView;

		private static readonly Dictionary<LocalSearchFilterEditorFeatureSet, string> dictValidRecTypes = new Dictionary<LocalSearchFilterEditorFeatureSet, string>
		{
			{
				LocalSearchFilterEditorFeatureSet.Contacts,
				"MailContact,MailUser"
			},
			{
				LocalSearchFilterEditorFeatureSet.DistributionGroups,
				"MailNonUniversalGroup,MailUniversalDistributionGroup,MailUniversalSecurityGroup,DynamicDistributionGroup"
			},
			{
				LocalSearchFilterEditorFeatureSet.Mailboxes,
				"UserMailbox,LinkedMailbox,LegacyMailbox,RemoteUserMailbox"
			},
			{
				LocalSearchFilterEditorFeatureSet.ResourceMailboxes,
				"RoomMailbox,EquipmentMailbox"
			},
			{
				LocalSearchFilterEditorFeatureSet.SharedMailboxes,
				"SharedMailbox"
			},
			{
				LocalSearchFilterEditorFeatureSet.Members,
				"Members"
			}
		};

		private static readonly Dictionary<string, object> dictCommonValidPropValues = (from a in StringExtension.QueryNameToPropertyDef
		where a.Value.Type == typeof(bool)
		select a.Key).ToDictionary((string p) => p, (string q) => "True,False", StringComparer.InvariantCultureIgnoreCase);
	}
}
