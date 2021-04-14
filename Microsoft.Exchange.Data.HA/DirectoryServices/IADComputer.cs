using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IADComputer : IADObjectCommon
	{
		string DnsHostName { get; }
	}
}
