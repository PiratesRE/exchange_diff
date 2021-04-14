using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct IdentityPair
	{
		public string LogonUserSid { get; set; }

		public string LogonUserDisplayName { get; set; }
	}
}
