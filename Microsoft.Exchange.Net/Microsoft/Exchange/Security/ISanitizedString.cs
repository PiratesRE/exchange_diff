using System;

namespace Microsoft.Exchange.Security
{
	public interface ISanitizedString<SanitizingPolicy> where SanitizingPolicy : ISanitizingPolicy, new()
	{
		string UntrustedValue { get; set; }

		void DecreeToBeTrusted();

		void DecreeToBeUntrusted();

		string ToString();
	}
}
