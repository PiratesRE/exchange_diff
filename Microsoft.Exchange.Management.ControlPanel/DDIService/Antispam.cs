using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.ControlPanel;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class Antispam
	{
		public static void GetConnectionFilterPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			List<IPAddressEntry> list = new List<IPAddressEntry>();
			List<IPAddressEntry> list2 = new List<IPAddressEntry>();
			if (!DDIHelper.IsEmptyValue(dataRow["IPAllowList"]))
			{
				foreach (IPRange range in ((MultiValuedProperty<IPRange>)dataRow["IPAllowList"]))
				{
					list.Add(new IPAddressEntry(range));
				}
			}
			if (!DDIHelper.IsEmptyValue(dataRow["IPBlockList"]))
			{
				foreach (IPRange range2 in ((MultiValuedProperty<IPRange>)dataRow["IPBlockList"]))
				{
					list2.Add(new IPAddressEntry(range2));
				}
			}
			dataRow["IPAllowList"] = list;
			dataRow["IPBlockList"] = list2;
		}

		public static void GetContentFilterPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow row = dataTable.Rows[0];
			Antispam.contentEmailListParameters.ForEach(delegate(string p)
			{
				if (!row[p].IsNullValue())
				{
					MultiValuedProperty<SmtpAddress> multiValuedProperty = (MultiValuedProperty<SmtpAddress>)row[p];
					row[string.Format("str{0}", p)] = multiValuedProperty.ToStringArray<SmtpAddress>().StringArrayJoin("; ");
				}
			});
			List<Identity> list = new List<Identity>();
			List<Identity> list2 = new List<Identity>();
			LanguageList languageList = new LanguageList();
			if (!row["RegionBlockList"].IsNullValue())
			{
				foreach (string text in ((MultiValuedProperty<string>)row["RegionBlockList"]))
				{
					list.Add(new Identity(text.ToString(), Antispam.GetRegionDisplayName(text)));
				}
				row["regionList"] = list.ToArray();
			}
			if (!row["LanguageBlockList"].IsNullValue())
			{
				foreach (string text2 in ((MultiValuedProperty<string>)row["LanguageBlockList"]))
				{
					list2.Add(new Identity(text2.ToString(), RtlUtil.ConvertToDecodedBidiString(languageList.GetDisplayValue(text2), RtlUtil.IsRtl)));
				}
				row["languageList"] = list2.ToArray();
			}
		}

		public static void GetOutboundSpamFilterPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow row = dataTable.Rows[0];
			Antispam.outboundEmailListParameters.ForEach(delegate(string p)
			{
				if (!row[p].IsNullValue())
				{
					MultiValuedProperty<SmtpAddress> multiValuedProperty = (MultiValuedProperty<SmtpAddress>)row[p];
					row[string.Format("str{0}", p)] = multiValuedProperty.ToStringArray<SmtpAddress>().StringArrayJoin("; ");
				}
			});
		}

		public static void GetConnectionFilterSDOPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			List<IPAddressEntry> list = new List<IPAddressEntry>();
			List<IPAddressEntry> list2 = new List<IPAddressEntry>();
			if (!DDIHelper.IsEmptyValue(dataRow["IPAllowList"]))
			{
				foreach (IPRange range in ((MultiValuedProperty<IPRange>)dataRow["IPAllowList"]))
				{
					list.Add(new IPAddressEntry(range));
				}
			}
			if (!DDIHelper.IsEmptyValue(dataRow["IPBlockList"]))
			{
				foreach (IPRange range2 in ((MultiValuedProperty<IPRange>)dataRow["IPBlockList"]))
				{
					list2.Add(new IPAddressEntry(range2));
				}
			}
			dataRow["IPAllowList"] = list;
			dataRow["IPBlockList"] = list2;
			if (list.Count > 0)
			{
				dataRow["AllowListStatus"] = Strings.ASConfigured.ToString();
			}
			else
			{
				dataRow["AllowListStatus"] = Strings.ASNotConfigured.ToString();
			}
			if (list2.Count > 0)
			{
				dataRow["BlockListStatus"] = Strings.ASConfigured.ToString();
				return;
			}
			dataRow["BlockListStatus"] = Strings.ASNotConfigured.ToString();
		}

		public static void GetContentFilterSDOPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow row = dataTable.Rows[0];
			Antispam.contentEmailListParameters.ForEach(delegate(string p)
			{
				if (!row[p].IsNullValue())
				{
					MultiValuedProperty<SmtpAddress> multiValuedProperty3 = (MultiValuedProperty<SmtpAddress>)row[p];
					row[string.Format("str{0}", p)] = multiValuedProperty3.ToStringArray<SmtpAddress>().StringArrayJoin("; ");
				}
			});
			row["FalsePositiveStatus"] = ((!string.IsNullOrEmpty(row["strFalsePositiveAdditionalRecipients"].ToString())) ? Strings.ASConfigured.ToString() : Strings.ASNotConfigured.ToString());
			MultiValuedProperty<string> multiValuedProperty = new MultiValuedProperty<string>();
			if (Antispam.ParsedASFEIsOn(row["IncreaseScoreWithImageLinks"].ToString()))
			{
				multiValuedProperty.Add(Strings.IncreaseScoreWithImageLinksSDO);
			}
			if (Antispam.ParsedASFEIsOn(row["IncreaseScoreWithNumericIps"].ToString()))
			{
				multiValuedProperty.Add(Strings.IncreaseScoreWithNumericIPsSDO);
			}
			if (Antispam.ParsedASFEIsOn(row["IncreaseScoreWithRedirectToOtherPort"].ToString()))
			{
				multiValuedProperty.Add(Strings.IncreaseScoreWithRedirectToOtherPortSDO);
			}
			if (Antispam.ParsedASFEIsOn(row["IncreaseScoreWithBizOrInfoUrls"].ToString()))
			{
				multiValuedProperty.Add(Strings.IncreaseScoreWithBizContentFilteringSDO);
			}
			if (Antispam.ParsedASFEIsOn(row["MarkAsSpamEmptyMessages"].ToString()))
			{
				multiValuedProperty.Add(Strings.MarkAsSpamEmptyMessagesSDO);
			}
			if (Antispam.ParsedASFEIsOn(row["MarkAsSpamJavaScriptInHtml"].ToString()))
			{
				multiValuedProperty.Add(Strings.MarkAsSpamJavaScriptInHtmlSDO);
			}
			if (Antispam.ParsedASFEIsOn(row["MarkAsSpamFramesInHtml"].ToString()))
			{
				multiValuedProperty.Add(Strings.MarkAsSpamFramesInhtmlSDO);
			}
			if (Antispam.ParsedASFEIsOn(row["MarkAsSpamObjectTagsInHtml"].ToString()))
			{
				multiValuedProperty.Add(Strings.MarkAsSpamObjectTagsInHtmlSDO);
			}
			if (Antispam.ParsedASFEIsOn(row["MarkAsSpamEmbedTagsInHtml"].ToString()))
			{
				multiValuedProperty.Add(Strings.MarkAsSpamEmbedTagsInHtmlSDO);
			}
			if (Antispam.ParsedASFEIsOn(row["MarkAsSpamFormTagsInHtml"].ToString()))
			{
				multiValuedProperty.Add(Strings.MarkAsSpamFormTagsInHtmlSDO);
			}
			if (Antispam.ParsedASFEIsOn(row["MarkAsSpamWebBugsInHtml"].ToString()))
			{
				multiValuedProperty.Add(Strings.MarkAsSpamWebBugsInHtmlSDO);
			}
			if (Antispam.ParsedASFEIsOn(row["MarkAsSpamSensitiveWordList"].ToString()))
			{
				multiValuedProperty.Add(Strings.MarkAsSpamSensitiveWordListSDO);
			}
			if (Antispam.ParsedASFEIsOn(row["MarkAsSpamSpfRecordHardFail"].ToString()))
			{
				multiValuedProperty.Add(Strings.MarkAsSpamSpfRecordHardFailSDO);
			}
			if (Antispam.ParsedASFEIsOn(row["MarkAsSpamFromAddressAuthFail"].ToString()))
			{
				multiValuedProperty.Add(Strings.MarkAsSpamFromAddressAuthFailSDO);
			}
			if (Antispam.ParsedASFEIsOn(row["MarkAsSpamNdrBackscatter"].ToString()))
			{
				multiValuedProperty.Add(Strings.MarkAsSpamNdrBackscatterSDO);
			}
			if (Antispam.ParsedASFEIsOn(row["MarkAsSpamBulkMail"].ToString()))
			{
				multiValuedProperty.Add(Strings.MarkAsSpamBulkMailSDO);
			}
			if (multiValuedProperty.Count == 0)
			{
				multiValuedProperty.Add(Strings.ASNone);
			}
			row["ASFOnOutput"] = multiValuedProperty;
			MultiValuedProperty<string> multiValuedProperty2 = new MultiValuedProperty<string>();
			if (Antispam.ParsedASFEIsTest(row["IncreaseScoreWithImageLinks"].ToString()))
			{
				multiValuedProperty2.Add(Strings.IncreaseScoreWithImageLinksSDO);
			}
			if (Antispam.ParsedASFEIsTest(row["IncreaseScoreWithNumericIps"].ToString()))
			{
				multiValuedProperty2.Add(Strings.IncreaseScoreWithNumericIPsSDO);
			}
			if (Antispam.ParsedASFEIsTest(row["IncreaseScoreWithRedirectToOtherPort"].ToString()))
			{
				multiValuedProperty2.Add(Strings.IncreaseScoreWithRedirectToOtherPortSDO);
			}
			if (Antispam.ParsedASFEIsTest(row["IncreaseScoreWithBizOrInfoUrls"].ToString()))
			{
				multiValuedProperty2.Add(Strings.IncreaseScoreWithBizContentFilteringSDO);
			}
			if (Antispam.ParsedASFEIsTest(row["MarkAsSpamEmptyMessages"].ToString()))
			{
				multiValuedProperty2.Add(Strings.MarkAsSpamEmptyMessagesSDO);
			}
			if (Antispam.ParsedASFEIsTest(row["MarkAsSpamJavaScriptInHtml"].ToString()))
			{
				multiValuedProperty2.Add(Strings.MarkAsSpamJavaScriptInHtmlSDO);
			}
			if (Antispam.ParsedASFEIsTest(row["MarkAsSpamFramesInHtml"].ToString()))
			{
				multiValuedProperty2.Add(Strings.MarkAsSpamFramesInhtmlSDO);
			}
			if (Antispam.ParsedASFEIsTest(row["MarkAsSpamObjectTagsInHtml"].ToString()))
			{
				multiValuedProperty2.Add(Strings.MarkAsSpamObjectTagsInHtmlSDO);
			}
			if (Antispam.ParsedASFEIsTest(row["MarkAsSpamEmbedTagsInHtml"].ToString()))
			{
				multiValuedProperty2.Add(Strings.MarkAsSpamEmbedTagsInHtmlSDO);
			}
			if (Antispam.ParsedASFEIsTest(row["MarkAsSpamFormTagsInHtml"].ToString()))
			{
				multiValuedProperty2.Add(Strings.MarkAsSpamFormTagsInHtmlSDO);
			}
			if (Antispam.ParsedASFEIsTest(row["MarkAsSpamWebBugsInHtml"].ToString()))
			{
				multiValuedProperty2.Add(Strings.MarkAsSpamWebBugsInHtmlSDO);
			}
			if (Antispam.ParsedASFEIsTest(row["MarkAsSpamSensitiveWordList"].ToString()))
			{
				multiValuedProperty2.Add(Strings.MarkAsSpamSensitiveWordListSDO);
			}
			if (Antispam.ParsedASFEIsTest(row["MarkAsSpamSpfRecordHardFail"].ToString()))
			{
				multiValuedProperty2.Add(Strings.MarkAsSpamSpfRecordHardFailSDO);
			}
			if (Antispam.ParsedASFEIsTest(row["MarkAsSpamFromAddressAuthFail"].ToString()))
			{
				multiValuedProperty2.Add(Strings.MarkAsSpamFromAddressAuthFailSDO);
			}
			if (Antispam.ParsedASFEIsTest(row["MarkAsSpamNdrBackscatter"].ToString()))
			{
				multiValuedProperty2.Add(Strings.MarkAsSpamNdrBackscatterSDO);
			}
			if (Antispam.ParsedASFEIsTest(row["MarkAsSpamBulkMail"].ToString()))
			{
				multiValuedProperty2.Add(Strings.MarkAsSpamBulkMailSDO);
			}
			if (multiValuedProperty2.Count == 0)
			{
				multiValuedProperty2.Add(Strings.ASNone);
			}
			row["ASFTestOutput"] = multiValuedProperty2;
			string value = string.Empty;
			switch ((SpamFilteringTestModeAction)Enum.Parse(typeof(SpamFilteringTestModeAction), row["TestModeAction"].ToString(), true))
			{
			case SpamFilteringTestModeAction.None:
				value = Strings.ASNone;
				break;
			case SpamFilteringTestModeAction.AddXHeader:
				value = Strings.AddTestXHeaderSDOLabel;
				break;
			case SpamFilteringTestModeAction.BccMessage:
				value = Strings.BccMessageSDOLabel;
				break;
			}
			row["TestModeDisplay"] = value;
		}

		public static void PostGetForSDOActionRule(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow dataRow = dataTable.Rows[0];
			if (!DDIHelper.IsEmptyValue(dataRow["Description"]))
			{
				RuleDescription ruleDescription = (RuleDescription)dataRow["Description"];
				dataRow["RuleDescriptionIf"] = ruleDescription.RuleDescriptionIf;
				dataRow["RuleDescriptionTakeActions"] = ruleDescription.RuleDescriptionTakeActions;
				dataRow["RuleDescriptionExceptIf"] = ruleDescription.RuleDescriptionExceptIf;
				dataRow["ConditionDescriptions"] = ruleDescription.ConditionDescriptions.ToArray();
				dataRow["ActionDescriptions"] = ruleDescription.ActionDescriptions.ToArray();
				dataRow["ExceptionDescriptions"] = ruleDescription.ExceptionDescriptions.ToArray();
			}
			dataRow["CanToggleESN"] = (!DBNull.Value.Equals(dataRow["RecipientDomainIs"]) && DBNull.Value.Equals(dataRow["SentTo"]) && DBNull.Value.Equals(dataRow["SentToMemberOf"]) && DBNull.Value.Equals(dataRow["ExceptIfSentTo"]) && DBNull.Value.Equals(dataRow["ExceptIfSentToMemberOf"]) && DBNull.Value.Equals(dataRow["ExceptIfRecipientDomainIs"]));
		}

		public static void PostGetListWorkflow(DataRow inputrow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.NewRow();
			dataRow["Identity"] = new Identity(Guid.Empty.ToString(), "Default");
			dataRow["RuleName"] = "Default";
			dataRow["State"] = "Enabled";
			dataRow["Priority"] = int.MaxValue;
			dataTable.Rows.Add(dataRow);
		}

		public static void GetOutboundSpamFilterSDOPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			if (dataTable.Rows.Count == 0)
			{
				return;
			}
			DataRow row = dataTable.Rows[0];
			Antispam.outboundEmailListParameters.ForEach(delegate(string p)
			{
				if (!row[p].IsNullValue())
				{
					MultiValuedProperty<SmtpAddress> multiValuedProperty = (MultiValuedProperty<SmtpAddress>)row[p];
					row[string.Format("str{0}", p)] = multiValuedProperty.ToStringArray<SmtpAddress>().StringArrayJoin("; ");
				}
			});
		}

		public static void SetContentFilterPreAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow row = dataTable.Rows[0];
			List<string> modifiedColumns = new List<string>();
			Antispam.contentEmailListParameters.ForEach(delegate(string p)
			{
				string text = string.Format("str{0}", p);
				if (!DBNull.Value.Equals(row[text]))
				{
					string[] array = row[text].ToString().Split(new char[]
					{
						';'
					}, StringSplitOptions.RemoveEmptyEntries);
					MultiValuedProperty<SmtpAddress> multiValuedProperty3 = new MultiValuedProperty<SmtpAddress>();
					foreach (string text2 in array)
					{
						multiValuedProperty3.Add(new SmtpAddress(text2.Trim()));
					}
					inputRow[text] = multiValuedProperty3;
					modifiedColumns.Add(text);
				}
			});
			MultiValuedProperty<string> multiValuedProperty = new MultiValuedProperty<string>();
			if (!row["regionList"].IsNullValue())
			{
				foreach (object obj in ((Array)row["regionList"]))
				{
					Identity identity = (Identity)obj;
					multiValuedProperty.Add(identity.RawIdentity);
				}
				if (multiValuedProperty.Count > 0)
				{
					inputRow["RegionBlockList"] = multiValuedProperty;
				}
				else
				{
					inputRow["RegionBlockList"] = null;
				}
			}
			else
			{
				inputRow["RegionBlockList"] = null;
			}
			modifiedColumns.Add("RegionBlockList");
			MultiValuedProperty<string> multiValuedProperty2 = new MultiValuedProperty<string>();
			if (!row["languageList"].IsNullValue())
			{
				foreach (object obj2 in ((Array)row["languageList"]))
				{
					Identity identity2 = (Identity)obj2;
					multiValuedProperty2.Add(identity2.RawIdentity);
				}
				if (multiValuedProperty2.Count > 0)
				{
					inputRow["LanguageBlockList"] = multiValuedProperty2;
				}
				else
				{
					inputRow["LanguageBlockList"] = null;
				}
			}
			else
			{
				inputRow["LanguageBlockList"] = null;
			}
			modifiedColumns.Add("LanguageBlockList");
			if (!DBNull.Value.Equals(row["SentTo"]) || !DBNull.Value.Equals(row["SentToMemberOf"]) || !DBNull.Value.Equals(row["ExceptIfSentTo"]) || !DBNull.Value.Equals(row["ExceptIfSentToMemberOf"]) || !DBNull.Value.Equals(row["ExceptIfRecipientDomainIs"]))
			{
				inputRow["EnableEndUserSpamNotifications"] = false;
				modifiedColumns.Add("EnableEndUserSpamNotifications");
			}
			if (modifiedColumns.Count > 0)
			{
				store.SetModifiedColumns(modifiedColumns);
			}
		}

		public static void SetOutboundSpamFilterPreAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow row = dataTable.Rows[0];
			List<string> modifiedColumns = new List<string>();
			Antispam.outboundEmailListParameters.ForEach(delegate(string p)
			{
				string text = string.Format("str{0}", p);
				if (!DBNull.Value.Equals(row[text]))
				{
					string[] array = row[text].ToString().Split(new char[]
					{
						';'
					}, StringSplitOptions.RemoveEmptyEntries);
					MultiValuedProperty<SmtpAddress> multiValuedProperty = new MultiValuedProperty<SmtpAddress>();
					foreach (string text2 in array)
					{
						multiValuedProperty.Add(new SmtpAddress(text2.Trim()));
					}
					inputRow[text] = multiValuedProperty;
					modifiedColumns.Add(text);
				}
			});
			if (modifiedColumns.Count > 0)
			{
				store.SetModifiedColumns(modifiedColumns);
			}
		}

		private static bool ParsedASFEIsOn(string asfSetting)
		{
			return (SpamFilteringOption)Enum.Parse(typeof(SpamFilteringOption), asfSetting) == SpamFilteringOption.On;
		}

		private static bool ParsedASFEIsTest(string asfSetting)
		{
			return (SpamFilteringOption)Enum.Parse(typeof(SpamFilteringOption), asfSetting) == SpamFilteringOption.Test;
		}

		public static void BuildRegionList(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			dataTable.BeginLoadData();
			Array.ForEach<string>(HygieneUtils.iso3166Alpha2Codes, delegate(string code)
			{
				string text = code.ToUpper().ToString();
				string regionDisplayName = Antispam.GetRegionDisplayName(text);
				DataRow dataRow = dataTable.NewRow();
				dataRow["RegionCode"] = text;
				dataRow["RegionName"] = regionDisplayName;
				dataRow["Identity"] = new Identity(text, regionDisplayName);
				dataTable.Rows.Add(dataRow);
			});
			dataTable.EndLoadData();
		}

		public static void BuildLanguageList(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			LanguageList lang = new LanguageList();
			dataTable.BeginLoadData();
			Array.ForEach<string>(HygieneUtils.antispamFilterableLanguages, delegate(string code)
			{
				string text = code.ToUpper().ToString();
				DataRow dataRow = dataTable.NewRow();
				try
				{
					dataRow["LanguageCode"] = text;
					dataRow["LanguageName"] = RtlUtil.ConvertToDecodedBidiString(lang.GetDisplayValue(code), RtlUtil.IsRtl);
					dataRow["Identity"] = new Identity(text, dataRow["LanguageName"].ToString());
				}
				catch
				{
					dataRow["LanguageCode"] = text;
					dataRow["LanguageName"] = text;
					dataRow["Identity"] = new Identity(text, text);
				}
				dataTable.Rows.Add(dataRow);
			});
			dataTable.EndLoadData();
		}

		private static string GetRegionDisplayName(string countryCode)
		{
			string result;
			try
			{
				CountryInfo countryInfo = CountryInfo.Parse(countryCode);
				result = RtlUtil.ConvertToDecodedBidiString(Strings.GetLocalizedString((Strings.IDs)Enum.Parse(typeof(Strings.IDs), countryInfo.UniqueId)), RtlUtil.IsRtl);
			}
			catch
			{
				try
				{
					result = RtlUtil.ConvertToDecodedBidiString(Strings.GetLocalizedString((Strings.IDs)Enum.Parse(typeof(Strings.IDs), string.Format("Region_{0}", countryCode))), RtlUtil.IsRtl);
				}
				catch
				{
					result = countryCode;
				}
			}
			return result;
		}

		public static bool IsDefaultPolicyIdentity(object identity)
		{
			return identity is Identity && string.Compare(((Identity)identity).RawIdentity, Guid.Empty.ToString(), true) == 0;
		}

		public static PeopleIdentity[] ConvertToPeopleIdentity(object identity)
		{
			if (identity is RecipientIdParameter[])
			{
				return Identity.ConvertToPeopleIdentity((RecipientIdParameter[])identity);
			}
			return null;
		}

		private static readonly List<string> emailListParameters = new List<string>
		{
			"FalsePositiveAdditionalRecipients",
			"RedirectToRecipients",
			"TestModeBccToRecipients",
			"BccSuspiciousOutboundAdditionalRecipients",
			"NotifyOutboundSpamRecipients"
		};

		private static readonly List<string> contentEmailListParameters = new List<string>
		{
			"FalsePositiveAdditionalRecipients",
			"RedirectToRecipients",
			"TestModeBccToRecipients"
		};

		private static readonly List<string> outboundEmailListParameters = new List<string>
		{
			"BccSuspiciousOutboundAdditionalRecipients",
			"NotifyOutboundSpamRecipients"
		};
	}
}
