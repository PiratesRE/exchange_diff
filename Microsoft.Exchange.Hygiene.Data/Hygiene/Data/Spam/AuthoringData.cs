using System;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	public class AuthoringData
	{
		public long GroupID { get; set; }

		public byte RuleTarget { get; set; }

		public string Regex { get; set; }

		public byte LanguageID { get; set; }

		public byte Category { get; set; }

		public string Flags { get; set; }

		public byte ActionID { get; set; }
	}
}
