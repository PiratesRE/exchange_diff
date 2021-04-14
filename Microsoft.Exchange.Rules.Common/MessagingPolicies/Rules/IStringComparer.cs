using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal interface IStringComparer
	{
		bool Equals(string x, string y);
	}
}
