using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PromptForAliasGrammarFile : SearchGrammarFile
	{
		internal PromptForAliasGrammarFile(List<IUMRecognitionPhrase> resultList, CultureInfo culture) : base(culture)
		{
			this.userList = resultList;
			this.GenerateGrammar();
		}

		internal override string FilePath
		{
			get
			{
				return this.grammarFile.FilePath;
			}
		}

		internal override bool HasEntries
		{
			get
			{
				return this.hasEntries;
			}
		}

		private void GenerateGrammar()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.hasEntries = false;
			bool flag = false;
			foreach (IUMRecognitionPhrase iumrecognitionPhrase in this.userList)
			{
				string text = (string)iumrecognitionPhrase["SMTP"];
				string text2 = iumrecognitionPhrase["ContactName"] as string;
				string text3 = (string)iumrecognitionPhrase["ObjectGuid"];
				if (string.IsNullOrEmpty(text))
				{
					PIIMessage data = PIIMessage.Create(PIIType._PII, text2);
					CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, data, "PromptForAliasGrammarFile::Skipping contact with empty emailaddress: _PII.", new object[0]);
				}
				else
				{
					this.hasEntries = true;
					if (!flag)
					{
						flag = true;
						stringBuilder.AppendFormat("<grammar xml:lang=\"{0}\" version=\"1.0\" xmlns=\"http://www.w3.org/2001/06/grammar\" tag-format=\"semantics-ms/1.0\">\r\n\r\n\t<rule id=\"Names\" scope=\"public\">\r\n\t\t<tag>$.RecoEvent={{}};</tag>\r\n\t\t<tag>$.ResultType={{}};</tag>\r\n\t\t<tag>$.ObjectGuid={{}};</tag>\r\n\t\t<tag>$.SMTP={{}};</tag>\r\n\t\t<tag>$.ContactName={{}};</tag>\r\n\t\t<one-of>\r\n", base.Culture);
					}
					stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "\t\t\t<item>{0}\r\n\t\t\t\t<tag>\r\n\t\t\t\t\t$.RecoEvent._value=\"recoNameOrDepartment\";\r\n\t\t\t\t\t$.ResultType._value=\"DirectoryContact\";\r\n\t\t\t\t\t$.ObjectGuid._value=\"{3}\";\r\n\t\t\t\t\t$.SMTP._value=\"{1}\";\r\n\t\t\t\t\t$.ContactName._value=\"{2}\";\r\n\t\t\t\t</tag>\r\n\t\t\t</item>\r\n", new object[]
					{
						this.EncodeAlias(text),
						SpeechUtils.SrgsEncode(text),
						text2,
						text3
					}));
				}
			}
			if (!this.hasEntries)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, "PromptForAliasGrammarFile::GenerateGrammar - grammar is empty. No grammar file will be written", new object[0]);
				return;
			}
			stringBuilder.AppendFormat("\r\n\t\t</one-of>\r\n\t\t<!-- the following will add an option politeending to the recognition -->\r\n\t\t<item repeat=\"0-1\">\r\n\t\t\t<ruleref uri=\"{0}#politeEndPhrases\"/>\r\n\t\t</item>\r\n\t</rule>\r\n</grammar>", Utils.GetCommonGrammarFilePath(base.Culture));
			CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, "Prompt For Alias Grammar File: {0}.", new object[]
			{
				stringBuilder
			});
			using (StreamWriter streamWriter = new StreamWriter(this.grammarFile.FilePath))
			{
				streamWriter.Write(stringBuilder.ToString());
			}
		}

		private string EncodeAlias(string email)
		{
			if (string.IsNullOrEmpty(email))
			{
				throw new ArgumentNullException(email);
			}
			string text = email;
			int num = email.IndexOf("@", StringComparison.InvariantCulture);
			if (num != -1)
			{
				text = email.Substring(0, num);
			}
			text = text.ToLowerInvariant();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c in text)
			{
				if (char.IsLetterOrDigit(c))
				{
					stringBuilder.Append(" ");
					if (c >= '0' && c <= '9')
					{
						int num2 = Convert.ToInt32(c) - Convert.ToInt32('0');
						stringBuilder.Append(PromptForAliasGrammarFile.digitMap[num2]);
					}
					else
					{
						stringBuilder.Append(c);
						stringBuilder.Append(".");
					}
				}
				else
				{
					char c2 = c;
					switch (c2)
					{
					case '#':
						stringBuilder.Append(" ");
						stringBuilder.Append("pound");
						break;
					case '$':
						stringBuilder.Append(" ");
						stringBuilder.Append("dollar");
						break;
					default:
						switch (c2)
						{
						case '-':
							stringBuilder.Append(" ");
							stringBuilder.Append("dash");
							break;
						case '.':
							stringBuilder.Append(" ");
							stringBuilder.Append("dot");
							break;
						default:
							if (c2 == '_')
							{
								stringBuilder.Append(" ");
								stringBuilder.Append("underscore");
							}
							break;
						}
						break;
					}
				}
			}
			PIIMessage data = PIIMessage.Create(PIIType._EmailAddress, email);
			CallIdTracer.TraceDebug(ExTraceGlobals.AsrContactsTracer, this, data, "EncodeAlias(_EmailAddress) returning {0}.", new object[]
			{
				stringBuilder
			});
			return stringBuilder.ToString();
		}

		private static string[] digitMap = new string[]
		{
			"zero",
			"one",
			"two",
			"three",
			"four",
			"five",
			"six",
			"seven",
			"eight",
			"nine"
		};

		private ITempFile grammarFile = TempFileFactory.CreateTempGrammarFile();

		private List<IUMRecognitionPhrase> userList;

		private bool hasEntries;
	}
}
