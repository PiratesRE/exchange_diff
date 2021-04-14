using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Search.AqsParser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IRecipientResolver
	{
		string[] Resolve(string identity);
	}
}
