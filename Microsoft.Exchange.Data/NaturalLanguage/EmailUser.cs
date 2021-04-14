using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.NaturalLanguage
{
	public class EmailUser : IEquatable<EmailUser>
	{
		[XmlText]
		public string Name { get; set; }

		[XmlAttribute("Id")]
		public string UserId { get; set; }

		public bool Equals(EmailUser other)
		{
			return other != null && this.UserId == other.UserId;
		}
	}
}
