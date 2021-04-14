using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class MowaPersonalContactsGrammarFile : PersonalContactsGrammarFile
	{
		internal MowaPersonalContactsGrammarFile(UMMailboxRecipient caller, CultureInfo culture) : base(caller, culture)
		{
		}

		internal override string ContactsRulePrefix
		{
			get
			{
				return "\r\n\t<rule id=\"Names\" scope=\"public\">\r\n\t\t<tag>$.RecoEvent={{}};</tag>\r\n\t\t<tag>$.ResultType={{}};</tag>\r\n\t\t<tag>$.UmSubscriberObjectGuid={{}};</tag>\r\n\t\t<tag>$.UmSubscriberObjectGuid._value=\"{0}\";</tag>\r\n\t\t<tag>$.ContactId={{}};</tag>\r\n\t\t<tag>$.ContactName={{}};</tag>\r\n\t\t<tag>$.DisambiguationField={{}};</tag>\r\n\t\t<tag>$.PersonId={{}};</tag>\r\n\t\t<tag>$.GALLinkID={{}};</tag>\r\n";
			}
		}

		internal override bool ShouldGenerateEmptyGrammar
		{
			get
			{
				return true;
			}
		}

		protected override List<ContactSearchItem> GetContactsList()
		{
			return base.GetContactsList(delegate(UMMailboxRecipient subscriber, IDictionary<PropertyDefinition, object> searchFilter, List<ContactSearchItem> list, int maxItemCount)
			{
				ContactSearchItem.AddMOWASearchItems(subscriber, searchFilter, list, maxItemCount);
			});
		}

		protected override void GenerateContactsCompiledGrammar(List<ContactSearchItem> list)
		{
			this.GenerateGrammar(list);
		}

		protected override void AppendContactNode(XmlWriter namesGrammarWriter, ContactSearchItem contact, string entryName, string entryDisplayName)
		{
			string entryNames = this.GetEntryNames(contact, entryName);
			namesGrammarWriter.WriteRaw(string.Format(CultureInfo.InvariantCulture, "\t\t\t<item>{0}\r\n\t\t\t\t<tag>\r\n\t\t\t\t\t$.RecoEvent._value=\"recoNameOrDepartment\";\r\n\t\t\t\t\t$.ResultType._value=\"PersonalContact\";\r\n\t\t\t\t\t$.ContactId._value=\"{1}\";\r\n\t\t\t\t\t$.ContactName._value=\"{5}\";\r\n\t\t\t\t\t$.DisambiguationField._value=\"{2}\";\r\n\t\t\t\t\t$.PersonId._value=\"{3}\";\r\n\t\t\t\t\t$.GALLinkID._value=\"{4}\";\r\n\t\t\t\t</tag>\r\n\t\t\t</item>\r\n", new object[]
			{
				entryNames,
				contact.Id,
				entryDisplayName,
				contact.PersonId,
				contact.GALLinkId,
				entryName
			}));
		}

		private string GetEntryNames(ContactSearchItem contact, string entryName)
		{
			if (!contact.IsFromRecipientCache)
			{
				return entryName;
			}
			StringBuilder stringBuilder = new StringBuilder(300);
			stringBuilder.AppendLine("\t\t<one-of>\r\n");
			if (!string.IsNullOrEmpty(contact.FirstName))
			{
				this.AppendContactItem(contact.FirstName, stringBuilder);
			}
			if (!string.IsNullOrEmpty(contact.LastName))
			{
				this.AppendContactItem(contact.LastName, stringBuilder);
			}
			this.AppendContactItem(entryName, stringBuilder);
			stringBuilder.AppendLine("\t\t</one-of>\r\n");
			return stringBuilder.ToString();
		}

		private void AppendContactItem(string item, StringBuilder sb)
		{
			sb.AppendLine(string.Format(CultureInfo.InvariantCulture, "<item>{0}</item>", new object[]
			{
				PersonalContactsGrammarFile.SrgsSanitizeString(item)
			}));
		}
	}
}
