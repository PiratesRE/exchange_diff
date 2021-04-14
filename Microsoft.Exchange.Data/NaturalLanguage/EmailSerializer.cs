using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class EmailSerializer : BaseSerializer<Email>
	{
		protected override XmlSerializer GetSerializer()
		{
			return EmailSerializer.serializer;
		}

		private static XmlSerializer serializer = new XmlSerializer(typeof(Email[]), new XmlRootAttribute("Emails"));
	}
}
