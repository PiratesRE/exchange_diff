using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PersonNameCustomGrammar : PersonCustomGrammar
	{
		internal PersonNameCustomGrammar(CultureInfo transctiptionLanguage, List<ContactInfo> persons) : base(transctiptionLanguage, persons)
		{
		}

		internal override string FileName
		{
			get
			{
				return "ExtPersonName.grxml";
			}
		}

		internal override string Rule
		{
			get
			{
				return "ExtPersonName";
			}
		}

		protected override List<GrammarItemBase> GetItems(ContactInfo person)
		{
			List<GrammarItemBase> list = new List<GrammarItemBase>(3);
			string tag = string.Format(CultureInfo.InvariantCulture, "out.{0} = \"{1}\";", new object[]
			{
				person.EwsType,
				person.EwsId
			});
			if (!string.IsNullOrEmpty(person.DisplayName))
			{
				ExclusionList instance = ExclusionList.Instance;
				if (instance != null)
				{
					List<Replacement> list2 = null;
					switch (instance.GetReplacementStrings(person.DisplayName, RecipientType.Contact, out list2))
					{
					case MatchResult.None:
					case MatchResult.MatchWithNoReplacements:
					case MatchResult.NotFound:
						goto IL_FC;
					case MatchResult.NoMatch:
						list.Add(new GrammarItem(person.DisplayName, tag, base.TranscriptionLanguage));
						goto IL_FC;
					case MatchResult.MatchWithReplacements:
						using (List<Replacement>.Enumerator enumerator = list2.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								Replacement replacement = enumerator.Current;
								list.Add(new GrammarItem(replacement.ReplacementString, tag, base.TranscriptionLanguage));
							}
							goto IL_FC;
						}
						break;
					default:
						goto IL_FC;
					}
				}
				list.Add(new GrammarItem(person.DisplayName, tag, base.TranscriptionLanguage));
			}
			IL_FC:
			if (!string.IsNullOrEmpty(person.FirstName))
			{
				list.Add(new GrammarItem(person.FirstName, tag, base.TranscriptionLanguage));
			}
			if (!string.IsNullOrEmpty(person.LastName))
			{
				list.Add(new GrammarItem(person.LastName, tag, base.TranscriptionLanguage));
			}
			return list;
		}
	}
}
