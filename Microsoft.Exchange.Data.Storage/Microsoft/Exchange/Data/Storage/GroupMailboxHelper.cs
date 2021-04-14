using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class GroupMailboxHelper
	{
		internal const string ConfigureGroupMailboxSessionClientString = "Client=WebServices;Action=ConfigureGroupMailbox";
	}
}
