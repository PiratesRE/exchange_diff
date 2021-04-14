using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class PersonCustomGrammar : CustomGrammarBase
	{
		protected PersonCustomGrammar(CultureInfo transctiptionLanguage, List<ContactInfo> persons) : base(transctiptionLanguage)
		{
			this.persons = persons;
		}

		protected override List<GrammarItemBase> GetItems()
		{
			List<GrammarItemBase> list = new List<GrammarItemBase>();
			foreach (ContactInfo contactInfo in this.persons)
			{
				if (!(contactInfo is DefaultContactInfo))
				{
					list.AddRange(this.GetItems(contactInfo));
				}
			}
			return list;
		}

		protected abstract List<GrammarItemBase> GetItems(ContactInfo person);

		private List<ContactInfo> persons;
	}
}
