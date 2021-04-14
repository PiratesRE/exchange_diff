using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class CustomGrammarFile : SearchGrammarFile
	{
		internal CustomGrammarFile(CustomMenuKeyMapping[] customExtensions, string rule, CultureInfo culture, string autoAttendantName) : base(culture)
		{
			this.customExtensions = customExtensions;
			this.rule = rule;
			this.autoAttendantName = autoAttendantName;
			this.GenerateGrammar();
		}

		internal override string FilePath
		{
			get
			{
				return this.customMenuGrammarFile.FilePath;
			}
		}

		internal override bool HasEntries
		{
			get
			{
				return true;
			}
		}

		private void GenerateGrammar()
		{
			StringBuilder stringBuilder = new StringBuilder();
			string path = Utils.GrammarPathFromCulture(base.Culture);
			string text = Path.Combine(path, "common.grxml");
			stringBuilder.AppendFormat("<grammar root=\"{0}\"\txml:lang=\"{2}\" version=\"1.0\" xmlns=\"http://www.w3.org/2001/06/grammar\" tag-format=\"semantics-ms/1.0\">\r\n\t<!-- NoGrammar rule recognizes phrases like 'No Sales', where No is optional -->\r\n\t<rule id=\"No{0}\" scope=\"public\">\r\n\t\t<tag>$.RecoEvent={{}};</tag>\r\n   \t\t <item repeat=\"0-1\">\r\n\t\t\t\t<ruleref uri=\"{1}#noPhrases\"/>\r\n\t\t </item>\r\n\t\t <ruleref uri=\"#{0}\"/>\r\n\t\t<tag>$=$$;</tag>\r\n\t</rule>\r\n\r\n\t<rule id=\"{0}\" scope=\"public\">\r\n\t\t<tag>$.RecoEvent={{}};</tag>\r\n\t\t<tag>$.Extension={{}};</tag>\r\n\t\t<tag>$.ResultType={{}};</tag>\r\n\t\t<tag>$.DepartmentName={{}};</tag>\r\n\t\t<tag>$.MappedKey={{}};</tag>\r\n\t\t<tag>$.CustomMenuTarget={{}};</tag>\r\n\t\t<tag>$.PromptFileName={{}};</tag>\r\n\t\t<one-of>", this.rule, text, base.Culture);
			foreach (CustomMenuKeyMapping customMenuKeyMapping in this.customExtensions)
			{
				string text2 = customMenuKeyMapping.Extension;
				AutoAttendantCustomOptionType autoAttendantCustomOptionType = AutoAttendantCustomOptionType.None;
				CustomMenuKey mappedKey = customMenuKeyMapping.MappedKey;
				if (!string.IsNullOrEmpty(text2))
				{
					autoAttendantCustomOptionType = AutoAttendantCustomOptionType.TransferToExtension;
				}
				else if (!string.IsNullOrEmpty(customMenuKeyMapping.AutoAttendantName))
				{
					autoAttendantCustomOptionType = AutoAttendantCustomOptionType.TransferToAutoAttendant;
					text2 = customMenuKeyMapping.AutoAttendantName;
				}
				else if (!string.IsNullOrEmpty(customMenuKeyMapping.LegacyDNToUseForLeaveVoicemailFor))
				{
					autoAttendantCustomOptionType = AutoAttendantCustomOptionType.TransferToVoicemailDirectly;
					text2 = customMenuKeyMapping.LegacyDNToUseForLeaveVoicemailFor;
				}
				else if (!string.IsNullOrEmpty(customMenuKeyMapping.LegacyDNToUseForTransferToMailbox))
				{
					autoAttendantCustomOptionType = AutoAttendantCustomOptionType.TransferToVoicemailPAA;
					text2 = customMenuKeyMapping.LegacyDNToUseForTransferToMailbox;
				}
				else if (!string.IsNullOrEmpty(customMenuKeyMapping.AnnounceBusinessLocation))
				{
					autoAttendantCustomOptionType = AutoAttendantCustomOptionType.ReadBusinessLocation;
				}
				else if (!string.IsNullOrEmpty(customMenuKeyMapping.AnnounceBusinessHours))
				{
					autoAttendantCustomOptionType = AutoAttendantCustomOptionType.ReadBusinessHours;
				}
				else
				{
					text2 = null;
				}
				this.ProcessGrammarElement(stringBuilder, customMenuKeyMapping.Description, mappedKey, text2, autoAttendantCustomOptionType.ToString(), customMenuKeyMapping.PromptFileName, this.autoAttendantName);
				string[] asrPhraseList = customMenuKeyMapping.AsrPhraseList;
				if (asrPhraseList != null)
				{
					for (int j = 0; j < asrPhraseList.Length; j++)
					{
						if (string.Compare(asrPhraseList[j], customMenuKeyMapping.Description, StringComparison.OrdinalIgnoreCase) != 0)
						{
							this.ProcessGrammarElement(stringBuilder, asrPhraseList[j], mappedKey, text2, autoAttendantCustomOptionType.ToString(), customMenuKeyMapping.PromptFileName, this.autoAttendantName);
						}
					}
				}
			}
			stringBuilder.AppendFormat("\t\t</one-of>\r\n\t\t<!-- the following will add an option politeending to the recognition -->\r\n\t   \t<item repeat=\"0-1\">\r\n\t\t\t<ruleref uri=\"{0}#politeEndPhrases\"/>\r\n\t\t</item>\r\n\t</rule>\r\n</grammar>", text);
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, this, "Department Grammar: {0}.", new object[]
			{
				stringBuilder
			});
			this.customMenuGrammarFile = TempFileFactory.CreateTempGrammarFile();
			using (StreamWriter streamWriter = new StreamWriter(this.customMenuGrammarFile.FilePath, false, Encoding.UTF8))
			{
				streamWriter.Write(stringBuilder.ToString());
			}
		}

		private void ProcessGrammarElement(StringBuilder sb, string name, CustomMenuKey keyPress, string extension, string target, string promptFileName, string autoAttendantName)
		{
			try
			{
				if (!string.IsNullOrEmpty(name))
				{
					Platform.Utilities.CheckGrammarEntryFormat(name);
					sb.AppendFormat(CultureInfo.InvariantCulture, "\t\t\t <item>{0}\r\n\t\t\t\t <tag>\r\n\t\t\t\t\t $.RecoEvent._value=\"recoNameOrDepartment\";\r\n\t\t\t\t\t $.Extension._value=\"{1}\";\r\n\t\t\t\t\t $.ResultType._value=\"Department\";\r\n\t\t\t\t\t $.DepartmentName._value=\"{0}\";\r\n\t\t\t\t\t $.MappedKey._value=\"{4}\";\r\n\t\t\t\t\t $.CustomMenuTarget._value = \"{2}\"\r\n\t\t\t\t\t $.PromptFileName._value = \"{3}\"\r\n\t\t\t\t </tag>\r\n\t\t\t </item>", new object[]
					{
						SpeechUtils.SrgsEncode(name),
						SpeechUtils.SrgsEncode(extension),
						target,
						SpeechUtils.SrgsEncode(promptFileName),
						keyPress
					});
				}
			}
			catch (FormatException ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.AutoAttendantTracer, this, "An error was encountered while writing custom menu entry '{0}' for Auto Attendant '{1}' in the grammar file. The error was '{2}'. Continuing...", new object[]
				{
					name,
					autoAttendantName,
					ex.Message
				});
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_SpeechAAGrammarEntryFormatErrors, null, new object[]
				{
					name,
					autoAttendantName,
					ex.Message
				});
			}
		}

		private ITempFile customMenuGrammarFile;

		private CustomMenuKeyMapping[] customExtensions;

		private string rule;

		private string autoAttendantName;
	}
}
