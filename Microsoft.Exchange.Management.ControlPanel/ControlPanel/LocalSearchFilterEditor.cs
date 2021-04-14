using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class LocalSearchFilterEditor : MoreOptionRecipientConditionEditor
	{
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.FeatureSet = new LocalSearchFilterEditorFeatureSet?(this.FeatureSet ?? ((LocalSearchFilterEditorFeatureSet)Enum.Parse(typeof(LocalSearchFilterEditorFeatureSet), this.Page.Request.QueryString["FeatureSet"])));
		}

		public LocalSearchFilterEditorFeatureSet? FeatureSet { get; set; }

		protected override RulePhrase[] SupportedConditions
		{
			get
			{
				return (from phrase in LocalSearchFilterEditor.allSupportedRules
				where this.ShouldShow(phrase)
				select phrase).ToArray<RulePhrase>();
			}
		}

		private bool ShouldShow(RulePhrase phrase)
		{
			if (string.IsNullOrEmpty(phrase.AdditionalRoles))
			{
				return true;
			}
			Regex regex = new Regex("([\\w,]*)\\+([\\w,]*)");
			string[] array = regex.Match(phrase.AdditionalRoles).Groups[1].Value.Split(new char[]
			{
				','
			});
			string[] array2 = regex.Match(phrase.AdditionalRoles).Groups[2].Value.Split(new char[]
			{
				','
			});
			bool flag = array.Length == 1 && array[0] == string.Empty;
			if (array.Length > 0 && !flag && array.All((string feature) => !this.FeatureSet.Equals(Enum.Parse(typeof(LocalSearchFilterEditorFeatureSet), feature, true))))
			{
				return false;
			}
			bool flag2 = array2.Length == 1 && array2[0] == string.Empty;
			if (array2.Length > 0 && !flag2)
			{
				if (array2.All((string topology) => !RbacPrincipal.Current.IsInRole(topology)))
				{
					return false;
				}
			}
			return true;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static LocalSearchFilterEditor()
		{
			RulePhrase[] array = new RulePhrase[31];
			array[0] = new RulePhrase("ConditionalCity", Strings.ConditionalCityText, new FormletParameter[]
			{
				new StringParameter("city", Strings.StringArrayDialogTitle, Strings.StringArrayDialogTitle, typeof(FilterFieldText), false)
			}, "Contacts,Mailboxes,ResourceMailboxes,SharedMailboxes,Members+", false);
			array[1] = new RulePhrase("ConditionalCompany", Strings.ConditionalCompanyText, new FormletParameter[]
			{
				new StringParameter("company", Strings.StringArrayDialogTitle, Strings.StringArrayDialogTitle, typeof(FilterFieldText), false)
			}, "Contacts,Mailboxes,ResourceMailboxes,SharedMailboxes,Members+", false);
			RulePhrase[] array2 = array;
			int num = 2;
			string name = "ConditionalCountryOrRegionText";
			LocalizedString conditionalCountryOrRegionText = Strings.ConditionalCountryOrRegionText;
			FormletParameter[] array3 = new FormletParameter[1];
			FormletParameter[] array4 = array3;
			int num2 = 0;
			EnumParameter enumParameter = new EnumParameter("countryorregion", Strings.StringArrayDialogTitle, Strings.ConditionalCountryOrRegionText, null);
			enumParameter.Values = (from ci in CountryInfo.AllCountryInfos
			select new EnumValue(ci.LocalizedDisplayName, ci.DisplayName)).ToArray<EnumValue>();
			array4[num2] = enumParameter;
			array2[num] = new RulePhrase(name, conditionalCountryOrRegionText, array3, "Contacts,Mailboxes,ResourceMailboxes,SharedMailboxes,Members+", false);
			array[3] = new RulePhrase("ConditionalCustomAttribute1", Strings.ConditionalCustomAttribute1Text, new FormletParameter[]
			{
				new StringParameter("customattribute1", Strings.StringArrayDialogTitle, Strings.StringArrayDialogTitle, typeof(FilterFieldText), false)
			}, null, false);
			array[4] = new RulePhrase("ConditionalCustomAttribute2", Strings.ConditionalCustomAttribute2Text, new FormletParameter[]
			{
				new StringParameter("customattribute2", Strings.StringArrayDialogTitle, Strings.StringArrayDialogTitle, typeof(FilterFieldText), false)
			}, null, false);
			array[5] = new RulePhrase("ConditionalCustomAttribute3", Strings.ConditionalCustomAttribute3Text, new FormletParameter[]
			{
				new StringParameter("customattribute3", Strings.StringArrayDialogTitle, Strings.StringArrayDialogTitle, typeof(FilterFieldText), false)
			}, null, false);
			array[6] = new RulePhrase("ConditionalCustomAttribute4", Strings.ConditionalCustomAttribute4Text, new FormletParameter[]
			{
				new StringParameter("customattribute4", Strings.StringArrayDialogTitle, Strings.StringArrayDialogTitle, typeof(FilterFieldText), false)
			}, null, false);
			array[7] = new RulePhrase("ConditionalCustomAttribute5", Strings.ConditionalCustomAttribute5Text, new FormletParameter[]
			{
				new StringParameter("customattribute5", Strings.StringArrayDialogTitle, Strings.StringArrayDialogTitle, typeof(FilterFieldText), false)
			}, null, false);
			array[8] = new RulePhrase("ConditionalCustomAttribute6", Strings.ConditionalCustomAttribute6Text, new FormletParameter[]
			{
				new StringParameter("customattribute6", Strings.StringArrayDialogTitle, Strings.StringArrayDialogTitle, typeof(FilterFieldText), false)
			}, null, false);
			array[9] = new RulePhrase("ConditionalCustomAttribute7", Strings.ConditionalCustomAttribute7Text, new FormletParameter[]
			{
				new StringParameter("customattribute7", Strings.StringArrayDialogTitle, Strings.StringArrayDialogTitle, typeof(FilterFieldText), false)
			}, null, false);
			array[10] = new RulePhrase("ConditionalCustomAttribute8", Strings.ConditionalCustomAttribute8Text, new FormletParameter[]
			{
				new StringParameter("customattribute8", Strings.StringArrayDialogTitle, Strings.StringArrayDialogTitle, typeof(FilterFieldText), false)
			}, null, false);
			array[11] = new RulePhrase("ConditionalCustomAttribute9", Strings.ConditionalCustomAttribute9Text, new FormletParameter[]
			{
				new StringParameter("customattribute9", Strings.StringArrayDialogTitle, Strings.StringArrayDialogTitle, typeof(FilterFieldText), false)
			}, null, false);
			array[12] = new RulePhrase("ConditionalCustomAttribute10", Strings.ConditionalCustomAttribute10Text, new FormletParameter[]
			{
				new StringParameter("customattribute10", Strings.StringArrayDialogTitle, Strings.StringArrayDialogTitle, typeof(FilterFieldText), false)
			}, null, false);
			array[13] = new RulePhrase("ConditionalCustomAttribute11", Strings.ConditionalCustomAttribute11Text, new FormletParameter[]
			{
				new StringParameter("customattribute11", Strings.StringArrayDialogTitle, Strings.StringArrayDialogTitle, typeof(FilterFieldText), false)
			}, null, false);
			array[14] = new RulePhrase("ConditionalCustomAttribute12", Strings.ConditionalCustomAttribute12Text, new FormletParameter[]
			{
				new StringParameter("customattribute12", Strings.StringArrayDialogTitle, Strings.StringArrayDialogTitle, typeof(FilterFieldText), false)
			}, null, false);
			array[15] = new RulePhrase("ConditionalCustomAttribute13", Strings.ConditionalCustomAttribute13Text, new FormletParameter[]
			{
				new StringParameter("customattribute13", Strings.StringArrayDialogTitle, Strings.StringArrayDialogTitle, typeof(FilterFieldText), false)
			}, null, false);
			array[16] = new RulePhrase("ConditionalCustomAttribute14", Strings.ConditionalCustomAttribute14Text, new FormletParameter[]
			{
				new StringParameter("customattribute14", Strings.StringArrayDialogTitle, Strings.StringArrayDialogTitle, typeof(FilterFieldText), false)
			}, null, false);
			array[17] = new RulePhrase("ConditionalCustomAttribute15", Strings.ConditionalCustomAttribute15Text, new FormletParameter[]
			{
				new StringParameter("customattribute15", Strings.StringArrayDialogTitle, Strings.StringArrayDialogTitle, typeof(FilterFieldText), false)
			}, null, false);
			array[18] = new RulePhrase("ConditionalDatabaseText", Strings.ConditionalDatabaseText, new FormletParameter[]
			{
				new ObjectParameter("database", Strings.ConditionalDatabaseText, Strings.ConditionalDatabaseText, typeof(ADObjectId), "~/Pickers/MailboxDatabasePicker.aspx?&version=*&PreVersion=1", "DistinguishedName")
			}, string.Format("{0},{1},{2}+{3}", new object[]
			{
				"Mailboxes",
				"ResourceMailboxes",
				"SharedMailboxes",
				"Enterprise"
			}), false);
			array[19] = new RulePhrase("ConditionalEmailAddressPolicyEnabled", Strings.ConditionalEmailAddressPolicyEnabled, new FormletParameter[]
			{
				new EnumParameter("emailaddresspolicyenabled", Strings.ConditionalTrueOrFalseTitle, Strings.EmptyLabel, typeof(TrueOrFalseEnum), null)
			}, "Contacts,Mailboxes,ResourceMailboxes,SharedMailboxes,Members+", false);
			array[20] = new RulePhrase("ConditionalLitigationHoldEnabled", Strings.ConditionalLitigationHoldEnabled, new FormletParameter[]
			{
				new EnumParameter("litigationholdenabled", Strings.ConditionalTrueOrFalseTitle, Strings.EmptyLabel, typeof(TrueOrFalseEnum), null)
			}, string.Format("{0},{1}+", "Mailboxes", "SharedMailboxes"), false);
			array[21] = new RulePhrase("ConditionalManagedBy", Strings.ConditionalManagedBy, new FormletParameter[]
			{
				new ObjectParameter("managedby", Strings.ConditionalManagedBy, Strings.ConditionalManagedBy, typeof(ADObjectId), "~/Pickers/OwnerPicker.aspx", "DistinguishedName")
			}, "DistributionGroups+", false);
			array[22] = new RulePhrase("ConditionalManager", Strings.ConditionalManager, new FormletParameter[]
			{
				new ObjectParameter("manager", Strings.ConditionalManager, Strings.ConditionalManager, typeof(ADObjectId), "~/Pickers/ManagerPicker.aspx", "DistinguishedName")
			}, string.Format("{0},{1},{2}+", "Contacts", "Mailboxes", "SharedMailboxes"), false);
			array[23] = new RulePhrase("ConditionalMemberOfGroup", Strings.ConditionalMemberOfGroup, new FormletParameter[]
			{
				new ObjectParameter("memberofgroup", Strings.ConditionalMemberOfGroup, Strings.ConditionalMemberOfGroup, typeof(ADObjectId), "~/Pickers/grouppicker.aspx", "DistinguishedName")
			}, "Contacts,Mailboxes,ResourceMailboxes,SharedMailboxes,Members+", false);
			array[24] = new RulePhrase("ConditionalOffice", Strings.ConditionalOfficeText, new FormletParameter[]
			{
				new StringParameter("office", Strings.StringArrayDialogTitle, Strings.StringArrayDialogTitle, typeof(FilterFieldText), false)
			}, string.Format("{0},{1},{2}+", "Contacts", "Mailboxes", "SharedMailboxes"), false);
			array[25] = new RulePhrase("ConditionalLocation", Strings.ConditionalLocationText, new FormletParameter[]
			{
				new StringParameter("location", Strings.StringArrayDialogTitle, Strings.StringArrayDialogTitle, typeof(FilterFieldText), false)
			}, string.Format("{0}+", "ResourceMailboxes"), false);
			array[26] = new RulePhrase("ConditionalServerName", Strings.ConditionalServer, new FormletParameter[]
			{
				new ObjectParameter("servername", LocalizedString.Empty, LocalizedString.Empty, typeof(string), "~/Pickers/MailboxServerPicker.aspx", "Name")
			}, string.Format("{0},{1}+{2}", "Mailboxes", "SharedMailboxes", "Enterprise"), false);
			array[27] = new RulePhrase("ConditionalStateOrProvince", Strings.ConditionalStateOrProvinceText, new FormletParameter[]
			{
				new StringParameter("stateorprovince", Strings.StringArrayDialogTitle, Strings.StringArrayDialogTitle, typeof(FilterFieldText), false)
			}, string.Format("{0},{1},{2}+", "Contacts", "Mailboxes", "SharedMailboxes"), false);
			array[28] = new RulePhrase("ConditionalTitle", Strings.ConditionalTitleText, new FormletParameter[]
			{
				new StringParameter("title", Strings.StringArrayDialogTitle, Strings.StringArrayDialogTitle, typeof(FilterFieldText), false)
			}, string.Format("{0},{1},{2}+", "Contacts", "Mailboxes", "SharedMailboxes"), false);
			array[29] = new RulePhrase("ConditionalUMEnabled", Strings.ConditionalUMEnabled, new FormletParameter[]
			{
				new EnumParameter("umenabled", Strings.ConditionalTrueOrFalseTitle, Strings.EmptyLabel, typeof(TrueOrFalseEnum), null)
			}, string.Format("{0},{1}+", "Mailboxes", "SharedMailboxes"), false);
			array[30] = new RulePhrase("ConditionalUMMailboxPolicy", Strings.ConditionalUMMailboxPolicy, new FormletParameter[]
			{
				new ObjectParameter("ummailboxpolicy", Strings.ConditionalUMMailboxPolicy, Strings.ConditionalUMMailboxPolicy, typeof(ADObjectId), "~/Pickers/UMMailboxPolicyPicker.aspx", "DistinguishedName")
			}, string.Format("{0},{1}+", "Mailboxes", "SharedMailboxes"), false);
			LocalSearchFilterEditor.allSupportedRules = array;
		}

		private const string ContactsFeatureName = "Contacts";

		private const string DGFeatureName = "DistributionGroups";

		private const string MBXFeatureName = "Mailboxes";

		private const string RMBXFeatureName = "ResourceMailboxes";

		private const string SMBXFeatureName = "SharedMailboxes";

		private const string MBFeatureName = "Members";

		private const string FullAdditionalRoles = "Contacts,Mailboxes,ResourceMailboxes,SharedMailboxes,Members+";

		private static RulePhrase[] allSupportedRules;
	}
}
