using System;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public sealed class SanitizedJScriptString : SanitizedStringBase<OwaHtml>
	{
		public SanitizedJScriptString(string value) : base(value)
		{
			base.DecreeToBeUntrusted();
		}

		private SanitizedJScriptString()
		{
		}

		public static SanitizedJScriptString StringAssignmentStatement(string varName, string varValue)
		{
			return new SanitizedJScriptString
			{
				TrustedValue = string.Concat(new string[]
				{
					"var ",
					varName,
					" = \"",
					StringSanitizer<OwaHtml>.PolicyObject.EscapeJScript(varValue),
					"\";"
				})
			};
		}

		protected override string Sanitize(string rawValue)
		{
			return StringSanitizer<OwaHtml>.PolicyObject.EscapeJScript(rawValue);
		}
	}
}
