using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class OVAPersonalContactsGrammarFile : PersonalContactsGrammarFile
	{
		internal OVAPersonalContactsGrammarFile(UMMailboxRecipient caller, CultureInfo culture) : base(caller, culture)
		{
		}

		internal override string ContactsRulePrefix
		{
			get
			{
				return "\r\n\t<rule id=\"Names\" scope=\"public\">\r\n\t\t<tag>$.RecoEvent={{}};</tag>\r\n\t\t<tag>$.ResultType={{}};</tag>\r\n\t\t<tag>$.UmSubscriberObjectGuid={{}};</tag>\r\n\t\t<tag>$.UmSubscriberObjectGuid._value=\"{0}\";</tag>\r\n\t\t<tag>$.ContactId={{}};</tag>\r\n\t\t<tag>$.ContactName={{}};</tag>\r\n\t\t<tag>$.DisambiguationField={{}};</tag>\r\n";
			}
		}

		internal override bool ShouldGenerateEmptyGrammar
		{
			get
			{
				return false;
			}
		}

		protected override List<ContactSearchItem> GetContactsList()
		{
			return base.GetContactsList(delegate(UMMailboxRecipient subscriber, IDictionary<PropertyDefinition, object> searchFilter, List<ContactSearchItem> list, int maxItemCount)
			{
				ContactSearchItem.AddSearchItems(subscriber, searchFilter, list, maxItemCount);
			});
		}

		protected override void GenerateContactsCompiledGrammar(List<ContactSearchItem> list)
		{
			if (list.Count > 0)
			{
				this.GenerateGrammar(list);
			}
			if (!this.HasEntries)
			{
				base.DisposeGrammarFile();
			}
		}

		protected override void AppendContactNode(XmlWriter namesGrammarWriter, ContactSearchItem contact, string entryName, string entryDisplayName)
		{
			namesGrammarWriter.WriteRaw(string.Format(CultureInfo.InvariantCulture, "\t\t\t<item>{0}\r\n\t\t\t\t<tag>\r\n\t\t\t\t\t$.RecoEvent._value=\"recoNameOrDepartment\";\r\n\t\t\t\t\t$.ResultType._value=\"PersonalContact\";\r\n\t\t\t\t\t$.ContactId._value=\"{1}\";\r\n\t\t\t\t\t$.ContactName._value=\"{0}\";\r\n\t\t\t\t\t$.DisambiguationField._value=\"{2}\";\r\n\t\t\t\t</tag>\r\n\t\t\t</item>\r\n", new object[]
			{
				entryName,
				contact.Id,
				entryDisplayName
			}));
		}
	}
}
