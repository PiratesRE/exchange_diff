using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IContactLinkingMatchProperties
	{
		HashSet<string> EmailAddresses { get; }

		string IMAddress { get; }
	}
}
