using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PersonInfoCustomGrammar : PersonCustomGrammar
	{
		internal PersonInfoCustomGrammar(CultureInfo transctiptionLanguage, List<ContactInfo> persons) : base(transctiptionLanguage, persons)
		{
		}

		internal override string FileName
		{
			get
			{
				return "ExtCallerInfo.grxml";
			}
		}

		internal override string Rule
		{
			get
			{
				return "ExtCallerInfo";
			}
		}

		protected override List<GrammarItemBase> GetItems(ContactInfo person)
		{
			List<GrammarItemBase> list = new List<GrammarItemBase>(3);
			if (!string.IsNullOrEmpty(person.Company))
			{
				list.Add(new GrammarItem(person.Company, base.TranscriptionLanguage));
			}
			if (!string.IsNullOrEmpty(person.City))
			{
				list.Add(new GrammarItem(person.City, base.TranscriptionLanguage));
			}
			if (!string.IsNullOrEmpty(person.Country))
			{
				list.Add(new GrammarItem(person.Country, base.TranscriptionLanguage));
			}
			return list;
		}
	}
}
